using System;
using System.Collections.Generic;
using System.Xml.Schema;
using JetBrains.Annotations;
using PJTC.CatScripts;
using PJTC.Controllers;
using PJTC.Enums;
using PJTC.Structs;
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
            crossIcon,
            backGround;

        private Transform attackBanner;
        private Image attackIcon;
        private GameObject cross;
        float bias = -20f;

        private Sprite currentIcon
        {
            get { return attackIcon.sprite; }
            set
            {
                attackIcon.sprite = value;
                if (value == null)
                {
                    attackIcon.color = new Color(1, 1, 1, 0);
                }
                else
                {
                    attackIcon.color = Color.white;
                }
            }
        }

        public void SetAttackBanner(CatData catData)
        {
            if (attackBanner == null)
            {
                CreateBanner();
            }

            if (catData.attackType == CatsType.Attack.None)
            {
                cross.SetActive(catData.attackHints.excludedAttack != CatsType.Attack.None);
                SetBannerIcon(catData.attackHints.excludedAttack);
            }
            else
            {
                cross.SetActive(false);
                SetBannerIcon(catData.attackType);
            }
        }

        private void SetBannerIcon(CatsType.Attack attackType)
        {
            switch (attackType)
            {
                case CatsType.Attack.Jaws:
                    currentIcon = jawIcon;

                    break;
                case CatsType.Attack.Paws:
                    currentIcon = pawIcon;
                    break;
                case CatsType.Attack.Tail:
                    currentIcon = tailIcon;
                    break;
                default:
                    currentIcon = null;

                    break;
            }
        }

        private void Update()
        {
            if (attackBanner != null)
            {
                Vector3 screenPosition = Camera.main.WorldToScreenPoint(transform.position);
                screenPosition.y += bias;
                attackBanner.position = screenPosition;
            }
        }

        private void OnDestroy()
        {
            if (attackBanner != null)
            {
                Destroy(attackBanner.gameObject);
            }
        }

        private void CreateBanner()
        {
            Transform canvas = GameObject.FindGameObjectWithTag("AttackCanvas").transform;

            Image attackBannerImage = CreateImageObject(
                "AttackBanner",
                new Vector2(40, 40),
                canvas
            );
            attackBannerImage.sprite = backGround;
            attackBanner = attackBannerImage.transform;

            attackIcon = CreateImageObject("AttackIcon", new Vector2(30, 30), attackBanner);

            if (GetComponentInParent<Cat>().catData.team != GameController.playerTeam)
            {
                bias *= -1;
            }

            Image crossImage = CreateImageObject("AttackIcon", new Vector2(40, 40), attackBanner);
            crossImage.sprite = crossIcon;
            cross = crossImage.gameObject;
        }

        private Image CreateImageObject(string name, Vector2 size, Transform parent)
        {
            GameObject imageObject = new GameObject(name);
            Image image = imageObject.AddComponent<Image>();
            imageObject.GetComponent<RectTransform>().sizeDelta = size;
            imageObject.transform.SetParent(parent);
            imageObject.transform.localScale = Vector3.one;
            return image;
        }
    }
}
