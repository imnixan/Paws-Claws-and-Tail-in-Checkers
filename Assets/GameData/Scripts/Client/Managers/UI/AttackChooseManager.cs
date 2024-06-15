using System;
using System.Collections.Generic;
using System.Xml.Schema;
using GameData.Managers;
using PJTC.Controllers;
using PJTC.Enums;
using PJTC.Structs;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PJTC.Managers
{
    public class AttackChooseManager : MonoBehaviour
    {
        public static event UnityAction PlayerFinishChoosingAttacks;
        public static event UnityAction<AttacksPool> RandomChoose;

        [SerializeField]
        private TextMeshProUGUI maxPawsText,
            maxJawsText,
            maxTailsText,
            currentPawsText,
            currentJawsText,
            currentTailsText;

        [SerializeField]
        private Button finishBtn;

        public static event UnityAction AttacksChoosed;

        private int _currentPaws,
            _currentJaws,
            _currentTails;

        private int currentPaws
        {
            get { return _currentPaws; }
            set
            {
                _currentPaws = Mathf.Max(0, value);
                currentPawsText.text = _currentPaws.ToString();
            }
        }

        private int currentJaws
        {
            get { return _currentJaws; }
            set
            {
                _currentJaws = Mathf.Max(0, value);
                currentJawsText.text = _currentJaws.ToString();
            }
        }

        private int currentTails
        {
            get { return _currentTails; }
            set
            {
                _currentTails = Mathf.Max(0, value);
                currentTailsText.text = _currentTails.ToString();
            }
        }

        private AttacksPool attacksPool;

        public void OnPlayerFinish()
        {
            PlayerFinishChoosingAttacks?.Invoke();
        }

        public void SetRandomAttacks()
        {
            RandomChoose?.Invoke(attacksPool);
        }

        private void OnPlayerInit(PlayerInitData initData)
        {
            attacksPool = initData.attacksPool;
            maxPawsText.text = attacksPool.maxPaws.ToString();
            maxJawsText.text = attacksPool.maxJaws.ToString();
            maxTailsText.text = attacksPool.maxTails.ToString();
            currentJaws = 0;
            currentPaws = 0;
            currentTails = 0;
            finishBtn.interactable = false;
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
            finishBtn.interactable = choosedCorrect;
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
