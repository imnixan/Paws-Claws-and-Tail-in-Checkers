using System;
using System.Collections.Generic;
using PJTC.Handlers;
using UnityEngine;

namespace PJTC.Controllers
{
    public class CarpetController : MonoBehaviour
    {
        private ClickInputHandler[,] cellHandlers;
        private Vector2Int[] activeCoords;
        private GameController gameController;

        public void Init(ClickInputHandler[,] cellHandlers, GameController gameController)
        {
            this.cellHandlers = cellHandlers;
            this.gameController = gameController;
        }

        public void ActiveCells(Vector2Int[] coords)
        {
            if (activeCoords != null)
            {
                DeactivateCells();
            }
            this.activeCoords = coords;
            foreach (var coord in this.activeCoords)
            {
                cellHandlers[coord.x, coord.y].Click += OnCarpetClick;
                cellHandlers[coord.x, coord.y].GetComponent<Carpet>().ActivateCell();
            }
        }

        public void DeactivateCells()
        {
            foreach (var coord in this.activeCoords)
            {
                cellHandlers[coord.x, coord.y].Click -= OnCarpetClick;
                cellHandlers[coord.x, coord.y].GetComponent<Carpet>().DeactivateCell();
            }
            activeCoords = null;
        }

        private void OnCarpetClick(ClickInputHandler carpetCell)
        {
            gameController.OnCarpetClick(carpetCell.position);
        }
    }
}
