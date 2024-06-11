using System;
using DG.Tweening;
using PCTC.Handlers;
using PCTC.Structs;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace PCTC.CatScripts
{
    [RequireComponent(typeof(ClickInputHandler))]
    [RequireComponent(typeof(MoveController))]
    public class Cat : MonoBehaviour
    {
        [SerializeField]
        private Material orangeCat,
            blackCat;
        public CatData catData;
        public event UnityAction<Cat> catTouched;
        public ClickInputHandler clickHandler { get; private set; }
        private MoveController moveController;

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
        }

        private void InitCat()
        {
            this.moveController = GetComponent<MoveController>();
            this.clickHandler = GetComponent<ClickInputHandler>();
            switch (catData.team)
            {
                case Enums.CatsType.Team.Black:
                    GetComponentInChildren<MeshRenderer>().material = blackCat;
                    break;
                case Enums.CatsType.Team.Orange:
                    GetComponentInChildren<MeshRenderer>().material = orangeCat;
                    break;
            }
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
        }

        private void MakeSubscribes()
        {
            this.moveController.destinationReached += this.OnDestinationReached;
            this.clickHandler.Click += this.OnClick;
        }

        private void MakeUnSubscrubs()
        {
            this.moveController.destinationReached -= this.OnDestinationReached;
            this.clickHandler.Click -= this.OnClick;
        }

        public Tween MoveTo(Vector2Int destination)
        {
            Vector3 position = new Vector3(destination.x, 0, destination.y);
            this.catData.position = destination;
            Tween tween = moveController.MoveTo(position);
            return tween;
        }

        private void OnDestroy()
        {
            MakeUnSubscrubs();
        }

        public void RemoveCat()
        {
            Destroy(gameObject);
        }

        public void UpgradeCat()
        {
            this.catData.type = Enums.CatsType.Type.Chonky;
            Debug.Log("cat upgraded");
        }
    }
}
