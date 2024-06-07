using System;
using System.Collections.Generic;
using GameData.Scripts;
using PCTC.Builders;
using PCTC.CatScripts;
using PCTC.Controllers;
using PCTC.Handlers;
using PCTC.Scripts;
using PCTC.Structs;
using Unity.VisualScripting;
using UnityEngine;

namespace PCTC.Managers
{
    public class ClientGameManager : MonoBehaviour
    {
        [SerializeField]
        private CarpetBuilder carpetBuilder;

        [SerializeField]
        private ServerCommunicator serverCommunicator;

        [SerializeField]
        private GameBuilder gameBuilder;

        [SerializeField]
        private GameController gameController;

        public int playerId { get; private set; }

        public void Update()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                serverCommunicator.ConnectToServer(this);
            }
        }

        public void InitPlayer(PlayerInitData playerInitData)
        {
            this.playerId = playerInitData.playerId;
            CatData[,] gameField = ArrayTransformer.Expand(playerInitData.gameField);
            List<Cat> cats = gameBuilder.PlaceCats(gameField);
            carpetBuilder.BuildGameField(gameField, playerInitData.playerId);
            gameController.Init(
                gameField,
                playerInitData.playerId,
                cats,
                carpetBuilder.GetHandlersMap(),
                this
            );
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
        }

        public void MakeMove(MoveData moveData)
        {
            serverCommunicator.serverDataSender.SendPlayerMove(moveData);
        }

        public void FinishMove()
        {
            serverCommunicator.serverDataSender.SendPlayerFinishedMove();
        }

        private void OnMoveError()
        {
            Debug.Log("NoMoves for this cat");
        }
    }
}
