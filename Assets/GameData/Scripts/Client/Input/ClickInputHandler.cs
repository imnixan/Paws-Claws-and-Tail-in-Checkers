using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;

namespace PJTC.Handlers
{
    public class ClickInputHandler : MonoBehaviour
    {
        public event UnityAction<ClickInputHandler> Click;
        public Vector2Int position;

        private void OnMouseDown()
        {
            this.Click?.Invoke(this);
        }
    }
}
