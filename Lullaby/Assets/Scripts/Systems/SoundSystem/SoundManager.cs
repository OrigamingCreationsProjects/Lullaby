using System.Collections.Generic;
using DG.Tweening;
using Lullaby;
using UnityEngine;
using UnityEngine.Audio;

namespace Systems.SoundSystem
{
    [AddComponentMenu("Systems/SoundSystem/SoundManager")]
    public class SoundManager : Singleton<SoundManager>
    {
        public SoundList[] lists;
        private Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();
        [SerializeField] private AudioMixerGroup masterMixer;
        [SerializeField] private AudioMixerGroup musicMixer;
        [SerializeField] [Range(0.0f, 1.0f)] private float BGM_MusicVolume;
        
        private void Start()
        {
            for (int i = 0; i < lists.Length; i++)
            {
                for (int j = 0; j < lists[i].sounds.Length; j++)
                {
                    Sound sound = lists[i].sounds[j];
                    sound.audioSource = gameObject.AddComponent<AudioSource>();
                    sound.audioSource.clip = sound.soundClip;
                    sound.audioSource.volume = sound.volume;
                    sound.audioSource.pitch = sound.pitch;
                    sound.audioSource.loop = sound.loop;
                    sound.audioSource.outputAudioMixerGroup = sound.mixerGroup;
                    sounds.Add(sound.name, sound);
                    //Debug.Log($"Pooled {sound.name}");
                }
            }
            Play("MainMenu_Music");
        }
        
        public void Play(string name)
        {
            sounds[name]?.Play();
        }

        public void PlayOneShot(string name)
        {
            sounds[name]?.PlayOneShot();
        }
        public void PlayDelayed(string name, float delay)
        {
            sounds[name]?.PlayDelayed(delay);
        }
        public void Stop(string name)
        {
            sounds[name]?.Stop();
        }

        public float GetSoundDuration(string name)
        {
            return sounds[name].GetDuration();
        }

        public void FadeBGMClipsVolumes(string clipOut, string clipIn, float duration)
        {
            sounds[clipOut].audioSource.DOFade(0, duration);
            sounds[clipIn].audioSource.DOFade(BGM_MusicVolume, duration);
        }
        
        public void ChangeGeneralVolume(float percent)
        {
            float volume = percent > 0? 20 * Mathf.Log10(percent) : -80;
            masterMixer.audioMixer.SetFloat("MasterVol", volume);

        }
        
        public void ChangeMusicVolume(float percent)
        {
            float volume = percent > 0? 20 * Mathf.Log10(percent) : -80;
            musicMixer.audioMixer.SetFloat("MusicVol", volume);
        }
    }
}