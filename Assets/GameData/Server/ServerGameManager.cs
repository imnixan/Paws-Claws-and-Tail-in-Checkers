using System;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using GameData.Scripts;
using JetBrains.Annotations;
using PCTC.Enums;
using PCTC.Game;
using PCTC.Server;
using PCTC.Structs;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PCTC.Server
{
    public class ServerGameManager
    {
        private PlayersCommunicator playersCommunicator;
        private MoveChecker moveChecker;
        private MoveMaker moveMaker;
        private bool[] playerReadyMarks;
        private int _currentPlayer;
        private int playersCount;
        private CatsCount catsCount;
        public GameField gameField { get; private set; }

        private Enums.GameData.GameState gameState;

        private int currentPlayer
        {
            get { return _currentPlayer; }
            set
            {
                _currentPlayer = value >= playersCount ? 0 : value;
                Debug.Log($"current player {_currentPlayer}");
            }
        }

        public ServerGameManager(PlayersCommunicator playersCommunicator, int playersCount)
        {
            gameState = Enums.GameData.GameState.GameStart;
            this.playersCount = playersCount;
            playerReadyMarks = new bool[playersCount];
            gameField = new GameField();
            this.playersCommunicator = playersCommunicator;
            playersCommunicator.Init(this);
            moveChecker = new MoveChecker(gameField);
            moveMaker = new MoveMaker(gameField, moveChecker);
            InitAllPlayers();
        }

        private void StartGame()
        {
            currentPlayer = 0;
            gameState = Enums.GameData.GameState.Game;
            playersCommunicator.playerDataSender.SendAllPlayersOrder(currentPlayer);
            playersCommunicator.playerDataSender.SendAllGameStart();
        }

        private void InitAllPlayers()
        {
            Debug.Log("Server init all players");
            for (int playerID = 0; playerID < playersCount; playerID++)
            {
                playersCommunicator.playerDataSender.InitPlayer(
                    playerID,
                    gameField.matrix,
                    catsCount
                );
            }
        }

        public void OnPlayerCatSelect(CatData cat)
        {
            cat = gameField.GetElementById(cat.id);
            Moves moves = moveChecker.GetPossibleMoves(cat);

            playersCommunicator.playerDataSender.SendPlayerPossibleMoves(currentPlayer, moves);
        }

        public void OnPlayerDisconnect(int playerID)
        {
            if (gameState == Enums.GameData.GameState.GameEnd)
            {
                return;
            }
            playersCommunicator.playerDataSender.playerListeners[playerID].active = false;
            Enums.GameData.EndGameReason reason = Enums.GameData.EndGameReason.Disconnect;
            int winnerID = playerID == 0 ? 1 : 0;
            GameResult gameResult = new GameResult(winnerID, (int)reason);
            OnGameEnd(gameResult);
        }

        public void OnPlayerMove(MoveData move)
        {
            move.catData = gameField.GetElementById(move.catData.id);
            if (!moveChecker.IsCorrectMove(move))
            {
                return;
            }
            MoveResult moveResult = moveMaker.MakeMove(move);
            playersCommunicator.playerDataSender.SendAllPlayerMove(moveResult);
            catsCount = moveResult.catsCount;
        }

        private bool CheckEndGame()
        {
            bool orangeWinsByClear = catsCount.blackCats == 0;
            bool blackWinsByClear = catsCount.orangeCats == 0;
            bool orangeWinsByStuck = moveChecker.CheckPlayerStuck(Enums.CatsType.Team.Black);
            bool blackWindByStuck = moveChecker.CheckPlayerStuck(Enums.CatsType.Team.Orange);
            bool gameEnd =
                orangeWinsByClear || blackWinsByClear || orangeWinsByStuck || blackWindByStuck;

            int winnerID = orangeWinsByClear || orangeWinsByStuck ? 0 : 1;
            Enums.GameData.EndGameReason reason =
                orangeWinsByClear || blackWinsByClear
                    ? Enums.GameData.EndGameReason.Clear
                    : Enums.GameData.EndGameReason.Stuck;
            if (gameEnd)
            {
                GameResult gameResult = new GameResult(winnerID, (int)reason);
                OnGameEnd(gameResult);
                return true;
            }
            return false;
        }

        private void OnGameEnd(GameResult gameResult)
        {
            gameState = Enums.GameData.GameState.GameEnd;
            currentPlayer = -1;
            playersCommunicator.playerDataSender.SendAllGameEnd(gameResult);
        }

        public void OnPlayerReady(MapHash playerHash, int playerID)
        {
            if (playerHash.maphash == gameField.mapHash)
            {
                playerReadyMarks[playerID] = true;
                Debug.Log($"player{playerID} is ready");
                if (CheckAllReady())
                {
                    OnAllPlayersReady();
                }
            }
            else
            {
                Debug.Log(
                    $"WRONG HASH\nserver: {gameField.mapHash}\nplayer[{playerID}]: {playerHash.maphash}"
                );
                playersCommunicator.playerDataSender.InitPlayer(
                    playerID,
                    gameField.matrix,
                    moveMaker.CountCats()
                );
            }
        }

        private void OnAllPlayersReady()
        {
            switch (gameState)
            {
                case Enums.GameData.GameState.GameStart:
                    Debug.Log("starting game");
                    StartGame();
                    break;
                case Enums.GameData.GameState.Game:
                    Debug.Log("continue game");
                    if (!CheckEndGame())
                    {
                        NextPlayerTurn();
                    }
                    break;
            }
        }

        private void NextPlayerTurn()
        {
            this.gameState = Enums.GameData.GameState.Game;
            currentPlayer++;
            playersCommunicator.playerDataSender.SendAllPlayersOrder(currentPlayer);
        }

        private bool CheckAllReady()
        {
            bool allReady = true;

            foreach (bool playerMark in playerReadyMarks)
            {
                allReady &= playerMark;
            }

            if (allReady)
            {
                ClearReadyMarks();
            }
            return allReady;
        }

        private void ClearReadyMarks()
        {
            playerReadyMarks = new bool[playersCount];
        }

        public bool IsCurrentPlayer(int playerID)
        {
            return playerID == currentPlayer;
        }
    }
}
