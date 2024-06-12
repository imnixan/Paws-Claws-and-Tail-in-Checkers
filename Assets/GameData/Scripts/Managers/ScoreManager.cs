using System;
using PCTC.Structs;
using TMPro;
using UnityEngine;

namespace PCTC.Managers
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

        public void OnGameEnd()
        {
            scoresBoard.SetActive(false);
        }

        public void OnGameStart()
        {
            scoresBoard.SetActive(true);
        }

        public void UpdateScores(CatsCount catsCount)
        {
            orangeCountText.text = catsCount.orangeCats.ToString();
            blackCountText.text = catsCount.blackCats.ToString();
            orangeChokyCountText.text = catsCount.orangeChonkyCats.ToString();
            blackChonkyCountText.text = catsCount.blackChonkyCats.ToString();
        }
    }
}
