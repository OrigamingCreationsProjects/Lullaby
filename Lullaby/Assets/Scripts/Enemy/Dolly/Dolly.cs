using System;
using System.Collections;
using DG.Tweening;
using Lullaby.Entities.States;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lullaby.Entities.Enemies
{
    [RequireComponent(typeof(DollyStatsManager))]
    [RequireComponent(typeof(EnemyStateManager))]
    [RequireComponent(typeof(Health))]
    [AddComponentMenu("Lullaby/Enemies/Dolly")]
    public class Dolly : Enemy
    {
        public DollyEvents dollyEvents;

        public DollyStatsManager dollyStats { get; protected set; }
        
        
        private float _moveSpeed = 1;
        protected Vector3 moveDirection;

        [Header("States")]
        [SerializeField] private bool isPreparingAttack;
        [SerializeField] private bool isMoving;
        [SerializeField] private bool isRetreating;
        [SerializeField] private bool isLockedTarget;
        [SerializeField] private bool isStunned;
        [SerializeField] private bool isWaiting = true;
        
        
        private DollyManager _dollyManager;
        
        /// <summary>
        /// Returns the Dolly Stats Manager instance.
        /// </summary>
        protected override void InitializeStatsManager() => dollyStats = GetComponent<DollyStatsManager>();
        
        public Vector3 MoveDirection => moveDirection;
        
        /// <summary>
        /// Applies a downward force by its gravity stats.
        /// </summary>
        public override void ApplyGravity() => ApplyGravity(dollyStats.current.gravity);

        /// <summary>
        /// Applies a downward force when ground by its snap stats.
        /// </summary>
        public override void SnapToGround() => SnapToGround(dollyStats.current.snapForce);
        
        
        #region -- BOOLEANOS PARA LOS ESTADOS IMPLÍCITOS --

        /// <summary>
        /// Return if the enemy is moving.
        /// </summary>
        public bool IsMoving()
        {
            return isMoving;
        }

        public bool IsAttackable()
        {
            return health.current > 0;
        }

        /// <summary>
        /// Return if the enemy is preparing an attack.
        /// </summary>
         public bool IsPreparingAttack()
        {
            return isPreparingAttack;
        }
        
        
        /// <summary>
        /// Return if the enemy is retreating.
        /// </summary>
        public bool IsRetreating()
        {
            return isRetreating;
        }
        /// <summary>
        /// Return if the enemy is attacking.
        /// </summary>
        public bool IsLockedTarget()
        {
            return isLockedTarget;
        }
        
        /// <summary>
        /// Return if the enemy is stunned. (Receiving an attack)
        /// </summary>
        public bool IsStunned()
        {
            return isStunned;
        }

        /// <summary>
        /// Return if the enemy is waiting to start the moving.
        /// </summary>
        public bool IsWaiting()
        {
            return isWaiting;
        }

        #endregion

        #region -- STATES COROUTINES

            protected Coroutine PrepareAttackCoroutine;
            protected Coroutine RetreatCoroutine;
            protected Coroutine DamageCoroutine;
            protected Coroutine MovementCoroutine;
            
        #endregion

        // La corrutina que maneja el movimiento de cada enemigo para no sobrecargar el hilo principal.
        protected IEnumerator DollyMovement()
        {
            // Waits until the enemy is not asssigned to no action like attacking or retreating
            yield return new WaitUntil(() => isWaiting == true);

            //Probabilidad de que el enemigo se mueva o no
            int randomChance = Random.Range(0, 10);

            if (randomChance <= dollyStats.current.chanceToMoveLaterally)
            {
                // Probabilidad 50/50 de que el enemigo se mueva a la derecha o a la izquierda
                int randomDir = Random.Range(0, 2);
                moveDirection = randomDir == 1 ? Vector3.right : Vector3.left;
                isMoving = true; // O cambiamos explicitamente de estado
            }
            else
            {
                StopMoving();
            }

            yield return new WaitForSeconds(dollyStats.current.timeBeforeMoveAgain);
            
            MovementCoroutine = StartCoroutine(DollyMovement());
        }

        public void StopMoving()
        {
            isMoving = false;
            moveDirection = Vector3.zero;
            if(controller.enabled)
                controller.Move(moveDirection); // CUIDADO CON ESTA LINEA
        }

        public virtual void MoveDolly(Vector3 direction)
        {
            //Set move speed based on the given direction
            _moveSpeed = 1;

            if (direction == Vector3.forward)
                _moveSpeed = dollyStats.current.forwardSpeed;
            else if (direction == -Vector3.forward)
                _moveSpeed = dollyStats.current.retreatSpeed;
            
            //If isMoving is false we dont want to do anything
            if (!isMoving) return;
            Vector3 dir = (player.gameObject.transform.position - transform.position).normalized;
            Vector3 pDir = Quaternion.AngleAxis(90, Vector3.up) * dir;
            Vector3 moveDir = Vector3.zero;

            Vector3 finalDir = Vector3.zero;

            if (direction == Vector3.forward)
                finalDir = dir;
            else if (direction == Vector3.right || direction == Vector3.left)
            {
                finalDir = (pDir * direction.normalized.x);
                _moveSpeed = dollyStats.current.forwardSpeed * dollyStats.current.lateralSpeedMultiplier; // Dividimos la velocidad entre 1.5 para que no sea tan rapido al moverse lateralmente
            }
            else if (direction == -Vector3.forward)
                finalDir = -transform.forward;
            
            moveDir += finalDir * _moveSpeed * Time.deltaTime;
            
            controller.Move(moveDir);
            
            if(!isPreparingAttack) 
                return;

            if (Vector3.Distance(transform.position, player.transform.position) < dollyStats.current.minDistanceToAttack)
            {
                StopMoving();
                if(!player.states.IsCurrentOfType(typeof(AttackPlayerState)))
                    Attack();
                else
                    PrepareAttack(false);
            }
        }

        protected virtual void Attack()
        {
            transform.DOMove(transform.position + (transform.forward / 1), dollyStats.current.attackMovementDuration);
            //Lanzar trigger de ataque o puño o lo que sea del animator
            Debug.Log("Dolly Attack");
        }

        public void SetRetreat()
        {
            StopActiveCoroutines();
            RetreatCoroutine = StartCoroutine(PrepRetreat());

            IEnumerator PrepRetreat()
            {
                yield return new WaitForSeconds(dollyStats.current.retreatPreparationTime);
                dollyEvents.OnRetreat?.Invoke(this);
                isRetreating = true;
                moveDirection = -Vector3.forward;
                isMoving = true;
                yield return new WaitUntil(() => Vector3.Distance(transform.position, player.transform.position) > dollyStats.current.minDistanceToStopRetreating);
                isRetreating = false;
                StopMoving();

                isWaiting = true;
                MovementCoroutine = StartCoroutine(DollyMovement());
            }
        }
        
        void Death()
        {
            StopActiveCoroutines();

            this.enabled = false;
            controller.enabled = false;
            //animator.SetTrigger("Death");
            _dollyManager.SetEnemyAvailability(this, false);
        }
        
        public void SetAttack()
        {
            isWaiting = false;
            
            PrepareAttackCoroutine = StartCoroutine(PrepAttack());
            
            IEnumerator PrepAttack()
            {
                PrepareAttack(true);
                yield return new WaitForSeconds(dollyStats.current.attackPreparationTime);
                moveDirection = Vector3.forward;
                isMoving = true;
            }
        }
        
        protected virtual void PrepareAttack(bool active)
        {
            isPreparingAttack = active;

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

        public override void ApplyDamage(int amount, Vector3 origin)
        {
            if (!health.isEmpty && !health.recovering)
            {
                health.Damage(amount);
                enemyEvents.OnDamage?.Invoke();

                //Debug.Log("Enemigo dañado");
                if (health.isEmpty)
                {
                    //controller.enabled = false;
                    enemyEvents.OnDie?.Invoke();
                    //states.Change<DieEnemyState>();
                    //gameObject.SetActive(false);
                    Death();
                }
                else
                {
                    //states.Change<HurtEnemyState>();
                }
            }
        }
        
        protected virtual void StopActiveCoroutines()
        {
            PrepareAttack(false);

            if (isRetreating)
            {
                if(RetreatCoroutine != null)
                    StopCoroutine(RetreatCoroutine);
            }
            
            if(PrepareAttackCoroutine != null)
                StopCoroutine(PrepareAttackCoroutine);
            
            if(DamageCoroutine != null)
                StopCoroutine(DamageCoroutine);
            
            if(MovementCoroutine != null)
                StopCoroutine(MovementCoroutine);
        }

        #region -- ANIMATION AND PLAYER LISTENER EVENTS -- (Quiza se puedan mover a otro sitio)

        public void HitEvent()
        {
            if (!player.states.IsCurrentOfType(typeof(AttackPlayerState)))
                player.ApplyDamage(dollyStats.current.attackDamage, transform.position);
            
            PrepareAttack(false);
        }

        //Listened event from Player Animation
        
        void OnPlayerHit(Dolly target)
        {
            if (target == this)
            {
                StopActiveCoroutines();
                DamageCoroutine = StartCoroutine(HitCoroutine());
                
                //Enemigo actual detectado == null?
                isLockedTarget = false;
                dollyEvents.OnDamage?.Invoke(this);
                
                ApplyDamage(player.stats.current.regularAttackDamage, player.transform.position);
                
                //ANIMATOR TRIGGER DE HIT?
                transform.DOMove(transform.position - (transform.forward / 2),
                    dollyStats.current.damageMovementDuration).SetDelay(dollyStats.current.damageMovementDelay);
                
                StopMoving();

                IEnumerator HitCoroutine()
                {
                    isStunned = true;
                    yield return new WaitForSeconds(.5f);
                    isStunned = false;
                }
            }
        }
        
        #endregion
        protected override void OnUpdate()
        {
            
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
        
        protected void Start()
        {
            player = FindObjectOfType<Player>();
            Debug.Log($"Se asigna al player {player.name}");
            MovementCoroutine = StartCoroutine(DollyMovement());
        }
        
        protected override void OnTriggerEnter(Collider other)
        {
            //ContactAttack(other);
            if (other.CompareTag(GameTags.Player))
            {
                PrepareAttack(false);
            }
        }
    }
}