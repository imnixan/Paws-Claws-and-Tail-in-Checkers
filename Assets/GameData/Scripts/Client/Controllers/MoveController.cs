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

        public Tween MoveTo(Vector3 destination)
        {
            Debug.Log("move speed = " + speed);
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
