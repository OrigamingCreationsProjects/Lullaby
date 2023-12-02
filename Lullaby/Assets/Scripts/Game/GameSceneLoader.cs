using System.Collections;
using Lullaby.UI;
using Systems.SoundSystem;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace Lullaby
{
    [AddComponentMenu("Lullaby/Game/Game Scene Loader")]
    public class GameSceneLoader : Singleton<GameSceneLoader>
    {
        /// <summary>
        /// Called when the loading proccess has started.
        /// </summary>
        public UnityEvent OnLoadStart;
        
        /// <summary>
        /// Called when the loading proccess has finished.
        /// </summary>
        public UnityEvent OnLoadFinish;
        
        public UIAnimator loadingScreen;
        
        [Header("Minimum Time")]
        public float startDelay = 1f;
        public float finishDelay = 1f;
        
        /// <summary>
        /// Returns true if there's any loading in proccess.
        /// </summary>
        public bool isLoading { get; protected set; }
        
        /// <summary>
        /// Returns the loading percentage
        /// </summary>
        public float loadingProgress { get; protected set; }

        /// <summary>
        /// Returns the name of the current scene.
        /// </summary>
        public string currentScene => SceneManager.GetActiveScene().name;

        /// <summary>
        /// Reloads the current scene
        /// </summary>
        public virtual void Reload()
        {
            StartCoroutine(LoadRoutine(currentScene));
        }

        /// <summary>
        /// Loads a given scene based on its name.
        /// </summary>
        /// <param name="scene">The name of the scene we want to load</param>
        public virtual void Load(string scene)
        {
            Debug.Log("Llegamos a Load");
            if (!isLoading && (currentScene != scene))
            {
                Debug.Log("Entramos en el if");
                StartCoroutine(LoadRoutine(scene));
            }
        }

        protected virtual IEnumerator LoadRoutine(string scene)
        {
            OnLoadStart?.Invoke();
            isLoading = true;
            loadingScreen.SetActive(true);
            loadingScreen.Show();

            yield return new WaitForSeconds(startDelay);

            var operation = SceneManager.LoadSceneAsync(scene);
            loadingProgress = 0;

            while (!operation.isDone)
            {
                loadingProgress = operation.progress;
                yield return null;
            }

            loadingProgress = 1;

            yield return new WaitForSeconds(finishDelay);
            isLoading = false;
            loadingScreen.Hide();
            OnLoadFinish?.Invoke();
            
            //YA ESTAS TARDANDO EN QUITAR ESTA PUTISIMA MIERDA
            if (scene == "Level1_Beta")
            {
                // SoundManager.instance.Stop("MainMenu_Music");
                // SoundManager.instance.Play("Aloras_DanceA(BGM2)");
                MusicManager.instance.ChangeCurrentPlaylist(MusicType.Platforming);
            }
            else if(scene == "MainMenu" && currentScene == "Level1_Beta")
            {
                MusicManager.instance.ChangeCurrentPlaylist(MusicType.MainMenu);
            }
        }
    }
}