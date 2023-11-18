using UnityEngine;
using UnityEngine.Audio;

namespace Systems.SoundSystem
{
    [CreateAssetMenu(fileName = "Sound", menuName = "SoundManager/Sound", order = 0)]
    public class Sound : ScriptableObject
    {
        public AudioClip soundClip;
        [Range(0f, 1f)] public float volume = 0.5f;
        [Range(.1f, 3f)] public float pitch = 1f;
        public bool loop = false;
        [HideInInspector] public AudioSource audioSource;
        public AudioMixerGroup mixerGroup;
        public void Play()
        {
            if (!audioSource.isPlaying)
            {
                audioSource.Play();
            }
        }

        public void PlayOneShot()
        {
            audioSource.PlayOneShot(audioSource.clip);
        }
        public void PlayDelayed(float delay)
        {
            if (!audioSource.isPlaying)
            {
                audioSource.PlayDelayed(delay);
            }
        }
        public void Stop()
        {
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }

        public float GetDuration()
        {
            return audioSource.clip.length;
        }
    }
}