using PJTC.CatScripts;
using PJTC.Controllers;
using PJTC.Enums;
using PJTC.Structs;
using UnityEngine;
using UnityEngine.UI;

namespace PJTC.Managers.UI
{
    public class AttackViewer : MonoBehaviour
    {
        [Header("AttackIcons")]
        [SerializeField]
        private Sprite jawIcon;

        [SerializeField]
        private Sprite pawIcon;

        [SerializeField]
        private Sprite tailIcon;

        [SerializeField]
        private Sprite crossIcon;

        [Tooltip("Backgrounds for player's team, Orange == 0, Black == 1")]
        [SerializeField]
        Sprite[] bgIcons;

        private Transform attackBanner;
        private Image attackBannerImage;
        private Image attackIcon;
        private GameObject cross;

        private float bias = 30f;

        public void SetAttackBanner(CatData catData)
        {
            Debug.Log("SET BANNER ATTACK TYPE " + catData.attackType);
            Debug.Log("SET BANNER ATTACK HINT " + catData.attackHints.excludedAttack);
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
                    attackIcon.color = new Color(1, 1, 1, 0.5f);
                }

                attackBannerImage.color = attackIcon.color;
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
            Cat cat = GetComponentInParent<Cat>();
            Transform canvas = GameObject.FindGameObjectWithTag("AttackCanvas").transform;

            attackBannerImage = CreateImageObject("AttackBanner", new Vector2(30, 30), canvas);
            attackBannerImage.sprite = bgIcons[(int)cat.catData.team];
            attackBanner = attackBannerImage.transform;
            attackIcon = CreateImageObject("AttackIcon", new Vector2(20, 20), attackBanner);

            if (cat.catData.team == GameController.playerTeam)
            {
                bias *= -1;
            }

            Image crossImage = CreateImageObject("Cross", new Vector2(30, 30), attackBanner);
            crossImage.sprite = crossIcon;
            crossImage.color = new Color(1, 1, 1, 0.5f);

            cross = crossImage.gameObject;
        }

        private Image CreateImageObject(string name, Vector2 size, Transform parent)
        {
            GameObject imageObject = new GameObject(name);
            imageObject.transform.SetParent(parent);
            imageObject.transform.localScale = Vector3.one;

            Image image = imageObject.AddComponent<Image>();
            imageObject.GetComponent<RectTransform>().sizeDelta = size;

            return image;
        }
    }
}
