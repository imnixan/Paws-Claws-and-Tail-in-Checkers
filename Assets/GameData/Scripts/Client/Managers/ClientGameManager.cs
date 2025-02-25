﻿using System;
using System.Collections.Generic;
using PJTC.Builders;
using PJTC.CameraControl;
using PJTC.CatScripts;
using PJTC.Controllers;
using PJTC.Enums;
using PJTC.Managers;
using PJTC.Managers.UI;
using PJTC.Scripts;
using PJTC.Structs;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace PJTC.Managers
{
    public class ClientGameManager : MonoBehaviour
    {
        public static event UnityAction<Enums.GameData.GameState> GameStateChanged;

        public ServerCommunicator serverCommunicator { get; private set; }

        [SerializeField]
        private CarpetBuilder carpetBuilder;

        [SerializeField]
        private GameBuilder gameBuilder;

        [SerializeField]
        private GameController gameController;

        [SerializeField]
        private CameraPositioner cameraPositioner;

        [SerializeField]
        private UIManager uiManager;

        private int _playerID;

        private Enums.GameData.GameState _gameState;
        private Enums.GameData.GameState gameState
        {
            get { return _gameState; }
            set
            {
                _gameState = value;
                GameStateChanged?.Invoke(value);
            }
        }

        public int playerID
        {
            get { return _playerID; }
            private set { _playerID = value; }
        }

        public void Start()
        {
            serverCommunicator = new ServerCommunicator(this);
        }

        public void Connect()
        {
            serverCommunicator.TryToConnect();
            uiManager.OnStartConnect();
        }

        public void OnConnect()
        {
            uiManager.OnConnected();
        }

        public void RestartGame()
        {
            serverCommunicator.Disconnect();
            RestartScene();
        }

        public void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void OnServerEndConnection()
        {
            switch (gameState)
            {
                case Enums.GameData.GameState.GameEnd:

                    break;
                default:
                    RestartScene();
                    break;
            }
        }

        public void MakeMove(MoveData moveData)
        {
            serverCommunicator.serverDataSender.SendPlayerMove(moveData);
        }

        public void OnReady(string mapHash)
        {
            serverCommunicator.serverDataSender.SendPlayerReady(mapHash);
        }

        public void RequestPossibleMoves(CatData catData)
        {
            this.serverCommunicator.serverDataSender.SendPlayerChooseCat(catData);
        }

        public void SendPlayerAttackTypes(PlayerAttackTypesData playerAttackTypesData)
        {
            this.serverCommunicator.serverDataSender.SendPlayerAttack(playerAttackTypesData);
            gameState = Enums.GameData.GameState.WaitingMatchStart;
        }

        public void OnError() { }

        public void OnConnectError()
        {
            uiManager.OnConnectError();
        }

        private void OnPlayerInit(PlayerInitData playerInitData)
        {
            gameState = Enums.GameData.GameState.PlayerInit;

            OnPlayerSync(playerInitData);

            cameraPositioner.PosCamera(playerID);
        }

        private void OnPlayerSync(PlayerInitData playerInitData)
        {
            this.playerID = playerInitData.playerID;
            GameController.playerTeam = (CatsType.Team)playerInitData.playerID;
            CatData[,] gameField = ArrayTransformer.Expand(playerInitData.gameField);
            List<Cat> cats = gameBuilder.PlaceCats(gameField);
            carpetBuilder.BuildGameField(gameField);
            gameController.Init(
                gameField,
                playerInitData.playerID,
                cats,
                carpetBuilder.GetHandlersMap(),
                this
            );
        }

        private void OnGameEnd(GameResult gameResult)
        {
            gameState = Enums.GameData.GameState.GameEnd;
            serverCommunicator.Disconnect();
        }

        private void OnGameStart()
        {
            gameState = Enums.GameData.GameState.Game;
        }

        private void ChangePlayerOrder(bool playerOrder)
        {
            this.gameController.playerOrder = playerOrder;
        }

        private void OnMoveError() { }

        private void SubOnEvents()
        {
            ServerDataHandler.GameStart += OnGameStart;
            ServerDataHandler.PlayerInit += OnPlayerInit;
            ServerDataHandler.MoverOrderChanging += ChangePlayerOrder;
            ServerDataHandler.GameEnd += OnGameEnd;
            ServerDataHandler.PlayerSync += OnPlayerSync;
        }

        private void UnsubFromEvents()
        {
            ServerDataHandler.GameStart -= OnGameStart;
            ServerDataHandler.PlayerInit -= OnPlayerInit;
            ServerDataHandler.MoverOrderChanging -= ChangePlayerOrder;
            ServerDataHandler.GameEnd -= OnGameEnd;
            ServerDataHandler.PlayerSync -= OnPlayerSync;
        }

        private void OnEnable()
        {
            SubOnEvents();
        }

        private void OnDisable()
        {
            UnsubFromEvents();
            serverCommunicator.Disconnect();
        }
    }
}
