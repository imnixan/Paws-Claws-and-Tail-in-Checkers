using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using PJTC.Enums;
using UnityEngine;

namespace PJTC.Scripts
{
    public class AttackViewer : MonoBehaviour
    {
        [SerializeField]
        private Texture2D pawsIcon;

        [SerializeField]
        private Texture2D jawsIcon;

        [SerializeField]
        private Texture2D tailIcon;

        [SerializeField]
        private Texture2D noneIcon;

        [SerializeField]
        private MeshRenderer renderer;

        private Material material;

        public void SetAttackBanner(CatsType.Attack attackType)
        {
            if (material == null)
            {
                material = new Material(renderer.material);
            }
            Texture2D currentTexture = noneIcon;
            switch (attackType)
            {
                case CatsType.Attack.Tail:
                    currentTexture = tailIcon;
                    break;
                case CatsType.Attack.Paws:
                    currentTexture = pawsIcon;
                    break;
                case CatsType.Attack.Jaws:
                    currentTexture = jawsIcon;
                    break;
            }
            material.mainTexture = currentTexture;
            renderer.material = material;
        }
    }
}
