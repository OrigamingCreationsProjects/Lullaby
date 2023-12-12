using System;
using Lullaby.LevelManagement;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lullaby.Entities
{
    [RequireComponent(typeof(Player))]
    [AddComponentMenu("Lullaby/Entities/Player/Player Sound")]
    public class PlayerSound : MonoBehaviour
    {
        [Header("Voices")] 
        public AudioClip[] jump;
        public AudioClip[] hurt;
        public AudioClip[] attack;
        public AudioClip[] deadlyFall;
        public AudioClip[] ledgeClimbing;
        public AudioClip[] idleVoices;
        
        [Header("Effects")] 
        public AudioClip airDash;
        public AudioClip ledgeGrabbing;
        public AudioClip startRailGrind;
        public AudioClip railGrind;
        public AudioClip pauseGame;
        public AudioClip unpauseGame;
        [Header("Other Sources")]
        public AudioSource grindAudio;
        public AudioSource pauseAudio;
        
        protected Player _player;
        protected AudioSource _audioSource;
        
        protected virtual void InitializePlayer() => _player = GetComponent<Player>();

        protected virtual void InitializeAudio()
        {
            if (!TryGetComponent(out _audioSource))
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }
        }

        protected virtual void PlayRandom(AudioClip[] clips)
        {
            if (clips != null && clips.Length > 0)
            {
                var index = Random.Range(0, clips.Length);
                
                if(clips[index])
                    Play(clips[index]);
            }
        }

        protected virtual void Play(AudioClip audio, bool stopPrevious = true)
        {
            if(audio == null) 
                return;
            
            if(stopPrevious)
                _audioSource.Stop();
            
            _audioSource.PlayOneShot(audio);
        }

        protected virtual void InitializeCallbacks()
        {
            _player.playerEvents.OnJump.AddListener(() => PlayRandom(jump));
            _player.playerEvents.OnHurt.AddListener(() => PlayRandom(hurt));
            _player.playerEvents.OnAttackStarted.AddListener(() => PlayRandom(attack));
            _player.playerEvents.OnDeadlyFall.AddListener(() => PlayRandom(deadlyFall));
            _player.playerEvents.OnDashStarted.AddListener(() => Play(airDash));
            _player.playerEvents.OnLedgeGrabbed.AddListener(() => Play(ledgeGrabbing, false));
            _player.playerEvents.OnLedgeClimbing.AddListener(() => PlayRandom(ledgeClimbing));
            _player.playerEvents.OnRandomIdleEnter.AddListener((x) => Play(idleVoices[x]));
            _player.playerEvents.OnRandomIdleExit.AddListener(() => _audioSource.Stop());
            
            _player.entityEvents.OnRailsExit.AddListener(() => grindAudio?.Stop());
            
            _player.entityEvents.OnRailsEnter.AddListener(() =>
            {
                Play(startRailGrind, false);
                grindAudio?.Play();
            });
            
            LevelPauser.instance?.OnPaused.AddListener(() =>
            {
                pauseAudio.clip = pauseGame;
                pauseAudio?.Play();
                _audioSource.Pause();
                grindAudio.Pause();
            });
            
            LevelPauser.instance?.OnUnpaused.AddListener(() =>
            {
                pauseAudio.clip = unpauseGame;
                pauseAudio?.Play();
                _audioSource.UnPause();
                grindAudio.UnPause();
            });
        }

        protected void Start()
        {
            InitializeAudio();
            InitializePlayer();
            InitializeCallbacks();
        }
    }
}