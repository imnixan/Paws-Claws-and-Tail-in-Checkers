using PJTC.Controllers;
using PJTC.Managers;
using PJTC.Structs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PJTC.Managers.UI
{
    public class ScoreManager : MonoBehaviour
    {
        [SerializeField]
        private Image scoresBoard;

        [SerializeField]
        [Tooltip("Backgrounds for player's team, Orange == 0, Black == 1")]
        private Sprite[] windowVariants;

        [Header("Counters")]
        [SerializeField]
        private TextMeshProUGUI orangeCountText;

        [SerializeField]
        private TextMeshProUGUI orangeChokyCountText;

        [SerializeField]
        private TextMeshProUGUI blackCountText;

        [SerializeField]
        private TextMeshProUGUI blackChonkyCountText;

        private void Start()
        {
            scoresBoard.gameObject.SetActive(false);
        }

        private void OnGameEnd(GameResult result)
        {
            scoresBoard.gameObject.SetActive(false);
        }

        private void OnGameStart()
        {
            scoresBoard.gameObject.SetActive(true);
            scoresBoard.sprite = windowVariants[(int)GameController.playerTeam];
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
