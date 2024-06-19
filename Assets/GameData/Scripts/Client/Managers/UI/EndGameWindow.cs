using DG.Tweening;
using PJTC.Controllers;
using PJTC.Managers;
using PJTC.Structs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PJTC.Managers.UI
{
    public class EndGameWindow : MonoBehaviour
    {
        [SerializeField]
        private Image endGameField;

        [SerializeField]
        private Image header;

        [SerializeField]
        private ClientGameManager gameManager;

        [SerializeField]
        private TextMeshProUGUI endReasonText;

        [SerializeField]
        private Sprite[] windowVariants;

        [SerializeField]
        private Sprite winHeader;

        [SerializeField]
        private Sprite drawHeader;

        [SerializeField]
        private Sprite loseHeader;

        private void Start()
        {
            endGameField.gameObject.SetActive(false);
        }

        private void OnGameEnd(GameResult result)
        {
            endGameField.sprite = windowVariants[(int)GameController.playerTeam];

            bool win = gameManager.playerID == result.winnerID;
            Enums.GameData.EndGameReason reason = (Enums.GameData.EndGameReason)result.reason;
            endGameField.gameObject.SetActive(true);
            header.sprite = win ? winHeader : loseHeader;
            string gameEndReason = "";

            switch (reason)
            {
                case Enums.GameData.EndGameReason.Disconnect:
                    gameEndReason = "Opponent left the game";
                    break;
                case Enums.GameData.EndGameReason.Clear:
                    gameEndReason = "Defeated all the cats";
                    break;
                case Enums.GameData.EndGameReason.Stuck:
                    gameEndReason = "The cats have no moves left";
                    break;
                case Enums.GameData.EndGameReason.Draw:
                    gameEndReason = "No one could achieve victory!";
                    header.sprite = drawHeader;
                    break;
            }

            endReasonText.text = gameEndReason;

            endGameField.transform.DOLocalMoveX(0, 0.5f).Play();
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
