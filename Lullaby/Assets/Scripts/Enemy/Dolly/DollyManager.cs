using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Lullaby.Entities.Enemies;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Lullaby.Entities.Enemies
{
    public class DollyManager : MonoBehaviour
    {
        public EnemieStruct[] allEnemies;
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
            randomDolly = (Dolly)allEnemies[enemyIndexes[randomIndex]].enemyScript;
            return randomDolly;
        }


        public Dolly RandomDollyExcludingOne(Dolly excludedDolly)
        {
            enemyIndexes = new List<int>();

            for (int i = 0; i < allEnemies.Length; i++)
            {
                if(allEnemies[i].enemyAvailability && allEnemies[i].enemyScript != excludedDolly && allEnemies[i].enemyScript is Dolly)
                    enemyIndexes.Add(i);
            }

            if (enemyIndexes.Count == 0)
            {
                return null;
            }

            Dolly randomDolly;
            int randomIndex = Random.Range(0, enemyIndexes.Count);
            randomDolly = (Dolly)allEnemies[enemyIndexes[randomIndex]].enemyScript;
            return randomDolly;
        }
        
        public int GetAliveEnemyCount()
        {
            int count = 0;
            for (int i = 0; i < allEnemies.Length; i++)
            {
                if (allEnemies[i].enemyScript.isActiveAndEnabled)
                    count++;
            }
            aliveDollysCount = count;
            return count;
        }
        
        public void SetEnemyAvailability(Enemy enemy, bool state)
        {
            for (int i = 0; i < allEnemies.Length; i++)
            {
                if (allEnemies[i].enemyScript == enemy)
                    allEnemies[i].enemyAvailability = state;
            }

            if (FindObjectOfType<PlayerEnemyDetector>().CurrentTarget() == enemy)
                FindObjectOfType<PlayerEnemyDetector>().SetCurrentTarget(null);
            if (attackingDolly == enemy)
            {
                StopCoroutine(AI_Loop(null));
                if(GetAliveEnemyCount() > 0)
                    StartAI();
            }
        }
        
        public bool ADollyIsPreparingAttack()
        {
            foreach (EnemieStruct enemyStruct in allEnemies)
            {
                if (((Dolly)enemyStruct.enemyScript).IsPreparingAttack())
                {
                    return true;
                }
            }
            return false;
        }


        private void Start()
        {
            _dollysClones = GetComponentsInChildren<Dolly>();
            
            allEnemies = new EnemieStruct[_dollysClones.Length];

            for (int i = 0; i < allEnemies.Length; i++)
            {
                allEnemies[i].enemyScript = _dollysClones[i];
                allEnemies[i].enemyAvailability = true;
            }
            
            StartAI();
        }
    }
    [System.Serializable]
    public struct EnemieStruct
    {
        public Enemy enemyScript;
        public bool enemyAvailability;
    }
}

