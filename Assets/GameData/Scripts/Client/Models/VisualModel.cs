using UnityEngine;

namespace PJTC.Managers
{
    public class VisualModel : MonoBehaviour
    {
        [SerializeField]
        private ParticleSystem hitEffect;

        [SerializeField]
        private ParticleSystem deathEffect;

        [SerializeField]
        private ParticleSystem moveEffect;

        [SerializeField]
        private ParticleSystem defendEffect;

        [SerializeField]
        private ParticleSystem loseAttackEffect;

        private SkinnedMeshRenderer meshRenderer;

        public void Init(Material material)
        {
            meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            meshRenderer.material = material;
        }

        public void PlayHitEffect()
        {
            Instantiate(hitEffect, transform.position, new Quaternion(), transform.parent);
        }

        public void PlayDeathEffect()
        {
            Instantiate(deathEffect, transform.position, new Quaternion(), transform.parent);
        }

        public void PlayDefendEffect()
        {
            Instantiate(defendEffect, transform.position, new Quaternion(), transform.parent);
        }

        public void PlayLoseAttackMove()
        {
            Instantiate(loseAttackEffect, transform.position, new Quaternion(), transform.parent);
        }

        public void StartMoving()
        {
            moveEffect.Play();
        }

        public void StopMoving()
        {
            moveEffect.Stop();
        }

        public Material GetMaterial()
        {
            return meshRenderer.material;
        }
    }
}
