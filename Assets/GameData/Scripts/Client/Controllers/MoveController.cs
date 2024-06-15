using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace PJTC.Controllers
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

        public Tween MoveTo(Vector3 destination)
        {
            Tween tween = this.transform.DOMove(
                destination,
                Vector3.Distance(transform.position, destination) / speed
            );
            return tween;
        }

        protected virtual void OnDestinationReached()
        {
            destinationReached?.Invoke();
        }
    }
}
