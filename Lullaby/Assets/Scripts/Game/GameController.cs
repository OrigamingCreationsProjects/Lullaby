using UnityEngine;

namespace Lullaby
{
    [AddComponentMenu("Lullaby/Game/Game Controller")]
    public class GameController : MonoBehaviour
    {
        protected GameManager _gameManager => GameManager.instance;
        protected GameSceneLoader _sceneLoader => GameSceneLoader.instance;
        
        public virtual void LoadScene(string scene) => _sceneLoader.Load(scene);
    }
}