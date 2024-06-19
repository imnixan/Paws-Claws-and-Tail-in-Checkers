using UnityEngine;

namespace PJTC.Managers
{
    public class VisualModel : MonoBehaviour
    {
        [Header("Effects")]
        [SerializeField]
        private ParticleSystem hitEffect;

        [SerializeField]
        private ParticleSystem moveEffect;

        [SerializeField]
        private ParticleSystem defendEffect;

        [SerializeField]
        private ParticleSystemRenderer core;

        [SerializeField]
        private ParticleSystem upgradeEffect;

        [Header("Sounds")]
        [SerializeField]
        private AudioClip hitSound;

        [SerializeField]
        private AudioClip defenceSound;

        [SerializeField]
        private AudioClip upgradeSound;

        [SerializeField]
        private AudioClip[] moveSounds;

        [SerializeField]
        private AudioClip[] meowSounds;

        private SkinnedMeshRenderer meshRenderer;
        private Animator animator;

        public void Init(Material material)
        {
            meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            meshRenderer.material = material;

            animator = GetComponentInChildren<Animator>();
        }

        public void PlayHitEffect(Material attackMaterial)
        {
            Debug.Log("play hit");
            core.material = attackMaterial;

            Instantiate(hitEffect, transform.position, new Quaternion()).Play();

            SoundManager.instance.PlaySound(hitSound, 0.6f);
        }

        public void PlayDefendEffect()
        {
            Debug.Log("play defend");

            SoundManager.instance.PlaySound(defenceSound);
            defendEffect.Play();
        }

        public void PlayUpgradeEffect()
        {
            SoundManager.instance.PlaySound(upgradeSound);
            upgradeEffect.Play();
        }

        public void OnInteract()
        {
            if (Random.value > 0.8f)
            {
                SoundManager.instance.PlaySound(
                    meowSounds[Random.Range(0, moveSounds.Length)],
                    0,
                    Random.Range(0.05f, 0.5f)
                );
            }
        }

        public void StartMoving()
        {
            moveEffect.Play();
            SoundManager.instance.PlaySound(
                moveSounds[Random.Range(0, moveSounds.Length)],
                0,
                0.3f
            );
            animator.SetBool("Moving", true);

            Debug.Log("start moving");
        }

        public void StopMoving()
        {
            Debug.Log("end moving");
            moveEffect.Stop();
            animator.SetBool("Moving", false);
        }

        public Material GetMaterial()
        {
            return meshRenderer.material;
        }
    }
}
