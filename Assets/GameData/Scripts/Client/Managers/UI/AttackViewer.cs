using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using PJTC.CatScripts;
using PJTC.Controllers;
using PJTC.Enums;
using UnityEngine;
using UnityEngine.UI;

namespace PJTC.Scripts
{
    public class AttackViewer : MonoBehaviour
    {
        [SerializeField]
        private Sprite jawIcon,
            pawIcon,
            tailIcon,
            noneIcon,
            backGround;

        private Transform attackBannerTransform;
        private Image attackIcon;
        float bias = -20f;

        public void SetAttackBanner(CatsType.Attack attackType)
        {
            if (attackBannerTransform == null)
            {
                CreateBanner();
            }

            switch (attackType)
            {
                case CatsType.Attack.Jaws:
                    attackIcon.sprite = jawIcon;
                    break;
                case CatsType.Attack.Paws:
                    attackIcon.sprite = pawIcon;
                    break;
                case CatsType.Attack.Tail:
                    attackIcon.sprite = tailIcon;
                    break;
                default:
                    attackIcon.sprite = noneIcon;
                    break;
            }
        }

        private void Update()
        {
            if (attackBannerTransform != null)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
                screenPosition.y += bias;
                attackBannerTransform.position = screenPosition;
            }
        }

        private void OnDestroy()
        {
            if (attackBannerTransform != null)
            {
                Destroy(attackBannerTransform.gameObject);
            }
        }

        private void CreateBanner()
        {
            Transform canvas = GameObject.FindGameObjectWithTag("AttackCanvas").transform;
            GameObject attackBanner = new GameObject("AttackBanner");
            attackBanner.AddComponent<Image>().sprite = backGround;
            attackBanner.GetComponent<RectTransform>().sizeDelta = new Vector2(40, 40);
            attackBannerTransform = attackBanner.transform;
            attackBannerTransform.transform.SetParent(canvas);
            attackBannerTransform.transform.localScale = Vector3.one;

            GameObject attackIconObject = new GameObject("AttackBanner");
            attackIcon = attackIconObject.AddComponent<Image>();
            attackIcon.GetComponent<RectTransform>().sizeDelta = new Vector2(30, 30);
            attackIcon.transform.SetParent(attackBannerTransform);
            attackIcon.transform.localScale = Vector3.one;
            attackIcon.transform.localPosition = Vector3.zero;
            if (GetComponentInParent<Cat>().catData.team != GameController.playerTeam)
            {
                bias *= -1;
            }
        }
    }
}
