using System;
using JetBrains.Annotations;
using PCTC.Game;
using PCTC.Server;
using PCTC.Structs;
using UnityEngine;

namespace PCTC.Server
{
    public class ServerGameManager
    {
        private PlayersCommunicator playersCommunicator;
        private MoveChecker moveChecker;
        private MoveMaker moveMaker;
        private bool firstPlayerMove;
        private int playersFinished = 0;
        private readonly object lockObject = new object();
        public GameField gameField { get; private set; }
        private int currentPlayerId
        {
            get { return firstPlayerMove ? 0 : 1; }
        }

        private int inactivePlayerId
        {
            get { return firstPlayerMove ? 1 : 0; }
        }

        public ServerGameManager(PlayersCommunicator playersCommunicator)
        {
            gameField = new GameField();
            this.playersCommunicator = playersCommunicator;
            playersCommunicator.Init(this);
            moveChecker = new MoveChecker(gameField);
            moveMaker = new MoveMaker(gameField, moveChecker);
            InitPlayers();
            StartGame();
        }

        private void StartGame()
        {
            firstPlayerMove = true;
            playersCommunicator.playerDataSender.ChangePlayerMoveOrder(firstPlayerMove);
        }

        private void InitPlayers()
        {
            Debug.Log("Server init players");
            playersCommunicator.playerDataSender.InitUsers(gameField.matrix);
        }

        public void OnPlayerCatSelect(CatData cat)
        {
            cat = gameField.GetElementById(cat.id);
            Moves moves = moveChecker.GetPossibleMoves(cat);
            playersCommunicator.playerDataSender.SendPlayerPossibleMoves(currentPlayerId, moves);
        }

        public void OnPlayerMove(MoveData move)
        {
            move.catData = gameField.GetElementById(move.catData.id);
            if (moveChecker.IsCorrectMove(move))
            {
                MoveResult moveResult = moveMaker.MakeMove(move);
                playersCommunicator.playerDataSender.SendPlayerMove(moveResult);
            }
        }

        public void OnPlayerMoveFinish(int playerId)
        {
            Debug.Log("player finished " + playerId);
            playersFinished++;
            if (playersFinished >= 2)
            {
                playersFinished = 0;
                firstPlayerMove = !firstPlayerMove;
                playersCommunicator.playerDataSender.ChangePlayerMoveOrder(firstPlayerMove);
            }
        }

        public bool IsCurrentPlayer(int playerId)
        {
            return playerId == currentPlayerId;
        }
    }
}
