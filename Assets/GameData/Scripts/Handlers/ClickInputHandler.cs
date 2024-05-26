using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace PCTC.Handlers
{
    public class ClickInputHandler : MonoBehaviour
    {
        public event UnityAction Click;
        public event UnityAction<ClickInputHandler> ClickTargeted;

        private void OnMouseDown()
        {
            this.Click?.Invoke();
            this.ClickTargeted?.Invoke(this);
        }
    }
}
