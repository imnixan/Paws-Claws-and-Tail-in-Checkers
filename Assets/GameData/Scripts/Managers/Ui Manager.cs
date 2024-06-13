using System;
using PJTC.Builders;
using PJTC.Enums;
using PJTC.Game;
using PJTC.Structs;
using UnityEngine;

namespace PJTC.Managers
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private GameObject connectBtn,
            restartBtn;

        [SerializeField]
        private ScoreManager scoreManager;

        [SerializeField]
        private EndGameWindow gameEndManager;

        [SerializeField]
        private UIFieldBuilder gameFieldBuilder;

        [SerializeField]
        private ClientGameManager gameManager;

        public void OnConnect()
        {
            connectBtn.SetActive(false);
        }

        public void ShowUIField(CatData[,] field)
        {
            gameFieldBuilder.BuildUIField(field, gameManager.playerID);
        }

        public void OnGameStart()
        {
            scoreManager.OnGameStart();
        }

        public void OnGameEnd(bool win, Enums.GameData.EndGameReason reason)
        {
            gameEndManager.OnGameEnd(win, reason);
            restartBtn.SetActive(false);
            scoreManager.OnGameEnd();
        }

        public void UpdateScores(CatsCount catsCount)
        {
            scoreManager.UpdateScores(catsCount);
        }
    }
}
