using System.Collections;
using System.Collections.Generic;
using Lullaby.Entities.Enemies;
using UnityEngine;

namespace Lullaby.Entities.Enemies
{
    public class DollyManager : MonoBehaviour
    {
        private Dolly[] _dolliesClones;
        public EnemyStruct[] allEnemies;
        private List<int> enemyIndexes;
        
        [Header("Main Dolly AI Loop - Settings")]
        private Coroutine _AI_LoopCoroutine;
        
        public int aliveDollysCount;

        
        
        public void StartAI()
        {
            _AI_LoopCoroutine = StartCoroutine(AI_Loop(null));
        }

        IEnumerator AI_Loop(Dolly dolly)
        {
            if (GetAliveEnemyCount() == 0)
            {
                StopCoroutine(AI_Loop(null));
                yield break;
            }
        }






        public int GetAliveEnemyCount()
        {
            int count = 0;
            for (int i = 0; i < allEnemies.Length; i++)
            {
                if (allEnemies[i].dollyScript.isActiveAndEnabled)
                    count++;
            }
            aliveDollysCount = count;
            return count;
        }
        
        public void SetEnemyAvailiability(Dolly enemy, bool state)
        {
            for (int i = 0; i < allEnemies.Length; i++)
            {
                if (allEnemies[i].dollyScript == enemy)
                    allEnemies[i].enemyAvailability = state;
            }

            // if (FindObjectOfType<EnemyDetection>().CurrentTarget() == enemy)
            //     FindObjectOfType<EnemyDetection>().SetCurrentTarget(null);
        }
    }
    [System.Serializable]
    public struct EnemyStruct
    {
        public Dolly dollyScript;
        public bool enemyAvailability;
    }
}

