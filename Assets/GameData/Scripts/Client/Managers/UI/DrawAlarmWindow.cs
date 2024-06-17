using DG.Tweening;
using GameData.Managers;
using UnityEngine;

namespace PJTC.Managers.UI
{
    public class DrawAlarmWindow : MonoBehaviour
    {
        [SerializeField]
        private Transform alarmWindow;

        private void Start()
        {
            alarmWindow.DOMoveX(-1000, 0);
            alarmWindow.gameObject.SetActive(false);
        }

        private void ShowAlarmWindow()
        {
            alarmWindow.gameObject.SetActive(true);
            alarmWindow.DOMoveX(0, 0.4f).SetEase(Ease.InBack);
        }

        public void HideAlarmWindow()
        {
            Sequence hideWindow = DOTween.Sequence();
            hideWindow
                .Append(alarmWindow.DOMoveX(-1000, 0.4f).SetEase(Ease.OutBack))
                .AppendCallback(() =>
                {
                    alarmWindow.gameObject.SetActive(false);
                })
                .Restart();
        }

        private void SubOnEvents()
        {
            ServerDataHandler.DrawAlarm += ShowAlarmWindow;
        }

        private void UnsubFromEvents()
        {
            ServerDataHandler.DrawAlarm -= ShowAlarmWindow;
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
