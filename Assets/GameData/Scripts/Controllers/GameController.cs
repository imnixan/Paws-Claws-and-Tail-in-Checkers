using System;
using System.Collections.Generic;
using PCTC.Builders;
using PCTC.Game;
using PCTC.Handlers;
using UnityEngine;

namespace PCTC.Controllers
{
    public class GameController : MonoBehaviour
    {
        [SerializeField]
        private CarpetBuilder carpetBuilder;
        private GameField gameField;

        public void InitControllers(GameField gameField, List<ClickInputHandler> carpetHandlers)
        {
            this.gameField = gameField;
            foreach (var handler in carpetHandlers)
            {
                handler.ClickTargeted += OnCarpetClick;
            }
        }

        private void OnCarpetClick(ClickInputHandler carpetHandler)
        {
            Debug.Log(carpetHandler.name);
        }
    }
}
