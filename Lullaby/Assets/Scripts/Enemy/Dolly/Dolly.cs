using System.Collections;
using DG.Tweening;
using Lullaby.Entities.States;
using UnityEngine;

namespace Lullaby.Entities.Enemies
{
    [RequireComponent(typeof(DollyStatsManager))]
    [RequireComponent(typeof(EnemyStateManager))]
    [RequireComponent(typeof(Health))]
    [AddComponentMenu("Lullaby/Enemies/Enemy")]
    public class Dolly : Enemy
    {
        public DollyEvents dollyEvents;

        private float _moveSpeed = 1;
        protected Vector3 moveDirection;
        
        
        public new DollyStatsManager stats { get; protected set; }
        
        /// <summary>
        /// Returns the Dolly Stats Manager instance.
        /// </summary>
        protected override void InitializeStatsManager() => stats = GetComponent<DollyStatsManager>();
        
        #region -- BOOLEANOS PARA LOS ESTADOS IMPLÍCITOS --
        /// <summary>
        /// Return if the enemy is moving.
        /// </summary>
        public bool IsMoving { get; protected set; }

        /// <summary>
        /// Return if the enemy is preparing an attack.
        /// </summary>
        public bool IsPreparingAttack { get; protected set; }
        
        
        /// <summary>
        /// Return if the enemy is retreating.
        /// </summary>
        public bool IsRetreating { get; protected set; }
        
        /// <summary>
        /// Return if the enemy is attacking.
        /// </summary>
        public bool IsLockedTarget { get; protected set; }
        
        /// <summary>
        /// Return if the enemy is stunned.
        /// </summary>
        public bool IsStunned { get; protected set; }
        
        /// <summary>
        /// Return if the enemy is waiting to start the moving.
        /// </summary>
        public bool IsWaiting { get; protected set; }

        #endregion
       
        
        protected Coroutine PrepareAttackCoroutine;
        protected Coroutine RetreatCoroutine;
        protected Coroutine DamageCoroutine;
        protected Coroutine MovementCoroutine;

        // La corrutina que maneja el movimiento de cada enemigo para no sobrecargar el hilo principal.
        protected IEnumerator DollyMovement()
        {
            // Waits until the enemy is not asssigned to no action like attacking or retreating
            yield return new WaitUntil(() => IsWaiting == true);

            //Probabilidad 50/50 de que el enemigo se mueva o no
            int randomChance = Random.Range(0, 2);

            if (randomChance == 1)
            {
                // Probabilidad 50/50 de que el enemigo se mueva a la derecha o a la izquierda
                int randomDir = Random.Range(0, 2);
                moveDirection = randomDir == 1 ? Vector3.right : Vector3.left;
                IsMoving = true; // O cambiamos explicitamente de estado
            }
            else
            {
                StopMoving();
            }

            yield return new WaitForSeconds(stats.current.timeBeforeMoveAgain);
            
            MovementCoroutine = StartCoroutine(DollyMovement());
        }

        public void StopMoving()
        {
            IsMoving = false;
            moveDirection = Vector3.zero;
            if(controller.enabled)
                controller.Move(moveDirection); // CUIDADO CON ESTA LINEA
        }

        protected virtual void MoveDolly(Vector3 direction)
        {
            //Set move speed based on the given direction
            _moveSpeed = 1;

            if (direction == Vector3.forward)
                _moveSpeed = stats.current.forwardSpeed;
            else if (direction == -Vector3.forward)
                _moveSpeed = stats.current.retreatSpeed;
            
            //If isMoving is false we dont want to do anything
            if (!IsMoving) return;
            
            Vector3 dir = (_player.transform.position - transform.position).normalized;
            Vector3 pDir = Quaternion.AngleAxis(90, Vector3.up) * dir;
            Vector3 moveDir = Vector3.zero;

            Vector3 finalDir = Vector3.zero;

            if (direction == Vector3.forward)
                finalDir = dir;
            else if (direction == Vector3.right || direction == Vector3.left)
            {
                finalDir = (pDir * direction.normalized.x);
                _moveSpeed /= 1.5f; // Dividimos la velocidad entre 1.5 para que no sea tan rapido al moverse lateralmente
            }
            else if (direction == -Vector3.forward)
                finalDir = -transform.forward;
            
            moveDir += finalDir * _moveSpeed * Time.deltaTime;
            
            controller.Move(moveDir);
            
            if(!IsPreparingAttack) 
                return;

            if (Vector3.Distance(transform.position, _player.transform.position) < stats.current.minDistanceToRetreat)
            {
                StopMoving();
                if(!_player.states.IsCurrentOfType(typeof(AttackPlayerState)))
                   Attack();
                else
                    PrepareAttack(false);
            }
        }

        protected virtual void Attack()
        {
            transform.DOMove(transform.position + (transform.forward / 1), stats.current.attackMovementDuration);
            //Lanzar trigger de ataque o puño o lo que sea del animator
            
        }

        protected virtual void PrepareAttack(bool active)
        {
            IsPreparingAttack = active;

            if (active)
            {
                //Particula? Shake?
            }
            else
            {
                StopMoving();
                //Parar particula?
            }
            
        }
        
        
        protected override void Awake()
        {
            InitializeController();
            InitializeStateManager();
            InitializeParent();
            InitializeTag();
            InitializeStatsManager();
            InitializeHealth();
        }
    }
}