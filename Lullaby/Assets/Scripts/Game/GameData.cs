using System;
using System.Linq;
using Lullaby.LevelManagement;
using UnityEngine;

namespace Lullaby
{
    [Serializable]
    public class GameData
    {
        public LevelData[] levels;
            
        /// <summary>
        /// Returns a new instance of a Game Data at runtime
        /// </summary>
        public static GameData Create()
        {
            return new GameData()
            {
                levels = GameManager.instance.levels.Select((level) =>
                {
                    return new LevelData()
                    {
                        locked = level.locked
                    };
                }).ToArray()
            };
        }

        public virtual string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        public static GameData FromJson(string json)
        {
            return JsonUtility.FromJson<GameData>(json);
        }
    }
}