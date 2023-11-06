using System.Collections.Generic;
using System.Linq;
using Level;
using UnityEngine;

namespace Lullaby
{
    [AddComponentMenu("Lullaby/Game/Game Manager")]
    public class GameManager : Singleton<GameManager>
    {
        public List<GameLevel> levels;

        protected int _dataIndex;
        
        /// <summary>
        /// Sets the cursor lock and hide state.
        /// </summary>
        /// <param name="value">If true, the cursor will be hidden</param>
        public static void LockCursor(bool value = true)
        {
#if UNITY_STANDALONE || UNITY_WEBGL
            //Cursor.visible = value;
            Cursor.lockState = value? CursorLockMode.Locked : CursorLockMode.None;
#endif
        }

        /// <summary>
        /// Loads this Game state from a given Game Data.
        /// </summary>
        /// <param name="index">The index of the Game Data.</param>
        /// <param name="data">The Game Data to read the state from.</param>
        public virtual void LoadState(int index, GameData data)
        {
            _dataIndex = index;

            for (int i = 0; i < data.levels.Length; i++)
            {
                levels[i].LoadState(data.levels[i]);
            }
        }

        /// <summary>
        /// Returns the Game Level array as Level Data.
        /// </summary>
        public virtual LevelData[] LevelsData()
        {
            return levels.Select(level => level.ToData()).ToArray();
        }

        //public virtual GameLevel GetCurrentLevel() { }
        
        
        //public virtual void UnlockNextLevel(){}


        /// <summary>
        /// Returns the Game Data of this Game to be used by the Data Layer.
        /// </summary>
        public virtual GameData ToData()
        {
            return new GameData()
            {
                levels = LevelsData(),
            };
        }

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
    }
}