using System;
using DG.Tweening;
using PJTC.Managers;
using UnityEngine;

namespace PJTC.Managers.UI
{
    public class GameHintElement : MonoBehaviour
    {
        [SerializeField]
        private Transform hint;

        [SerializeField]
        private Transform helpBtn;

        private bool showHint = false;
        private const float ANIM_TIME = 0.3f;

        private void Start()
        {
            HideHelpBtn(0);
        }

        public void HitButtonPress()
        {
            showHint = !showHint;
            if (showHint)
            {
                ShowHint();
            }
            else
            {
                HideHint();
            }
        }

        public void HideHint(float time = ANIM_TIME)
        {
            hint.transform.DOScale(Vector2.zero, time).Play();
            helpBtn.DOScale(Vector2.one, time).Play();
        }

        private void ShowHint()
        {
            hint.transform.DOScale(Vector2.one, ANIM_TIME).Play();
            helpBtn.DOScale(new Vector2(0.7f, 0.7f), ANIM_TIME).Play();
        }

        private void OnOrderChanged(bool playerOrder)
        {
            if (playerOrder)
            {
                ShowHelpBtn();
            }
            else
            {
                showHint = false;
                HideHelpBtn();
            }
        }

        private void HideHelpBtn(float time = ANIM_TIME)
        {
            hint.transform.DOScale(Vector2.zero, time).Play();
            helpBtn.DOScale(Vector2.zero, time).Play();
        }

        private void ShowHelpBtn()
        {
            helpBtn.gameObject.SetActive(true);
            helpBtn.DOScale(Vector2.one, ANIM_TIME).Play();
        }

        private void SubOnEvents()
        {
            ServerDataHandler.MoverOrderChanging += OnOrderChanged;
        }

        private void UnsubFromEvents()
        {
            ServerDataHandler.MoverOrderChanging -= OnOrderChanged;
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
