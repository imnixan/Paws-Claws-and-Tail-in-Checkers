using System;
using UnityEngine;

namespace PCTC
{
    public class Carpet : MonoBehaviour
    {
        [SerializeField]
        private Material choosedMat;
        private Material defaultMat;

        private MeshRenderer meshRenderer;

        private void Start()
        {
            this.meshRenderer = GetComponent<MeshRenderer>();
            this.defaultMat = this.meshRenderer.material;
        }

        public void ActivateCell()
        {
            this.meshRenderer.material = choosedMat;
        }

        public void DeactivateCell()
        {
            this.meshRenderer.material = defaultMat;
        }
    }
}
