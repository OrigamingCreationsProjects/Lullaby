using Lullaby;
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace Level
{
    [AddComponentMenu("Lullaby/Level/Level Finisher")]
    public class LevelFinisher : Singleton<LevelFinisher>
    {
        /// <summary>
        /// Called when the level is finished
        /// </summary>
        public UnityEvent OnFinish;

        /// <summary>
        /// Called when the Level has exited.
        /// </summary>
        public UnityEvent OnExit;
        
        [Tooltip("The scene to load when this level is finished. If its a level, its 'locking' properties are ignored.")]
        public string nextScene;
        [Tooltip("The scene to load when exiting this level.")]
        public string exitScene;
        [Tooltip("The delay in seconds applied when exiting or finishing the level.")]
        public float loadingDelay = 1f;
        
        protected GameManager _gameManager => GameManager.instance;
        protected Level _level => Level.instance;
        protected LevelPauser _pauser => LevelPauser.instance;
        //GameSceneLoader
        protected GameSceneLoader _sceneLoader => GameSceneLoader.instance;
        
        protected Fader _fader => Fader.instance;
        //Fader

        /// <summary>
        /// Invokes the Level finishing routine to load the next scene
        /// </summary>
        public virtual void Finish()
        {
            StopAllCoroutines();
            StartCoroutine(FinishRoutine());
        }
        
        /// <summary>
        /// Invokes the Level exit routine
        /// </summary>
        public virtual void Exit()
        {
            StopAllCoroutines();
            StartCoroutine(ExitRoutine());
        }

        protected virtual IEnumerator FinishRoutine()
        {
            _pauser.Pause(false);
            _pauser.canPause = false;
            _level.player.inputs.enabled = false;

            yield return new WaitForSeconds(loadingDelay);
            
            GameManager.LockCursor(false);
            
            
            //Cargar siguiente escena
            _sceneLoader.Load(nextScene);
            OnFinish?.Invoke();
        }

        protected virtual IEnumerator ExitRoutine()
        {
            Debug.Log("Empezamos corutina de salir");
            _pauser.Pause(false);
            _pauser.canPause = false;
            _level.player.inputs.enabled = false;
            yield return new WaitForSeconds(loadingDelay);
            GameManager.LockCursor(false);
            //Cargar escena de salida
            Debug.Log("Llegamos a la instruccion load");
            _sceneLoader.Load(exitScene);
            OnExit?.Invoke();
        }
        
    }
}