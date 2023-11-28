using System;
using Lullaby.LevelManagement;
using UnityEngine;

namespace Lullaby
{
    [Serializable]
    public class GameLevel
    {
        [Header("General Settings")] 
        public string name;
        public string scene;
        public string description;
        public Sprite image;
        
        [Header("Looking Settings")]
        [Tooltip("El nivel sera inaccesible desde la seccion de niveles a no ser que se desbloquee mediante codigo.")]
        public bool locked;

        /// <summary>
        /// Load this Game Level state from a given Game Data.
        /// </summary>
        /// <param name="data"> The Game Data to read the state from </param>
        public virtual void LoadState(LevelData data)
        {
            locked = data.locked;
        }
        
        /// <summary>
        /// Returns this Level Data of this Game level to be used by the Data Layer
        /// </summary>
        /// <returns></returns>
        public virtual LevelData ToData()
        {
            return new LevelData()
            {
                locked = this.locked
            };
        }
    }
}