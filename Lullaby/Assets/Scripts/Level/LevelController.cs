using Lullaby;
using UnityEngine;

namespace Level
{
    [AddComponentMenu("Lullaby/Level/Level Controller")]
    public class LevelController : MonoBehaviour
    {
        // -- MANAGERS DE NIVEL --
        #region Managers de nivel
        protected LevelFinisher _finisher => LevelFinisher.instance;
        protected LevelRespawner _respawner => LevelRespawner.instance;
        protected LevelPauser _pauser => LevelPauser.instance;
        
        #endregion
        
        // -- FUNCIONES GENERALES DE NIVEL --
        #region Funciones generales de nivel
       
        public virtual void Finish() => _finisher.Finish();
        
        public virtual void Exit() => _finisher.Exit();

        public virtual void Respawn() => _respawner.Respawn();
        public virtual void Restart() => _respawner.Restart();
        
        public virtual void Pause(bool value) => _pauser.Pause(value);

        #endregion

        void Update()
        {
            // if (_pauser.paused && Cursor.lockState != CursorLockMode.None)
            // {
            //     GameManager.LockCursor(false);
            // }
        }
    }
}