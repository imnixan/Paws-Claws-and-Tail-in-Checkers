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
            Debug.Log("play hit");
            Instantiate(hitEffect, transform.position, new Quaternion(), transform.parent);
        }

        public void PlayDeathEffect()
        {
            Debug.Log("play lose");
            Instantiate(deathEffect, transform.position, new Quaternion(), transform.parent);
        }

        public void PlayDefendEffect()
        {
            Debug.Log("play defend");
            Instantiate(defendEffect, transform.position, new Quaternion(), transform.parent);
        }

        public void PlayLoseAttackMove()
        {
            Debug.Log("play loseattack");
            Instantiate(loseAttackEffect, transform.position, new Quaternion(), transform.parent);
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
