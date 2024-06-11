using System;
using PCTC.Enums;
using PCTC.Structs;
using UnityEngine;

namespace PCTC.Managers
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

        public void OnConnect()
        {
            connectBtn.SetActive(false);
        }

        public void OnGameStart()
        {
            scoreManager.OnGameStart();
        }

        public void OnGameEnd(bool win, GameEnd.EndGameReason reason)
        {
            gameEndManager.OnGameEnd(win, reason);
            scoreManager.OnGameEnd();
        }

        public void UpdateScores(CatsCount catsCount)
        {
            scoreManager.UpdateScores(catsCount);
        }
    }
}
