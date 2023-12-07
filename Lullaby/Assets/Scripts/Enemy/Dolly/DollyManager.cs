using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lullaby.LevelManagement;
using UnityEngine.Events;
using Random = UnityEngine.Random;

namespace Lullaby.Entities.Enemies
{
    public class DollyManager : MonoBehaviour
    {
        public UnityEvent OnEnemiesDefeated;

        public EnemieStruct[] allEnemies;
        public float minTimeToAttackAgain = 0.1f;
        public float maxTimeToAttackAgain = 3f;
        public Dolly attackingDolly;
        public int aliveDollysCount;
        
        //private Dolly[] _dollysClones;
        private Enemy[] _dollysClones;
        private List<int> enemyIndexes;
        
        private Player _player;
            
        [Header("Main Dolly AI Loop - Settings")]
        private Coroutine _AI_LoopCoroutine;
        
        public int GetAliveEnemiesCount() => aliveDollysCount;
        
        public void StartAI()
        {
            _AI_LoopCoroutine = StartCoroutine(AI_Loop(null));
        }

        IEnumerator AI_Loop(Dolly dolly)
        {
            if (GetAliveEnemiesCount() == 0)
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
            
            if(GetAliveEnemiesCount() > 0)
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
        
        public int CheckAliveEnemyCount()
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
        
        public void DecreaseAliveEnemyCount() => aliveDollysCount--;
        
        public void SetEnemyAvailability(Enemy enemy, bool state)
        {
            for (int i = 0; i < allEnemies.Length; i++)
            {
                if (allEnemies[i].enemyScript == enemy)
                    allEnemies[i].enemyAvailability = state;
            }

            if (!state) DecreaseAliveEnemyCount();

            if (_player.playerEnemyDetector.GetCurrentTarget() == enemy)
                _player.playerEnemyDetector.SetCurrentTarget(null);
            
            if (attackingDolly == enemy)
            {
                StopCoroutine(AI_Loop(null));
                if(GetAliveEnemiesCount() > 0)
                    StartAI();
            }

            if (GetAliveEnemiesCount() <= 0)
            {
                OnEnemiesDefeated?.Invoke();
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

        public void DebugEventLaunched()
        {
            Debug.Log("SE HA LANZADO EL EVENTO DE ENEMIGOS DERROTADOS");
        }
        

        private void Start()
        {
            _dollysClones = GetComponentsInChildren<Enemy>();
            
            allEnemies = new EnemieStruct[_dollysClones.Length];
            _player = Level.instance.player;
            for (int i = 0; i < allEnemies.Length; i++)
            {
                allEnemies[i].enemyScript = _dollysClones[i];
                allEnemies[i].enemyAvailability = true;
                aliveDollysCount++;
            }
            OnEnemiesDefeated.AddListener(DebugEventLaunched);
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

