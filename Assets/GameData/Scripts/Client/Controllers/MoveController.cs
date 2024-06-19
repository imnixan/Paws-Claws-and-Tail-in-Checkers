using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace PJTC.Controllers
{
    public abstract class MoveController : MonoBehaviour
    {
        public event UnityAction destinationReached;
        public float speed;

        protected Transform transform;

        protected void Start()
        {
            this.transform = this.gameObject.transform;
        }

        public Tween Move(Vector3 moveStart, Vector3 destination)
        {
            Tween tween = this.transform.DOMove(
                destination,
                Vector3.Distance(moveStart, destination) / speed
            );

            return tween;
        }

        protected virtual void OnDestinationReached()
        {
            destinationReached?.Invoke();
        }
    }
}
