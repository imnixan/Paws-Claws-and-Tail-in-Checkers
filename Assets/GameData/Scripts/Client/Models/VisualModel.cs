using UnityEngine;

namespace PJTC.Managers
{
    public class VisualModel : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem hitEffect;

        [SerializeField]
        private ParticleSystem moveEffect;

        [SerializeField]
        private ParticleSystem defendEffect;

        [SerializeField]
        private ParticleSystemRenderer core;

        private SkinnedMeshRenderer meshRenderer;

        public void Init(Material material)
        {
            meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            meshRenderer.material = material;
        }

        public void PlayHitEffect(Material attackMaterial)
        {
            Debug.Log("play hit");
            core.material = attackMaterial;

            Instantiate(hitEffect, transform.position, new Quaternion());
        }

        public void PlayDefendEffect()
        {
            Debug.Log("play defend");
            defendEffect.Play();
        }

        public void StartMoving()
        {
            moveEffect.Play();
            Debug.Log("start moving");
        }

        public void StopMoving()
        {
            Debug.Log("end moving");
            moveEffect.Stop();
        }

        public Material GetMaterial()
        {
            return meshRenderer.material;
        }
    }
}
