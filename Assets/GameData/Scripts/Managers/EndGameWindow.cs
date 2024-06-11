using System;
using DG.Tweening;
using PCTC.Enums;
using PCTC.Structs;
using TMPro;
using UnityEngine;

namespace PCTC.Managers
{
    public class EndGameWindow : MonoBehaviour
    {
        [SerializeField]
        private Transform endGameField;

        [SerializeField]
        private TextMeshProUGUI endReasonText,
            winText;

        private void Start()
        {
            endGameField.gameObject.SetActive(false);
        }

        public void OnGameEnd(bool win, Enums.GameData.EndGameReason reason)
        {
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
    }
}
