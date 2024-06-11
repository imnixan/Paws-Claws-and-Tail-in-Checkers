using System;
using System.Collections.Generic;
using DG.Tweening;
using PCTC.Builders;
using PCTC.CatScripts;
using PCTC.Game;
using PCTC.Handlers;
using PCTC.Managers;
using PCTC.Structs;
using UnityEngine;

namespace PCTC.Controllers
{
    public class GameController : MonoBehaviour
    {
        private GameField gameField;

        [SerializeField]
        private CarpetController carpetController;

        [SerializeField]
        private PlayerController playerController;

        public bool playerOrder;

        private CatData choosedCat;
        private ClientGameManager gameManager;

        public void Init(
            CatData[,] catData,
            int playerID,
            List<Cat> cats,
            ClickInputHandler[,] cellHandlers,
            ClientGameManager gameManager
        )
        {
            this.gameField = new GameField(catData);
            playerController.InitController(cats, playerID, this);
            this.carpetController.Init(cellHandlers, this);
            this.gameManager = gameManager;
            gameManager.OnReady(gameField.mapHash);
        }

        public void ShowPossibleMoves(Moves moves)
        {
            carpetController.ActiveCells(moves.possibleMoves);
        }

        public void OnCatClick(CatData cat)
        {
            if (playerOrder)
            {
                choosedCat = cat;
                gameManager.RequestPossibleMoves(cat);
                carpetController.ActiveCells(new Vector2Int[0]);
            }
        }

        public void OnCarpetClick(Vector2Int position)
        {
            if (choosedCat != null)
            {
                MoveData moveData = new MoveData(choosedCat, position);
                gameManager.MakeMove(moveData);
                carpetController.DeactivateCells();
                choosedCat = null;
            }
        }

        public void ProcessMove(MoveResult moveResult)
        {
            Sequence move = DOTween.Sequence();

            foreach (var moveData in moveResult.moves)
            {
                Cat cat = playerController.GetCat(moveData.catData);
                if (cat != null)
                {
                    move.Append(cat.MoveTo(moveData.moveEnd));
                }
            }
            move.AppendCallback(() =>
                {
                    RemoveCats(moveResult.catsForRemove);
                    UpgradeCats(moveResult.catsForUpgrade);
                    gameField.UpdateField(moveResult);
                    gameManager.OnReady(gameField.mapHash);
                })
                .Restart();
        }

        private void RemoveCats(CatData[] catsForRemove)
        {
            foreach (var catData in catsForRemove)
            {
                playerController.RemoveCat(catData);
            }
        }

        private void UpgradeCats(CatData[] catsForUpgrade)
        {
            foreach (var catData in catsForUpgrade)
            {
                playerController.UpgradeCat(catData);
            }
        }
    }
}
