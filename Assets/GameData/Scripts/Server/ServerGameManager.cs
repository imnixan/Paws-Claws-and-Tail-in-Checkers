using System;
using System.Collections.Generic;
using PJTC.Enums;
using PJTC.Game;
using PJTC.Server.MovesCalculation;
using PJTC.Structs;
using UnityEngine;

namespace PJTC.Server
{
    public class ServerGameManager
    {
        private const int REPEATS_MAX = 5;
        private const int REPEATS_ALARM = 3;

        public GameField gameField { get; private set; }
        public MoveMaker moveMaker { get; private set; }
        public MoveChecker moveChecker { get; private set; }

        private PlayersCommunicator playersCommunicator;
        private bool[] playerReadyMarks;
        private int _currentPlayer;
        private int playersCount;
        private CatsCount catsCount;
        private Guid roomNumber;
        private RepeatMovesChecker repeatMovesChecker;
        private Enums.GameData.GameState gameState;
        private AttacksPool attacksPool = new AttacksPool(4, 4, 4);

        private int currentPlayer
        {
            get { return _currentPlayer; }
            set
            {
                _currentPlayer = value >= playersCount ? 0 : value;
                Debug.Log($"current player {_currentPlayer}");
            }
        }

        public ServerGameManager(
            PlayersCommunicator playersCommunicator,
            int playersCount,
            Guid roomNumber
        )
        {
            this.playersCount = playersCount;
            this.roomNumber = roomNumber;
            playerReadyMarks = new bool[playersCount];
            gameField = new GameField();
            this.playersCommunicator = playersCommunicator;
            playersCommunicator.Init(this);
            moveChecker = new MoveChecker(this, gameField);
            moveMaker = new MoveMaker(gameField, moveChecker);
            catsCount = moveMaker.CountCats();
            InitAllPlayers();
        }

        public void OnPlayerCatSelect(CatData cat)
        {
            cat = gameField.GetElementById(cat.id);
            Moves moves = moveChecker.GetPossibleMoves(cat);

            playersCommunicator.playerDataSender.SendPlayerPossibleMoves(currentPlayer, moves);
        }

        public void OnPlayerDisconnect(int playerID)
        {
            playersCount--;
            Debug.Log("Players count " + playersCount);
            if (gameState == Enums.GameData.GameState.GameEnd)
            {
                if (playersCount == 0)
                {
                    RoomCreator.DestroyRoom(roomNumber);
                }
            }
            else
            {
                playersCommunicator.playerDataSender.playerListeners[playerID].active = false;
                Enums.GameData.EndGameReason reason = Enums.GameData.EndGameReason.Disconnect;
                int winnerID = playerID == 0 ? 1 : 0;
                GameResult gameResult = new GameResult(winnerID, (int)reason);
                OnGameEnd(gameResult);
                Debug.Log($"players count {playersCount}");
            }
        }

        public void OnPlayerMove(MoveData move)
        {
            move.catData = gameField.GetElementById(move.catData.id);
            if (!moveChecker.IsCorrectMove(move))
            {
                return;
            }
            Debug.Log($"build move to {move.moveEnd}");
            MoveResult moveResult = moveMaker.MakeMove(new CompletedMoveData(move));
            gameField.UpdateField(moveResult);
            catsCount = moveResult.catsCount;

            for (int i = 0; i < playersCount; i++)
            {
                MoveResult playerResult = gameField.CensureMoveResultForPlayer(
                    (CatsType.Team)i,
                    moveResult
                );
                Debug.Log($"SERVER GM ORIGINAL move RESULT {JsonUtility.ToJson(moveResult)}");

                playersCommunicator.playerDataSender.SendPlayerMove(playerResult, i);
            }

            int repeats = repeatMovesChecker.GetMoveRepeats(moveResult);
            if (repeats == REPEATS_ALARM) { }
            if (repeats >= REPEATS_MAX)
            {
                OnGameEnd(new GameResult(-1, (int)Enums.GameData.EndGameReason.Draw));
            }
        }

        public void OnPlayerSetAttack(PlayerAttackTypesData playerAttackTypesData, int playerID)
        {
            if (UpdateAttacks(playerAttackTypesData, playerID))
            {
                playerReadyMarks[playerID] = true;
                Debug.Log($"player{playerID} send attacks");

                if (CheckAllReady())
                {
                    OnAllPlayersReady();
                }
            }
            else
            {
                Debug.Log($"WRONG Attack data");
                playersCommunicator.playerDataSender.InitPlayer(
                    playerID,
                    gameField.GetSecureField((CatsType.Team)playerID),
                    moveMaker.CountCats(),
                    attacksPool,
                    CSMRequest.Type.PLAYER_INIT
                );
            }
        }

        public bool IsCurrentPlayer(int playerID)
        {
            return playerID == currentPlayer;
        }

        private void StartGame()
        {
            currentPlayer = 0;
            gameState = Enums.GameData.GameState.Game;
            playersCommunicator.playerDataSender.SendAllPlayersOrder(currentPlayer);
            playersCommunicator.playerDataSender.SendAllGameStart();
            repeatMovesChecker = new RepeatMovesChecker();
        }

        private void InitAllPlayers()
        {
            Debug.Log("Server init all players");
            gameState = Enums.GameData.GameState.PlayerInit;

            for (int playerID = 0; playerID < playersCount; playerID++)
            {
                playersCommunicator.playerDataSender.InitPlayer(
                    playerID,
                    gameField.GetSecureField((CatsType.Team)playerID),
                    catsCount,
                    attacksPool
                );
            }
        }

        private bool CheckEndGame()
        {
            bool orangeWinsByClear = catsCount.blackCats == 0 && catsCount.blackChonkyCats == 0;
            bool blackWinsByClear = catsCount.orangeCats == 0 && catsCount.orangeChonkyCats == 0;
            bool orangeWinsByStuck = moveChecker.IsPlayerStuck(Enums.CatsType.Team.Black);
            bool blackWindByStuck = moveChecker.IsPlayerStuck(Enums.CatsType.Team.Orange);
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
                    gameField.GetSecureField((CatsType.Team)playerID),
                    moveMaker.CountCats(),
                    attacksPool,
                    CSMRequest.Type.PLAYER_SYNC
                );
            }
        }

        private bool UpdateAttacks(PlayerAttackTypesData playerAttackTypesData, int playerID)
        {
            CatsType.Team playerTeam = (CatsType.Team)playerID;
            List<CatData> cats = new List<CatData>();

            foreach (var attackData in playerAttackTypesData.attackTypes)
            {
                CatData cat = gameField.GetElementById(attackData.catID);

                if (cat.team != playerTeam)
                {
                    return false;
                }

                cat.attackType = (CatsType.Attack)attackData.attackType;
                cats.Add(cat);
            }
            foreach (var cat in cats)
            {
                gameField.SetElement(cat);
            }

            return true;
        }

        private void OnAllPlayersReady()
        {
            switch (gameState)
            {
                case Enums.GameData.GameState.PlayerInit:
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
    }
}
