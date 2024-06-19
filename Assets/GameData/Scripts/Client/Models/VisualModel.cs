using System.Collections;
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

        [SerializeField]
        private AnimationClip[] idleAnims;

        [SerializeField]
        private AnimationClip moveAnim;

        private SkinnedMeshRenderer meshRenderer;
        public Animation animator;

        public void Init(Material material)
        {
            meshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
            meshRenderer.material = material;
            InitAnims();
            StartCoroutine(RandomMoves());
        }

        private IEnumerator RandomMoves()
        {
            while (true)
            {
                yield return new WaitForSeconds(Random.Range(0, 25));
                if (!animator.isPlaying)
                {
                    string idleName = $"Idle{Random.Range(0, 2)}";
                    animator.Play(idleName);
                }
            }
        }

        private void InitAnims()
        {
            animator = GetComponentInChildren<Animation>();
            if (!moveAnim.legacy)
            {
                moveAnim.legacy = true;
            }
            animator.AddClip(moveAnim, "Move");
            for (int i = 0; i < idleAnims.Length; i++)
            {
                if (!idleAnims[i].legacy)
                {
                    idleAnims[i].legacy = true;
                }
                animator.AddClip(idleAnims[i], $"Idle{i}");
            }
            Debug.Log(animator.GetClipCount());
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
            string idleName = $"Idle{Random.Range(0, 2)}";
            animator.Play(idleName);
            if (Random.value > 0.5f)
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
                0.1f
            );
            animator.Play("Move");

            Debug.Log("start moving");
        }

        public void StopMoving()
        {
            moveEffect.Stop();
            Debug.Log("end moving");
            string idleName = $"Idle{Random.Range(0, 2)}";
            animator.Play(idleName);
        }

        public Material GetMaterial()
        {
            return meshRenderer.material;
        }
    }
}
