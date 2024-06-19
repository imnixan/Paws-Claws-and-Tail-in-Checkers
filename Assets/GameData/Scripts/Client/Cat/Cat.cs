using DG.Tweening;
using PJTC.Handlers;
using PJTC.Managers;
using PJTC.Managers.UI;
using PJTC.Structs;
using UnityEngine;
using UnityEngine.Events;

namespace PJTC.CatScripts
{
    [RequireComponent(typeof(ClickInputHandler))]
    [RequireComponent(typeof(MoveController))]
    public class Cat : MonoBehaviour
    {
        [SerializeField]
        private float chonkySpeed = 1.5f;

        [SerializeField]
        private float catSpeed = 3.5f;
        public event UnityAction<Cat> catTouched;

        public CatData catData;
        public VisualModel visualModel;
        public ClickInputHandler clickHandler { get; private set; }

        private MoveController moveController;
        private AttackViewer attackViewer;

        public Material mat
        {
            get { return visualModel.GetMaterial(); }
        }

        public void Init(CatData catData)
        {
            this.catData = catData;
            InitCat();
            MakeSubscribes();

            transform.forward = new Vector3(
                catData.team == Enums.CatsType.Team.Orange ? 1 : -1,
                0,
                0
            );

            attackViewer = GetComponentInChildren<AttackViewer>();
            attackViewer.SetAttackBanner(catData);
            moveController.speed =
                catData.type == Enums.CatsType.Type.Normal ? catSpeed : chonkySpeed;
        }

        public void UpdateAttackType(CatData catData)
        {
            this.catData.attackType = catData.attackType;
            attackViewer.SetAttackBanner(catData);
        }

        public Tween MoveTo(Vector2Int destination)
        {
            Vector3 position = new Vector3(destination.x, 0, destination.y);
            catData.position = destination;
            Vector3 forward = position - transform.position;
            transform.forward = forward;

            Tween tween = moveController.MoveTo(position);
            tween.OnStart(OnMoveStart).OnComplete(OnMoveEnd);
            return tween;
        }

        public void OnBattle(bool attacker, bool result, Material attackMat = null)
        {
            if (result)
            {
                if (attacker)
                {
                    visualModel.PlayHitEffect(attackMat);
                }
                else
                {
                    visualModel.PlayDefendEffect();
                }
            }
        }

        private void OnMoveStart()
        {
            visualModel.StartMoving();
        }

        private void OnMoveEnd()
        {
            visualModel.StopMoving();
        }

        public void OnCatUpgrade()
        {
            visualModel.PlayUpgradeEffect();
            moveController.speed = chonkySpeed;
        }

        public void RemoveCat()
        {
            Destroy(gameObject);
        }

        private void InitCat()
        {
            this.moveController = GetComponent<MoveController>();
            this.clickHandler = GetComponent<ClickInputHandler>();
            this.clickHandler.position = catData.position;
        }

        private void OnDestinationReached() { }

        private void OnClick(ClickInputHandler inputHandler)
        {
            this.catTouched?.Invoke(this);

            Sequence catJump = DOTween.Sequence();
            catJump
                .Append(transform.DOMoveY(0.5f, 0.15f))
                .Append(transform.DOMoveY(0, 0.1f))
                .Restart();

            visualModel.OnInteract();
        }

        private void MakeSubscribes()
        {
            this.clickHandler.Click += this.OnClick;
        }

        private void MakeUnSubscrubs()
        {
            this.clickHandler.Click -= this.OnClick;
        }

        private void OnDestroy()
        {
            MakeUnSubscrubs();
        }
    }
}
