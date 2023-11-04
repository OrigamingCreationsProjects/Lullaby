using Lullaby;
using Lullaby.Entities;
using UnityEngine;

namespace Level
{
    [AddComponentMenu("Lullaby/Level/Level")]
    public class Level : Singleton<Level>
    {
        protected Player _player;
        protected PlayerCamera _camera;

        /// <summary>
        /// Returns the Player activated in the current Level.
        /// </summary>
        public Player player
        {
            get
            {
                if (!_player)
                    _player = FindObjectOfType<Player>();
                
                return _player;
            }
        }

        public new PlayerCamera camera
        {
            get
            {
                if(!_camera) 
                    _camera = FindObjectOfType<PlayerCamera>();

                return _camera;
            }
        }
    }
}