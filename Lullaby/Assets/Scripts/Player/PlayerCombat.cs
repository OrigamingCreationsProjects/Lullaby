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
        private DollyManager _enemyManager;
        private PlayerEnemyDetector _enemyDetector;
        private PlayerAnimator _playerAnimator;
        private Player _player;
        [Header("Target")] 
        [SerializeField] private Enemy lockedTarget;

        [Header("Combat Settings")] 
        [SerializeField] private float attackCooldown = 0.7f;
        
        [Header("States")]
        public bool isAttackingEnemy = false;
        //Quiza luego nos conviene referenciar comportamientos externos
        [Header("Public references")]
        [SerializeField] private GameObject lastHitCamera;
        [SerializeField] private Transform lastHitFocusObject;
        
        //Coroutines
        private Coroutine counterCoroutine;
        private Coroutine attackCoroutine;
        private Coroutine damageCoroutine;

        [Space]

        //Events
        public UnityEvent<Enemy> OnHit;
        public UnityEvent<Enemy> OnTrajectory;

        private int _animationCount = 0;
        private string[] attackTriggers;
        
        private bool _dollyModeActive = false;
        
        private void Start()
        {
            _enemyManager = FindObjectOfType<DollyManager>();
            _playerAnimator = GetComponent<PlayerAnimator>();
            _enemyDetector = GetComponentInChildren<PlayerEnemyDetector>();
            _player = GetComponent<Player>();
        }

        public void RegularAttackCheck()
        {
            if(isAttackingEnemy)
                return;
            Debug.Log("Se llega a AttackCheck");
            //Check to see if the detection behavior has an enemy set
            if (_enemyDetector.CurrentTarget() == null)
            {
                //¿Asignamos que no se ataque a nadie?
                Attack(null, 0);
                Debug.Log("No hay target asignado vivos");
                return;
            }
            else
            {
                //Si hay un enemigo asignado, atacamos a ese
                lockedTarget = _enemyDetector.CurrentTarget();
            }

            if (_enemyDetector.GetInputMagnitude() > _player.stats.current.enemyDetectionTreshold)
                lockedTarget = _enemyDetector.CurrentTarget();

            Debug.Log("A la linea de attack se se llega");
            //ATACAMOS AL TONTO QUE TOCA. AGREGAR METODO
            Attack(lockedTarget, TargetDistance(lockedTarget)); // EL DISTANCE LLEVARMELO A METODO
        }
        
        #region -- DOLLYS COMBAT METHODS --

        public void AttackCheck()
        {
            if(isAttackingEnemy)
                return;
            _dollyModeActive = true;
            Debug.Log("Se llega a AttackCheck");
            //Check to see if the detection behavior has an enemy set
            if (_enemyDetector.CurrentTarget() == null)
            {
                //¿Asignamos que no se ataque a nadie?
                Attack(null, 0);
                Debug.Log("No hay target asignado vivos");
                return;
                // if (_enemyManager.GetAliveEnemyCount() == 0)
                // {
                //     //¿Asignamos que no se ataque a nadie?
                //     Attack(null, 0);
                //     Debug.Log("No hay enemigos vivos");
                //     return;
                // }
                // else
                // {
                //     //Si hay enemigos vivos, atacamos a uno aleatorio
                //     lockedTarget = _enemyManager.RandomDolly();
                // }
            }
            else
            {
                //Si hay un enemigo asignado, atacamos a ese
                lockedTarget = _enemyDetector.CurrentTarget();
            }

            if (_enemyDetector.GetInputMagnitude() > _player.stats.current.enemyDetectionTreshold)
                lockedTarget = _enemyDetector.CurrentTarget();

            if (lockedTarget == null)
                lockedTarget = _enemyManager.RandomDolly();
            
            Debug.Log("A la linea de attack se se llega");
            //ATACAMOS AL TONTO QUE TOCA. AGREGAR METODO
            Attack(lockedTarget, TargetDistance(lockedTarget)); // EL DISTANCE LLEVARMELO A METODO
        }
        #endregion

        public void Attack(Enemy target, float distance)
        {
            attackTriggers = new string[] {"AttackCombo1", "AttackCombo2", "AttackCombo3"};
            
            Debug.Log("Entramos en Attack");
            if (target == null)
            {
                AttackType(null, .2f, 0, "AttackCombo1");
                return;
            }

            if (distance < _player.stats.current.maxDistanceToAttack)
            {
                _animationCount = (int)Mathf.Repeat((float)_animationCount + 1, (float)attackTriggers.Length);
                string attackString = attackTriggers[_animationCount];
                Debug.Log($"TRIGGER A LANZAR {attackString}");
                AttackType(target, attackCooldown, .65f, attackString);
            }
            else
            {
                lockedTarget = null;
                AttackType(null, .2f, 0f, "AttackCombo1");
            }
            
            //¿Impulso de camara?
        }

        private void AttackType(Enemy target, float cooldown, float movementDuration, string attackTrigger)
        {
            Animator anim = _player.GetComponentInChildren<Animator>();
            anim.SetBool("Attacking", true);
            anim.SetTrigger(attackTrigger);
            _player.playerEvents.OnAttackStarted.Invoke();
            if(attackCoroutine != null)
                StopCoroutine(attackCoroutine);
            
            attackCoroutine = StartCoroutine(AttackCoroutine(cooldown));

            if (IsLastHit())
                StartCoroutine(FinalBlowCoroutine());
            
            if(target == null)
                return;

            if (target.TryGetComponent(out Dolly d))
            {
                d.StopMoving();
            }
            _player.MoveTowardsTarget(target, movementDuration);
            //MoveTowardsTarget(target, movementDuration);
            OnTrajectory?.Invoke(target);
            IEnumerator AttackCoroutine(float duration)
            {
                //_player.playerEvents.OnAttackStarted?.Invoke();
                isAttackingEnemy = true;
                _player.SetInputEnabled(false);
                yield return new WaitForSeconds(duration);
                isAttackingEnemy = false;
                yield return new WaitForSeconds(0.2f);
                _player.SetInputEnabled(true);
                _player.playerEvents.OnAttackFinished.Invoke();
                anim.SetBool("Attacking", false);
                _dollyModeActive = false;
                //_player.playerEvents.OnAttackFinished?.Invoke();
            }

            IEnumerator FinalBlowCoroutine()
            {
                Time.timeScale = .3f;
                lastHitCamera.SetActive(true);
                lastHitFocusObject.position = lockedTarget.transform.position;
                yield return new WaitForSecondsRealtime(3);
                lastHitCamera.SetActive(false);
                Time.timeScale = 1f;
            }
        }
        
        // protected void MoveTowardsTarget(Enemy target, float duration)
        // {
        //     Debug.Log("Llegamos a instruccion de movimiento");
        //     OnTrajectory?.Invoke(target);
        //     //transform.DOLookAt(target.transform.position, .2f);
        //     //_player.FaceDirectionSmooth(target.transform.position);
        //     FaceToEnemy(target.transform);
        //     transform.DOMove(TargetOffset(target), duration); //.SetEase(Ease.Linear);
        //     //_player.states.Change<AttackPlayerState>();
        // }
        //
        // protected void FaceToEnemy(Transform target)
        // {
        //     var destination = target.position;
        //     var head = destination - transform.position;
        //     var upOffset = Vector3.Dot(transform.up, head); // Sacamos la direccion a la que mirar manteniendo nuestro eje Y
        //    
        //     head -= transform.up * upOffset;
        //     
        //     var distance = head.magnitude;
        //     var direction = head / distance; // Normalizamos porque solo nos interesa la direccion
        //     var localDirection = Quaternion.FromToRotation(transform.up, Vector3.up) * direction;
        //     
        //     _player.FaceDirectionSmooth(localDirection);
        //
        // }
        
        protected float TargetDistance(Enemy target)
        {
            return Vector3.Distance(transform.position, target.transform.position);
        }

        private bool IsLastHit()
        {
            if (lockedTarget == null || !(lockedTarget is Dolly))
                return false;

            return _enemyManager.GetAliveEnemyCount() == 1 &&
                   (lockedTarget.health.current - _player.stats.current.regularAttackDamage) <= 0;
        }
    }
}