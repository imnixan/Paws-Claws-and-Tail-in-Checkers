using System;
using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

namespace PJTC.Managers
{
    public class SoundManager : MonoBehaviour
    {
        public static SoundManager instance { get; private set; }

        [SerializeField]
        private AudioSource soundPlayer;

        [SerializeField]
        private AudioSource musicSource;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this);
            }
            else
            {
                instance = this;
            }
        }

        public void PlaySound(AudioClip sound, float delay = 0, float volume = 1)
        {
            if (delay > 0)
            {
                StartCoroutine(PlaySoundWithDelay(sound, delay, volume));
            }
            else
            {
                soundPlayer.PlayOneShot(sound, volume);
            }
        }

        public void SetSoundValue(float soundValue)
        {
            soundPlayer.volume = soundValue;
        }

        public void SetMusicValue(float musicValue)
        {
            musicSource.volume = musicValue;
        }

        private IEnumerator PlaySoundWithDelay(AudioClip sound, float delay, float volume)
        {
            yield return new WaitForSecondsRealtime(delay);
            soundPlayer.PlayOneShot(sound, volume);
        }
    }
}
