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

        [SerializeField]
        private GameObject waitingForThePlayer;

        [SerializeField]
        private GameObject loadingIcon;

        [Header("Hideable buttons")]
        [SerializeField]
        private GameObject connectBtn;

        [SerializeField]
        private GameObject restartBtn;

        private void Start()
        {
            restartBtn.gameObject.SetActive(false);
            waitingForThePlayer.SetActive(false);
        }

        public void OnStartConnect()
        {
            loadingIcon.SetActive(true);
            connectBtn.SetActive(false);
        }

        public void OnConnectError()
        {
            Debug.Log("ui connect Error");
            loadingIcon.SetActive(false);
            connectBtn.SetActive(true);
        }

        public void OnConnected()
        {
            restartBtn.SetActive(true);
            loadingIcon.SetActive(false);
            waitingForThePlayer.SetActive(true);
        }

        private void OnPlayerInit(PlayerInitData playerInitData)
        {
            waitingForThePlayer.SetActive(false);
        }

        private void OnGameStart()
        {
            waitingForThePlayer.SetActive(false);
        }

        private void OnPlayerFinishChooseAttack()
        {
            waitingForThePlayer.SetActive(true);
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
            AttackChooseManager.PlayerFinishChoosingAttacks += OnPlayerFinishChooseAttack;
        }

        private void UnsubFromEvents()
        {
            ServerDataHandler.GameStart -= OnGameStart;
            ServerDataHandler.MoverOrderChanging -= OnOrderChanged;
            ServerDataHandler.PlayerInit -= OnPlayerInit;
            ServerDataHandler.GameEnd -= OnGameEnd;
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
