﻿using PJTC.Controllers;
using PJTC.Enums;
using PJTC.Managers;
using PJTC.Managers.UI;
using PJTC.Structs;
using UnityEngine;
using UnityEngine.Events;

namespace PJTC.Managers
{
    public class AttackChooseManager : MonoBehaviour
    {
        public static event UnityAction PlayerFinishChoosingAttacks;
        public static event UnityAction<AttacksPool> RandomChoose;

        [SerializeField]
        private AttackChooseWindow window;

        private int _currentPaws,
            _currentJaws,
            _currentTails;

        public void OnPlayerFinish()
        {
            PlayerFinishChoosingAttacks?.Invoke();
        }

        public void SetRandomAttacks()
        {
            RandomChoose?.Invoke(attacksPool);
        }

        private int currentPaws
        {
            get { return _currentPaws; }
            set
            {
                _currentPaws = Mathf.Max(0, value);
                Debug.Log("paws " + _currentPaws);
                window.UpdatePaws(_currentPaws);
            }
        }

        private int currentJaws
        {
            get { return _currentJaws; }
            set
            {
                _currentJaws = Mathf.Max(0, value);
                Debug.Log("jaws " + _currentJaws);
                window.UpdateJaws(_currentJaws);
            }
        }

        private int currentTails
        {
            get { return _currentTails; }
            set
            {
                _currentTails = Mathf.Max(0, value);
                Debug.Log("tails " + _currentTails);
                window.UpdateTails(_currentTails);
            }
        }

        private AttacksPool attacksPool;

        private void OnPlayerInit(PlayerInitData initData)
        {
            attacksPool = initData.attacksPool;
            window.SetMaxValues(attacksPool.maxPaws, attacksPool.maxJaws, attacksPool.maxTails);
            currentJaws = 0;
            currentPaws = 0;
            currentTails = 0;
            window.SetActiveFinishBtn(false);
        }

        private void OnAttackChanged(CatData catData, CatsType.Attack oldAttack)
        {
            switch (oldAttack)
            {
                case CatsType.Attack.Paws:
                    currentPaws--;
                    break;
                case CatsType.Attack.Jaws:
                    currentJaws--;
                    break;
                case CatsType.Attack.Tail:
                    currentTails--;
                    break;
            }
            switch (catData.attackType)
            {
                case CatsType.Attack.Paws:
                    currentPaws++;
                    break;
                case CatsType.Attack.Jaws:
                    currentJaws++;
                    break;
                case CatsType.Attack.Tail:
                    currentTails++;
                    break;
            }

            bool choosedCorrect =
                currentPaws == attacksPool.maxPaws
                && currentJaws == attacksPool.maxJaws
                && currentTails == attacksPool.maxTails;

            window.SetActiveFinishBtn(choosedCorrect);
        }

        private void SubOnEvents()
        {
            ServerDataHandler.PlayerInit += OnPlayerInit;
            GameController.AttackChanged += OnAttackChanged;
        }

        private void UnsubFromEvents()
        {
            ServerDataHandler.PlayerInit -= OnPlayerInit;
            GameController.AttackChanged -= OnAttackChanged;
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
