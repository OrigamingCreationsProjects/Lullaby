using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lullaby;
using UnityEngine;
using UnityEngine.Audio;

namespace Systems.SoundSystem
{
    [AddComponentMenu("Systems/Sound System/Music Manager")]
    public class MusicManager : Singleton<MusicManager>
    {
        public SoundList[] musicPlaylists;
        private Dictionary<string, Sound> sounds = new Dictionary<string, Sound>();
        private Dictionary<MusicType, SoundList> playlists = new Dictionary<MusicType, SoundList>();
        [SerializeField] private AudioMixerGroup masterMixer;
        [SerializeField] private AudioMixerGroup musicMixer;
        [Header("Music Settings")]
        [SerializeField] [Range(0.0f, 1.0f)] private float BGM_MusicVolume;
        [SerializeField] [Range(0.0f, 2.0f)] private float fadeDuration = 1f;
        public SoundList currentPlaylist;
        public Sound currentSong;
        public Sound lastSong;
        
        private void Start()
        {
            for (int i = 0; i < musicPlaylists.Length; i++)
            {
                for (int j = 0; j < musicPlaylists[i].sounds.Length; j++)
                {
                    musicPlaylists[i].sounds[j].audioSource = gameObject.AddComponent<AudioSource>();
                    musicPlaylists[i].sounds[j].audioSource.playOnAwake = false;
                    musicPlaylists[i].sounds[j].audioSource.clip = musicPlaylists[i].sounds[j].soundClip;
                    musicPlaylists[i].sounds[j].audioSource.volume = musicPlaylists[i].sounds[j].volume;
                    musicPlaylists[i].sounds[j].audioSource.pitch = musicPlaylists[i].sounds[j].pitch;
                    musicPlaylists[i].sounds[j].audioSource.loop = musicPlaylists[i].sounds[j].loop;
                    musicPlaylists[i].sounds[j].audioSource.outputAudioMixerGroup = musicPlaylists[i].sounds[j].mixerGroup;
                }
                playlists.Add(musicPlaylists[i].musicType, musicPlaylists[i]);
            }
            currentPlaylist = playlists[MusicType.MainMenu];
            PlayRandomPlaylistSong(currentPlaylist.musicType);
        }

        public void PlayRandomPlaylistSong(MusicType musicType)
        {
            SoundList playlist = playlists[musicType];
            int randomIndex = Random.Range(0, playlist.sounds.Length);
            Sound randomSong = playlist.sounds[randomIndex];
            //currentSong.Stop(); // O transicion suave entre las canciones
            lastSong = currentSong;
            currentSong = randomSong;
            FadeMusicBetweenPlaylist(lastSong, currentSong, fadeDuration);
        }
        public void PlayRandomPlaylistSongExcludingOne(MusicType musicType, Sound excludingSound)
        {
            SoundList playlist = playlists[musicType];
            List<int> auxList = new List<int>();
            for (int i = 0; i < playlist.sounds.Length; i++)
            {
                if(playlist.sounds[i] != excludingSound)
                    auxList.Add(i);
            }
            int randomIndex = Random.Range(0, auxList.Count);
            Sound randomSong = playlist.sounds[auxList[randomIndex]];
            //currentSong.Stop(); // O transicion suave entre las canciones
            lastSong = currentSong;
            currentSong = randomSong;
            FadeMusicBetweenPlaylist(lastSong, currentSong, fadeDuration);
        }
        public void ChangeCurrentPlaylist(MusicType newPlaylist)
        {
            currentPlaylist = playlists[newPlaylist];
            //PlayRandomPlaylistSong(newPlaylist);
            lastSong = currentSong;
            currentSong = GetRandomPlaylistSong(newPlaylist);
            FadeMusicBetweenPlaylist(lastSong, currentSong, fadeDuration);
        }
        public Sound GetRandomPlaylistSong(MusicType musicType)
        {
            SoundList playlist = playlists[musicType];
            int randomIndex = Random.Range(0, playlist.sounds.Length);
            Sound randomSong = playlist.sounds[randomIndex];
            return randomSong;
        }

        public void FadeMusicBetweenPlaylist(Sound songOut, Sound songIn, float duration)
        {
            songIn.Play();
            songOut.audioSource.DOFade(0, duration).onComplete += () => StopSong(songOut);
            songIn.audioSource.DOFade(BGM_MusicVolume, duration);
        } 
        
        public void StopSong(Sound song)
        {
            song.Stop();
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