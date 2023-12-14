using System.Collections;
using System.Collections.Generic;
using Systems.SoundSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Lullaby.LevelManagement
{
    [AddComponentMenu("Lullaby/Level/Level Respawner")]
    public class LevelRespawner : Singleton<LevelRespawner>
    {
        /// <summary>
        /// Called after the Respawn routine ended.
        /// </summary>
        public UnityEvent OnRespawn;
        
        /// <summary>
        /// Called after the Game Over routine ended.
        /// </summary>
        public UnityEvent OnGameOver;

        public float respawnFadeOutDelay = 1f;
        public float respawnFadeInDelay = 0.5f;
        public float gameOverFadeOutDelay = 5f;
        public float restartFadeOutDelay = 0.5f;

        protected List<PlayerCamera> _cameras;
        
        protected Level _level => Level.instance;
        protected LevelPauser _pauser => LevelPauser.instance;
        protected GameManager _gameManager => GameManager.instance;
        protected Fader _fader => Fader.instance;

        /// <summary>
        ///  Invokes either Respawn or Game Over routine depending of the retries available.
        /// </summary>
        public virtual void Respawn()
        {
            //Debug.Log("Entramos a respawn");
            StopAllCoroutines();
            if (SceneManager.GetActiveScene().name == "FinalBossScene")
            {
                StartCoroutine(RestartRoutine());
            }
            else
            {
                StartCoroutine(Routine());
            }
            //StartCoroutine(Routine());
        }

        /// <summary>
        /// Restart the current Level loading the scene again.
        /// </summary>
        public virtual void Restart()
        {
            StopAllCoroutines();
            StartCoroutine(RestartRoutine());
        }
        
        protected virtual IEnumerator RespawnRoutine()
        {
            if (_level.player.health.current <= 0)
            {
                _level.player.Respawn();
                
            }
            else
            {
                _level.player.RespawnWithCurrentHealth();
            }
            _level.camera.freeze = false;

            ResetCameras();
            OnRespawn?.Invoke();

            yield return new WaitForSeconds(respawnFadeInDelay);
            
            //Cosas del fader
            _fader.FadeIn(() =>
            {
                _pauser.canPause = true;
                _level.player.inputs.enabled = true;
            });
        }

        protected virtual IEnumerator Routine()
        {
            _pauser.Pause(false);
            _pauser.canPause = false;
            _level.player.inputs.enabled = false;
            _level.camera.freeze = true;

            // if (_level.player.health.current <= 0)
            // {
            //     StartCoroutine(GameOverRoutine());
            //     yield break;
            // }
            
            yield return new WaitForSeconds(respawnFadeOutDelay);
            
            //Cosas del fader
            _fader.FadeOut(() => StartCoroutine(RespawnRoutine()));
        }
        protected virtual IEnumerator RestartRoutine()
        {
            _pauser.Pause(false);
            _pauser.canPause = false;
            _level.player.inputs.enabled = false;
            MusicManager.instance.StopSceneMusic();
            yield return new WaitForSeconds(restartFadeOutDelay);
            //MusicManager.instance.PlayRandomPlaylistSong(MusicManager.instance.currentPlaylist.musicType);
            MusicManager.instance.currentSong.Play();
            GameSceneLoader.instance.Reload();
        }
        protected virtual IEnumerator GameOverRoutine()
        {
            yield return new WaitForSeconds(gameOverFadeOutDelay);
            //Recargar escena
            OnGameOver?.Invoke();
        }
        
        protected virtual void ResetCameras()
        {
            foreach (var camera in _cameras)
            {
                camera.Reset();
            }
        }
        
        protected virtual void Start()
        {
            _cameras = new List<PlayerCamera>(FindObjectsOfType<PlayerCamera>());
            _level.player.playerEvents.OnDie.AddListener(() => Respawn());
            _level.player.playerEvents.OnDeadlyFall.AddListener(() => Respawn());
        }
        
    }
}