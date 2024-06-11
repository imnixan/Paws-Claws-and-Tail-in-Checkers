using System;
using System.Collections.Generic;
using System.Security.Policy;
using GameData.Scripts;
using PCTC.Builders;
using PCTC.CameraControl;
using PCTC.CatScripts;
using PCTC.Controllers;
using PCTC.Enums;
using PCTC.Handlers;
using PCTC.Scripts;
using PCTC.Structs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using WebSocketSharp;

namespace PCTC.Managers
{
    public class ClientGameManager : MonoBehaviour
    {
        [SerializeField]
        private CarpetBuilder carpetBuilder;

        public ServerCommunicator serverCommunicator { get; private set; }

        [SerializeField]
        private GameBuilder gameBuilder;

        [SerializeField]
        private GameController gameController;

        [SerializeField]
        private CameraPositioner cameraPositioner;

        [SerializeField]
        private UIManager uiManager;
        public int playerID { get; private set; }

        private Enums.GameData.GameState gameState;

        public void Start()
        {
            serverCommunicator = new ServerCommunicator(this);
        }

        public void Connect()
        {
            serverCommunicator.ConnectToServer();
        }

        public void OnConnect()
        {
            uiManager.OnConnect();
        }

        public void RestartGame()
        {
            //RestartScene();
            serverCommunicator.Disconnect();
        }

        public void RestartScene()
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        public void InitPlayer(PlayerInitData playerInitData)
        {
            this.playerID = playerInitData.playerID;
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

        public void OnGameEnd(GameResult gameResult)
        {
            gameState = Enums.GameData.GameState.GameEnd;
            bool win = playerID == gameResult.winnerID;
            uiManager.OnGameEnd(win, (Enums.GameData.EndGameReason)gameResult.reason);
            serverCommunicator.Disconnect();
        }

        public void OnGameStart()
        {
            gameState = Enums.GameData.GameState.GameStart;
            cameraPositioner.PosCamera(playerID);
            uiManager.OnGameStart();
        }

        public void ChangePlayerOrder(bool playerOrder)
        {
            gameState = Enums.GameData.GameState.Game;
            this.gameController.playerOrder = playerOrder;
        }

        public void RequestPossibleMoves(CatData catData)
        {
            this.serverCommunicator.serverDataSender.SendPlayerChooseCat(catData);
        }

        public void ProcessPossibleMoves(Moves moves)
        {
            if (moves.possibleMoves.Length == 0)
            {
                OnMoveError();
            }
            gameController.ShowPossibleMoves(moves);
        }

        public void ProcessMoveResult(MoveResult moveResult)
        {
            if (moveResult.moves.Length == 0)
            {
                OnMoveError();
            }
            else
            {
                gameController.ProcessMove(moveResult);
            }
            uiManager.UpdateScores(moveResult.catsCount);
        }

        public void MakeMove(MoveData moveData)
        {
            serverCommunicator.serverDataSender.SendPlayerMove(moveData);
        }

        public void OnReady(string mapHash)
        {
            serverCommunicator.serverDataSender.SendPlayerReady(mapHash);
        }

        private void OnMoveError()
        {
            Debug.Log("NoMoves for this cat");
        }

        public void OnServerEndConnection()
        {
            switch (gameState)
            {
                case Enums.GameData.GameState.GameEnd:

                    break;
                case Enums.GameData.GameState.Game:
                    RestartScene();
                    break;
                default:
                    RestartScene();
                    break;
            }
        }
    }
}
