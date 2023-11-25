using System;
using System.Collections;
using DG.Tweening;
using Lullaby.Entities.Enemies;
using Lullaby.Entities.States;
using Unity.VisualScripting;
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
        [SerializeField] private Enemy lockedTarget;

        [Header("Combat Settings")] 
        [SerializeField] private float attackCooldown;
        
        [Header("States")]
        public bool isAttackingEnemy = false;
        //Quiza luego nos conviene referenciar comportamientos externos
        
        //Coroutines
        private Coroutine counterCoroutine;
        private Coroutine attackCoroutine;
        private Coroutine damageCoroutine;

        [Space]

        //Events
        public UnityEvent<Enemy> OnHit;
        public UnityEvent<Enemy> OnTrajectory;

        private void Start()
        {
            enemyManager = FindObjectOfType<DollyManager>();
            _playerAnimator = GetComponent<PlayerAnimator>();
            _enemyDetector = GetComponentInChildren<PlayerEnemyDetector>();
            _player = GetComponent<Player>();
        }

        public void AttackCheck()
        {
            if(isAttackingEnemy)
                return;
            Debug.Log("Se llega a AttackCheck");
            //Check to see if the detection behavior has an enemy set
            if (_enemyDetector.CurrentTarget() == null)
            {
                if (enemyManager.GetAliveEnemyCount() == 0)
                {
                    //¿Asignamos que no se ataque a nadie?
                    Attack(null, 0);
                    Debug.Log("No hay enemigos vivos");
                    return;
                }
                else
                {
                    //Si hay enemigos vivos, atacamos a uno aleatorio
                    lockedTarget = enemyManager.RandomDolly();
                }
            }
            else
            {
                //Si hay un enemigo asignado, atacamos a ese
                lockedTarget = _enemyDetector.CurrentTarget();
            }

            if (_enemyDetector.GetInputMagnitude() > _player.stats.current.enemyDetectionTreshold)
                lockedTarget = _enemyDetector.CurrentTarget();

            if (lockedTarget == null)
                lockedTarget = enemyManager.RandomDolly();
            
            Debug.Log("A la linea de attack se se llega");
            //ATACAMOS AL TONTO QUE TOCA. AGREGAR METODO
            Attack(lockedTarget, TargetDistance(lockedTarget)); // EL DISTANCE LLEVARMELO A METODO
        }

        public void Attack(Enemy target, float distance)
        {
            Debug.Log("Entramos en Attack");
            if (target == null)
            {
                AttackType(null, .2f, 0);
                return;
            }

            if (distance < 7)
            {
                AttackType(target, attackCooldown, .65f);
            }
            else
            {
                lockedTarget = null;
                AttackType(null, .2f, 0f);
            }
            
            //¿Impulso de camara?
        }

        private void AttackType(Enemy target, float cooldown, float movementDuration)
        {
            if(attackCoroutine != null)
                StopCoroutine(attackCoroutine);
            
            attackCoroutine = StartCoroutine(AttackCoroutine(cooldown));
            
            if(target == null)
                return;

            if (target.TryGetComponent(out Dolly d))
            {
                d.StopMoving();
            }
            MoveTowardsTarget(target, movementDuration);
           
            IEnumerator AttackCoroutine(float duration)
            {
                //_player.playerEvents.OnAttackStarted?.Invoke();
                isAttackingEnemy = true;
                _player.inputs.enabled = false;
                yield return new WaitForSeconds(duration);
                isAttackingEnemy = false;
                yield return new WaitForSeconds(0.2f);
                _player.inputs.enabled = true;
               //_player.playerEvents.OnAttackFinished?.Invoke();
            }
        }
        
        protected void MoveTowardsTarget(Enemy target, float duration)
        {
            Debug.Log("Llegamos a instruccion de movimiento");
            OnTrajectory?.Invoke(target);
            //transform.DOLookAt(target.transform.position, .2f);
            //_player.FaceDirectionSmooth(target.transform.position);
            FaceToEnemy(target.transform);
            transform.DOMove(TargetOffset(target), duration); //.SetEase(Ease.Linear);
            //_player.states.Change<AttackPlayerState>();
        }

        protected void FaceToEnemy(Transform target)
        {
            var destination = target.position;
            var head = destination - transform.position;
            var upOffset = Vector3.Dot(transform.up, head); // Sacamos la direccion a la que mirar manteniendo nuestro eje Y
           
            head -= transform.up * upOffset;
            
            var distance = head.magnitude;
            var direction = head / distance; // Normalizamos porque solo nos interesa la direccion
            var localDirection = Quaternion.FromToRotation(transform.up, Vector3.up) * direction;
            
            _player.FaceDirectionSmooth(localDirection);

        }
        
        protected float TargetDistance(Enemy target)
        {
            return Vector3.Distance(transform.position, target.transform.position);
        }
        
        public Vector3 TargetOffset(Enemy target)
        {
            Vector3 position;
            position = target.position;
            return Vector3.MoveTowards(position, transform.position, .95f);
        }
    }
}