using System.Collections.Generic;
using PCTC.Builders;
using PCTC.Controllers;
using PCTC.Game;
using PCTC.Handlers;
using UnityEngine;

namespace PCTC.Managers
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField]
        private int gameFieldSize;

        [SerializeField]
        private CarpetBuilder carpetBuilder;

        [SerializeField]
        private GameController gameController;

        void Start()
        {
            InitGame();
        }

        private void InitGame()
        {
            GameField gameField = new GameField(gameFieldSize);
            List<ClickInputHandler> carpetHandlers = carpetBuilder.BuildGameField(gameField);
            gameController.InitControllers(gameField, carpetHandlers);
        }
    }
}
