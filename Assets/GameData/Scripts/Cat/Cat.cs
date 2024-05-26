using System;
using PCTC.Handlers;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

namespace PCTC.Cat
{
    [RequireComponent(typeof(ClickInputHandler))]
    [RequireComponent(typeof(MoveController))]
    public class Cat : MonoBehaviour
    {
        public event UnityAction<Cat> catTouched;
        private ClickInputHandler clickHandler;
        private MoveController moveController;

        public void Init()
        {
            MakeSubscribes();
        }

        private void OnDestinationReached() { }

        private void OnClick()
        {
            this.catTouched?.Invoke(this);
        }

        private void MakeSubscribes()
        {
            this.moveController.destinationReached += this.OnDestinationReached;
            this.clickHandler.Click += this.OnClick;
        }

        public void MoveTo(Vector2 destination)
        {
            this.moveController.MoveTo(destination);
        }
    }
}
