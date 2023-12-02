using System.Collections.Generic;
using DG.Tweening;
using Lullaby;
using UnityEngine;
using UnityEngine.Audio;

namespace Systems.SoundSystem
{
    [AddComponentMenu("Systems/Sound System/Sound Manager")]
    public class MusicManager : Singleton<MusicManager>
    {
        public SoundList[] musicPlaylists;
        private Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();
        private Dictionary<MusicType, SoundList> playlists = new Dictionary<MusicType, SoundList>();
        [SerializeField] private AudioMixerGroup masterMixer;
        [SerializeField] private AudioMixerGroup musicMixer;
        [SerializeField] [Range(0.0f, 1.0f)] private float BGM_MusicVolume;
        public SoundList currentPlaylist;
        public Sound currentSong;
        private void Start()
        {
            for (int i = 0; i < musicPlaylists.Length; i++)
            {
                for (int j = 0; j < musicPlaylists[i].sounds.Length; j++)
                {
                    musicPlaylists[i].sounds[j].audioSource = gameObject.AddComponent<AudioSource>();
                    musicPlaylists[i].sounds[j].audioSource.clip = musicPlaylists[i].sounds[j].soundClip;
                    musicPlaylists[i].sounds[j].audioSource.volume = musicPlaylists[i].sounds[j].volume;
                    musicPlaylists[i].sounds[j].audioSource.pitch = musicPlaylists[i].sounds[j].pitch;
                    musicPlaylists[i].sounds[j].audioSource.loop = musicPlaylists[i].sounds[j].loop;
                    musicPlaylists[i].sounds[j].audioSource.outputAudioMixerGroup = musicPlaylists[i].sounds[j].mixerGroup;
                    //Debug.Log($"Pooled {sound.name}");
                }
                playlists.Add(musicPlaylists[i].musicType, musicPlaylists[i]);
            }
            
            
            //Play("MainMenu_Music");
        }
        
        public void PlayRandomPlaylistSong(MusicType musicType)
        {
            SoundList playlist = playlists[musicType];
            int randomIndex = Random.Range(0, playlist.sounds.Length);
            Sound randomSong = playlist.sounds[randomIndex];
            currentSong.Stop(); // O transicion suave entre las canciones
            currentSong = randomSong;
            currentSong.Play();
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
            sounds[clipIn].audioSource.DOFade(0.2f, duration);
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