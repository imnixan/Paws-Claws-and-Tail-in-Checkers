using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace PCTC.Controllers
{
    public abstract class MoveController : MonoBehaviour
    {
        public float speed;
        public event UnityAction destinationReached;
        protected Transform transform;

        protected void Start()
        {
            this.transform = this.gameObject.transform;
        }

        public virtual void MoveTo(Vector3 destination)
        {
            float moveTime = Vector3.Distance(transform.position, destination) / speed;
            Sequence moveTo = DOTween.Sequence();
            moveTo
                .Append(transform.DOMove(destination, moveTime))
                .AppendCallback(OnDestinationReached)
                .Restart();
        }

        protected virtual void OnDestinationReached()
        {
            destinationReached?.Invoke();
        }
    }
}
