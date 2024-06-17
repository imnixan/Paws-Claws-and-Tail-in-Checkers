using System;
using DG.Tweening;
using GameData.Managers;
using PJTC.Builders;
using PJTC.Enums;
using PJTC.Game;
using PJTC.Structs;
using UnityEngine;

namespace PJTC.Managers.UI
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
        private RectTransform chooseAttackWindow;

        private void Start()
        {
            chooseAttackWindow.localPosition = new Vector2(
                -1000,
                chooseAttackWindow.localPosition.y
            );
            chooseAttackWindow.gameObject.SetActive(false);
            restartBtn.gameObject.SetActive(false);
        }

        public void OnConnect()
        {
            connectBtn.SetActive(false);
            restartBtn.SetActive(true);
        }

        private void OnPlayerInit(PlayerInitData playerInitData)
        {
            chooseAttackWindow.gameObject.SetActive(true);
            chooseAttackWindow.DOLocalMoveX(0, 0.3f).Play();
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

        private void OnDrawAlarm() { }

        private void OnGameEnd(GameResult gameResult)
        {
            restartBtn.SetActive(false);
        }

        private void SubOnEvents()
        {
            ServerDataHandler.GameStart += OnGameStart;
            ServerDataHandler.PlayerInit += OnPlayerInit;
            ServerDataHandler.GameEnd += OnGameEnd;
            ServerDataHandler.DrawAlarm += OnDrawAlarm;

            AttackChooseManager.PlayerFinishChoosingAttacks += OnPlayerFinishChooseAttack;
        }

        private void UnsubFromEvents()
        {
            ServerDataHandler.GameStart -= OnGameStart;
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
