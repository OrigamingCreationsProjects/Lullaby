using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lullaby.Entities.Enemies;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lullaby.Entities.Enemies
{
    public class DollyManager : MonoBehaviour
    {
        public DollyStruct[] allEnemies;
        public float minTimeToAttackAgain = 0.1f;
        public float maxTimeToAttackAgain = 3f;
        public Dolly attackingDolly;
        
        private Dolly[] _dollysClones;
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
                Debug.Log("PARAMOS EL AI LOOP");
                StopCoroutine(AI_Loop(null));
                yield break;
            }

            yield return new WaitForSeconds(Random.Range(0.5f, 1.5f));
            
            attackingDolly = RandomDollyExcludingOne(dolly);

            if (attackingDolly == null)
                attackingDolly = RandomDolly();

            if (attackingDolly == null)
                yield break;
            Debug.Log("Dolly attacking is " + attackingDolly.name);
            Debug.Log("Esperamos a que RETREATING sea false");
            yield return new WaitUntil(() => attackingDolly.IsRetreating() == false);
            Debug.Log("Esperamos a que LOCKED TARGET sea false");
            yield return new WaitUntil(() => attackingDolly.IsLockedTarget() == false);
            Debug.Log("Esperamos a que STUNNED sea false");
            yield return new WaitUntil(() => attackingDolly.IsStunned() == false);
            
            attackingDolly.SetAttack();
            
            Debug.Log("Esperamos a que PREPARINGATTACK sea false");
            yield return new WaitUntil(() => attackingDolly.IsPreparingAttack() == false);
            
            attackingDolly.SetRetreat();

            yield return new WaitForSeconds(Random.Range(minTimeToAttackAgain, maxTimeToAttackAgain));
            
            if(GetAliveEnemyCount() > 0)
                _AI_LoopCoroutine = StartCoroutine(AI_Loop(attackingDolly));
        }

        public Dolly RandomDolly()
        {
            enemyIndexes = new List<int>();

            for (int i = 0; i < allEnemies.Length; i++)
            {
                if(allEnemies[i].enemyAvailability)
                    enemyIndexes.Add(i);
            }

            if (enemyIndexes.Count == 0)
                return null;
            
            Dolly randomDolly;
            int randomIndex = Random.Range(0, enemyIndexes.Count);
            randomDolly = allEnemies[enemyIndexes[randomIndex]].dollyScript;
            return randomDolly;
        }


        public Dolly RandomDollyExcludingOne(Dolly excludedDolly)
        {
            enemyIndexes = new List<int>();

            for (int i = 0; i < allEnemies.Length; i++)
            {
                if(allEnemies[i].enemyAvailability && allEnemies[i].dollyScript != excludedDolly)
                    enemyIndexes.Add(i);
            }

            if (enemyIndexes.Count == 0)
            {
                return null;
            }

            Dolly randomDolly;
            int randomIndex = Random.Range(0, enemyIndexes.Count);
            randomDolly = allEnemies[enemyIndexes[randomIndex]].dollyScript;
            return randomDolly;
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
        
        public void SetEnemyAvailability(Dolly enemy, bool state)
        {
            for (int i = 0; i < allEnemies.Length; i++)
            {
                if (allEnemies[i].dollyScript == enemy)
                    allEnemies[i].enemyAvailability = state;
            }

            if (FindObjectOfType<PlayerEnemyDetector>().CurrentTarget() == enemy)
                FindObjectOfType<PlayerEnemyDetector>().SetCurrentTarget(null);
            if (attackingDolly == enemy)
            {
                StopCoroutine(AI_Loop(null));
                StartAI();
            }
        }
        
        public bool ADollyIsPreparingAttack()
        {
            foreach (DollyStruct enemyStruct in allEnemies)
            {
                if (enemyStruct.dollyScript.IsPreparingAttack())
                {
                    return true;
                }
            }
            return false;
        }


        private void Start()
        {
            _dollysClones = GetComponentsInChildren<Dolly>();
            
            allEnemies = new DollyStruct[_dollysClones.Length];

            for (int i = 0; i < allEnemies.Length; i++)
            {
                allEnemies[i].dollyScript = _dollysClones[i];
                allEnemies[i].enemyAvailability = true;
            }
            
            StartAI();
        }
    }
    [System.Serializable]
    public struct DollyStruct
    {
        public Dolly dollyScript;
        public bool enemyAvailability;
    }
}

