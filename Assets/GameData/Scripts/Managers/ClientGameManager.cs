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

        private ServerCommunicator serverCommunicator;

        [SerializeField]
        private GameBuilder gameBuilder;

        [SerializeField]
        private GameController gameController;

        [SerializeField]
        private CameraPositioner cameraPositioner;

        [SerializeField]
        private UIManager uiManager;
        public int playerID { get; private set; }
        private bool gameEnded = false;

        public void Connect()
        {
            serverCommunicator = new ServerCommunicator();
            serverCommunicator.ConnectToServer(this);
        }

        public void OnConnect()
        {
            uiManager.OnConnect();
        }

        public void RestartGame()
        {
            RestartScene();
        }

        private void RestartScene()
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
            bool win = playerID == gameResult.winnerID;
            uiManager.OnGameEnd(win, (GameEnd.EndGameReason)gameResult.reason);
            gameEnded = true;
            serverCommunicator.Disconnect();
            gameEnded = true;
        }

        public void OnGameStart()
        {
            cameraPositioner.PosCamera(playerID);
            uiManager.OnGameStart();
        }

        public void ChangePlayerOrder(bool playerOrder)
        {
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

        internal void OnServerCloseConnect(object sender, CloseEventArgs e)
        {
            if (!gameEnded)
            {
                RestartGame();
            }
        }
    }
}
