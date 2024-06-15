using System;
using UnityEngine;

namespace PJTC.Scripts
{
    public class VisualModel : MonoBehaviour
    {
        private SkinnedMeshRenderer meshRenderer;

        public void Init(Material material)
        {
            meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            meshRenderer.material = material;
        }

        public Material GetMaterial()
        {
            return meshRenderer.material;
        }
    }
}
