using System;
using DG.Tweening;
using PJTC.Enums;
using PJTC.Handlers;
using PJTC.Scripts;
using PJTC.Structs;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PJTC.CatScripts
{
    [RequireComponent(typeof(ClickInputHandler))]
    [RequireComponent(typeof(MoveController))]
    public class Cat : MonoBehaviour
    {
        public CatData catData;
        public event UnityAction<Cat> catTouched;
        public ClickInputHandler clickHandler { get; private set; }
        private MoveController moveController;
        public VisualModel visualModel;
        private AttackViewer attackViewer;

        private GameObject image;

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
            visualModel = GetComponentInChildren<VisualModel>();
            attackViewer = GetComponentInChildren<AttackViewer>();
            attackViewer.SetAttackBanner(catData);
        }

        public void UpdateAttackType(CatData catData)
        {
            this.catData.attackType = catData.attackType;
            attackViewer.SetAttackBanner(catData);
        }

        public Material mat
        {
            get { return visualModel.GetMaterial(); }
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
        }

        private void MakeSubscribes()
        {
            this.clickHandler.Click += this.OnClick;
        }

        private void MakeUnSubscrubs()
        {
            this.clickHandler.Click -= this.OnClick;
        }

        public Tween MoveTo(Vector2Int destination)
        {
            Vector3 position = new Vector3(destination.x, 0, destination.y);
            catData.position = destination;
            Vector3 forward = position - transform.position;
            transform.forward = forward;
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
    }
}
