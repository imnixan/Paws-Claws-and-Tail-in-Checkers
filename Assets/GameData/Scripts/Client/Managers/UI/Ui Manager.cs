using DG.Tweening;
using GameData.Managers;
using PJTC.Structs;
using TMPro;
using UnityEngine;

namespace PJTC.Managers.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI turnStatus;

        [Header("Hideable buttons")]
        [SerializeField]
        private GameObject connectBtn;

        [SerializeField]
        private GameObject restartBtn;

        [Header("Other managers")]
        [SerializeField]
        private ScoreManager scoreManager;

        [SerializeField]
        private EndGameWindow gameEndManager;

        [Header("Windows")]
        [SerializeField]
        private Transform chooseAttackWindow;

        [SerializeField]
        private Transform alarmWindow;

        private void Start()
        {
            chooseAttackWindow.localPosition = new Vector2(
                -1000,
                chooseAttackWindow.localPosition.y
            );
            chooseAttackWindow.gameObject.SetActive(false);
            restartBtn.gameObject.SetActive(false);
            alarmWindow.gameObject.SetActive(false);
        }

        private void OnPlayerInit(PlayerInitData playerInitData)
        {
            connectBtn.SetActive(false);
            restartBtn.SetActive(true);
        }

        private void OnGameStart() { }

        private void OnPlayerFinishChooseAttack()
        {
            Sequence showAttackChooseWindow = DOTween.Sequence();
            showAttackChooseWindow
                .Append(chooseAttackWindow.DOLocalMoveX(-1000, 0.3f))
                .AppendCallback(() =>
                {
                    chooseAttackWindow.gameObject.SetActive(true);
                })
                .Restart();
        }

        private void OnDrawAlarm()
        {
            alarmWindow.gameObject.SetActive(true);
            alarmWindow.DOMoveX(-1000, 0);

            Sequence showAlarm = DOTween.Sequence();
            showAlarm
                .Append(alarmWindow.DOMoveX(0, 0.3f))
                .AppendInterval(1.5f)
                .Append(alarmWindow.DOMoveX(-1000, 0.15f))
                .AppendCallback(() =>
                {
                    alarmWindow.gameObject.SetActive(true);
                })
                .Restart();
        }

        private void OnGameEnd(GameResult gameResult)
        {
            restartBtn.SetActive(false);
        }

        private void OnOrderChanged(bool playerOrder)
        {
            if (playerOrder)
            {
                turnStatus.text = "Your turn";
                turnStatus.color = Color.green;
            }
            else
            {
                turnStatus.text = "Opponent's turn";
                turnStatus.color = Color.white;
            }
        }

        private void SubOnEvents()
        {
            ServerDataHandler.GameStart += OnGameStart;
            ServerDataHandler.MoverOrderChanging += OnOrderChanged;
            ServerDataHandler.PlayerInit += OnPlayerInit;
            ServerDataHandler.GameEnd += OnGameEnd;
            ServerDataHandler.DrawAlarm += OnDrawAlarm;
            AttackChooseManager.PlayerFinishChoosingAttacks += OnPlayerFinishChooseAttack;
        }

        private void UnsubFromEvents()
        {
            ServerDataHandler.GameStart -= OnGameStart;
            ServerDataHandler.MoverOrderChanging -= OnOrderChanged;
            ServerDataHandler.PlayerInit -= OnPlayerInit;
            ServerDataHandler.GameEnd -= OnGameEnd;
            ServerDataHandler.DrawAlarm += OnDrawAlarm;
            AttackChooseManager.PlayerFinishChoosingAttacks -= OnPlayerFinishChooseAttack;
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
