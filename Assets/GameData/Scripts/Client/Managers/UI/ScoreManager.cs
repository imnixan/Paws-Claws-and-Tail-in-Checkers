using System;
using GameData.Managers;
using PJTC.Structs;
using TMPro;
using UnityEngine;

namespace PJTC.Managers.UI
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI orangeCountText,
            orangeChokyCountText,
            blackCountText,
            blackChonkyCountText;

        [SerializeField]
        private GameObject scoresBoard;

        private void Start()
        {
            scoresBoard.SetActive(false);
        }

        private void OnGameEnd(GameResult result)
        {
            scoresBoard.SetActive(false);
        }

        private void OnGameStart()
        {
            scoresBoard.SetActive(true);
        }

        private void OnPlayerSync(PlayerInitData initData)
        {
            UpdateScores(initData.catsCount);
        }

        private void OnPlayerMove(MoveResult moveResult)
        {
            UpdateScores(moveResult.catsCount);
        }

        private void UpdateScores(CatsCount catsCount)
        {
            orangeCountText.text = catsCount.orangeCats.ToString();
            blackCountText.text = catsCount.blackCats.ToString();
            orangeChokyCountText.text = catsCount.orangeChonkyCats.ToString();
            blackChonkyCountText.text = catsCount.blackChonkyCats.ToString();
        }

        private void SubOnEvents()
        {
            ServerDataHandler.GameStart += OnGameStart;
            ServerDataHandler.GameEnd += OnGameEnd;
            ServerDataHandler.PlayerSync += OnPlayerSync;
            ServerDataHandler.Move += OnPlayerMove;
        }

        private void UnsubFromEvents()
        {
            ServerDataHandler.GameStart -= OnGameStart;
            ServerDataHandler.GameEnd -= OnGameEnd;
            ServerDataHandler.PlayerSync -= OnPlayerSync;
            ServerDataHandler.Move -= OnPlayerMove;
        }

        private void OnEnable()
        {
            SubOnEvents();
        }

        private void OnDisable()
        {
            UnsubFromEvents();
        }
    }
}
