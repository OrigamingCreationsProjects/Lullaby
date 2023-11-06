using System.Collections;
using Lullaby;
using UnityEngine;
using UnityEngine.Events;

namespace Level
{
    [AddComponentMenu("Lullaby/Level/Level Starter")]
    public class LevelStarter : Singleton<LevelStarter>
    {
        /// <summary>
        /// Called when the starter routine has endend.
        /// </summary>
        public UnityEvent OnStart;

        public float enablePlayerDelay = 1f;
        
        protected Level _level => Level.instance;
        protected LevelPauser _pauser => LevelPauser.instance;
        
        protected Fader _fader => Fader.instance;

        protected virtual IEnumerator Routine()
        {
            GameManager.LockCursor();
            _level.player.controller.enabled = false;
            _level.player.inputs.enabled = false;
            yield return new WaitForSeconds(enablePlayerDelay);
            _level.player.controller.enabled = true;
            _level.player.inputs.enabled = true;
            _pauser.canPause = true;
            OnStart?.Invoke();
        }

        protected virtual void Start()
        {
            StartCoroutine(Routine());
        }
    }
}