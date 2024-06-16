using System;
using DG.Tweening;
using GameData.Managers;
using PJTC.Enums;
using PJTC.Structs;
using TMPro;
using UnityEngine;

namespace PJTC.Managers.UI
{
    public class EndGameWindow : MonoBehaviour
    {
        [SerializeField]
        private Transform endGameField;

        [SerializeField]
        private TextMeshProUGUI endReasonText,
            winText;

        [SerializeField]
        private ClientGameManager gameManager;

        private void Start()
        {
            endGameField.gameObject.SetActive(false);
        }

        private void OnGameEnd(GameResult result)
        {
            bool win = gameManager.playerID == result.winnerID;
            Enums.GameData.EndGameReason reason = (Enums.GameData.EndGameReason)result.reason;
            endGameField.gameObject.SetActive(true);
            endGameField.DOLocalMoveX(0, 0.5f).Play();
            winText.text = win ? "You won!" : "You lose!";
            string gameEndReason = "";
            switch (reason)
            {
                case Enums.GameData.EndGameReason.Disconnect:
                    gameEndReason = "Disconnect";
                    break;
                case Enums.GameData.EndGameReason.GiveUp:
                    gameEndReason = "Give up";
                    break;
                case Enums.GameData.EndGameReason.Clear:
                    gameEndReason = "Defeated all the cats";
                    break;
                case Enums.GameData.EndGameReason.Stuck:
                    gameEndReason = "The cats have no moves left";
                    break;
            }
            endReasonText.text = gameEndReason;
        }

        private void SubOnEvents()
        {
            ServerDataHandler.GameEnd += OnGameEnd;
        }

        private void UnsubFromEvents()
        {
            ServerDataHandler.GameEnd -= OnGameEnd;
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
