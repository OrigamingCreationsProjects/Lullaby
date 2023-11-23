using System;
using DG.Tweening;
using Lullaby.Entities.Enemies;
using UnityEngine;
using UnityEngine.Events;

namespace Lullaby.Entities
{
    [AddComponentMenu("Lullaby/Combat System/Player Combat")]
    public class PlayerCombat : MonoBehaviour
    {
        private DollyManager enemyManager;
        private PlayerEnemyDetector _enemyDetector;
        private PlayerAnimator _playerAnimator;
        private Player _player;
        [Header("Target")] 
        private Enemy lockedTarget;

        [Header("Combat Settings")] 
        private float attackCooldown;
        
        //Quiza luego nos conviene referenciar comportamientos externos
        
        //Coroutines
        private Coroutine counterCoroutine;
        private Coroutine attackCoroutine;
        private Coroutine damageCoroutine;

        [Space]

        //Events
        public UnityEvent<Enemy> OnHit;


        private void Start()
        {
            enemyManager = FindObjectOfType<DollyManager>();
            _playerAnimator = GetComponent<PlayerAnimator>();
            _enemyDetector = GetComponent<PlayerEnemyDetector>();
            _player = GetComponent<Player>();
        }

        public void AttackCheck()
        {
            //Check to see if the detection behavior has an enemy set
            if (_enemyDetector.CurrentTarget() == null)
            {
                if (enemyManager.GetAliveEnemyCount() == 0)
                {
                    //¿Asignamos que no se ataque a nadie?
                    Debug.Log("No hay enemigos vivos");
                    return;
                }
                else
                {
                    //Si hay enemigos vivos, atacamos a uno aleatorio
                    lockedTarget = enemyManager.RandomDolly();
                }
            }

            if (_enemyDetector.GetInputMagnitude() > _player.stats.current.enemyDetectionTreshold)
                lockedTarget = _enemyDetector.CurrentTarget();

            if (lockedTarget == null)
                lockedTarget = enemyManager.RandomDolly();
            
            //ATACAMOS AL TONTO QUE TOCA. AGREGAR METODO
            Attack(lockedTarget, Vector3.Distance(transform.position, lockedTarget.position)); // EL DISTANCE LLEVARMELO A METODO
        }

        public void Attack(Enemy target, float distance)
        {
            MoveTowardsTarget(target, 0.1f);
        }

        protected void MoveTowardsTarget(Enemy target, float duration)
        {
            transform.DOLookAt(target.transform.position, .2f);
            transform.DOMove(target.transform.position, duration); //.SetEase(Ease.Linear);
        }
    }
}