using System;
using UnityEngine;

namespace PJTC.Scripts
{
    public class VisualModel : MonoBehaviour
    {
        [SerializeField]
        private MeshRenderer meshRenderer;

        public void Init(Material material)
        {
            meshRenderer.material = material;
        }

        public Material GetMaterial()
        {
            return meshRenderer.material;
        }
    }
}
