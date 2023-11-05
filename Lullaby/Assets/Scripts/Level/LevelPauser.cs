using Lullaby;
using Lullaby.UI;
using UnityEngine;
using UnityEngine.Events;

namespace Level
{
    [AddComponentMenu("Lullaby/Level/Level Pauser")]
    public class LevelPauser : Singleton<LevelPauser>
    {
        /// <summary>
        /// Called when the level is paused
        /// </summary>
        public UnityEvent OnPaused;

        /// <summary>
        /// Called when the Level is unpaused.
        /// </summary>
        public UnityEvent OnUnpaused;

        public UIAnimator pauseScreen;
        
        /// <summary>
        /// Returns if it's possible to pause the Level.
        /// </summary>
        public bool canPause  { get; set; }
        
        /// <summary>
        /// Returns if the Level is paused.
        /// </summary>
        public bool paused { get; protected set; }


        public virtual void Pause(bool value)
        {
            //Comprobamos que no se este intentando poner en el mismo estado que ya estamos
            if (paused != value)
            {
                Debug.Log("Pasamos el primer if");
                if (!paused)
                {
                    Debug.Log("Pasamos el segundo if");
                    //Comprobamos esta condicion ya que podriamos querer que no se pueda pausar en ciertos momentos
                    if (canPause) 
                    {
                        Debug.Log("Pasamos el tercer if");
                        //DESBLOQUEAR CURSOR AQUI PARA QUE PUEDAN NAVEGAR POR EL MENU (Si está en pc o el ultimo control no es mando)
                        GameManager.LockCursor(false);
                        paused = true;
                        Time.timeScale = 0;
                        pauseScreen.SetActive(true);
                        pauseScreen?.Show();
                        OnPaused?.Invoke();
                    }
                }
                else
                {
                    //BLOQUEAR CURSOR AQUI PARA QUE NO LO VEAN EN GAMEPLAY 
                    GameManager.LockCursor();
                    paused = false;
                    Time.timeScale = 1;
                    pauseScreen?.Hide();
                    OnUnpaused?.Invoke();
                }
            }
        }
    }
}