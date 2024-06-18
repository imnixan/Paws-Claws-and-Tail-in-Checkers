using DG.Tweening;
using GameData.Managers;
using PJTC.Structs;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace PJTC.Managers.UI
{
    internal class AttackChooseWindow : MonoBehaviour
    {
        [Header("Counters Max")]
        [SerializeField]
        private TextMeshProUGUI maxPawsText;

        [SerializeField]
        private TextMeshProUGUI maxJawsText;

        [SerializeField]
        private TextMeshProUGUI maxTailsText;

        [Header("Counters Current")]
        [SerializeField]
        private TextMeshProUGUI currentPawsText;

        [SerializeField]
        private TextMeshProUGUI currentJawsText;

        [SerializeField]
        private TextMeshProUGUI currentTailsText;

        [Header("Changing Images")]
        [SerializeField]
        private Image pawCircle;

        [SerializeField]
        private Image jawCircle;

        [SerializeField]
        private Image tailCircle;

        [SerializeField]
        private Image windowBG;

        [Header("Buttons")]
        [SerializeField]
        private Button finishBtn;

        [Header("Sprites")]
        [SerializeField]
        [Tooltip("Backgrounds for player's team, Orange == 0, Black == 1")]
        private Sprite[] windowVariants;

        [SerializeField]
        private Sprite redCircle,
            greenCircle;

        private void Start()
        {
            windowBG.transform.DOMoveX(-1000, 0);
            windowBG.gameObject.SetActive(false);
        }

        public void SetMaxValues(int maxPaws, int maxJaws, int maxTails)
        {
            maxPawsText.text = maxPaws.ToString();
            maxJawsText.text = maxJaws.ToString();
            maxTailsText.text = maxTails.ToString();
            currentPawsText.text = "0";
            currentJawsText.text = "0";
            currentTailsText.text = "0";
        }

        public void UpdatePaws(int paws)
        {
            currentPawsText.text = paws.ToString();
            pawCircle.sprite = currentPawsText.text == maxPawsText.text ? greenCircle : redCircle;
        }

        public void UpdateJaws(int jaws)
        {
            currentJawsText.text = jaws.ToString();
            jawCircle.sprite = currentJawsText.text == maxJawsText.text ? greenCircle : redCircle;
        }

        public void UpdateTails(int tails)
        {
            currentTailsText.text = tails.ToString();
            tailCircle.sprite =
                currentTailsText.text == maxTailsText.text ? greenCircle : redCircle;
        }

        public void SetActiveFinishBtn(bool active)
        {
            finishBtn.interactable = active;
        }

        private void OnAttackChoosed()
        {
            Sequence hideWindow = DOTween.Sequence();
            hideWindow
                .Append(windowBG.transform.DOMoveX(-1000, 0.4f).SetEase(Ease.OutBack))
                .AppendCallback(() =>
                {
                    windowBG.gameObject.SetActive(false);
                })
                .Restart();
        }

        private void OnPlayerInit(PlayerInitData playerInitData)
        {
            Debug.Log("show attack on playerInit");
            windowBG.gameObject.SetActive(true);
            windowBG.sprite = windowVariants[playerInitData.playerID];
            windowBG.transform.DOLocalMoveX(0, 0.4f).SetEase(Ease.InBack).Play();
        }

        private void SubOnEvents()
        {
            ServerDataHandler.PlayerInit += OnPlayerInit;
            AttackChooseManager.AttacksChoosed += OnAttackChoosed;
        }

        private void UnsubFromEvents()
        {
            AttackChooseManager.AttacksChoosed -= OnAttackChoosed;
            ServerDataHandler.PlayerInit -= OnPlayerInit;
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
