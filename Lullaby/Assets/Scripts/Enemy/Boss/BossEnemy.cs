using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lullaby.Entities.Enemies.States;
using Lullaby.Entities.Events;
using Lullaby.Entities.States;
using OpenCover.Framework.Model;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Lullaby.Entities.Enemies
{
    [RequireComponent(typeof(BEStateManager))]
    [RequireComponent(typeof(BEStatsManager))]
    [AddComponentMenu("Lullaby/Enemies/BossEnemy")]
    public class BossEnemy : Entity<BossEnemy>
    {
        [Header("Parameters"), Tooltip("Set the time to rest the IEnumerator before starting the loop again.")]
        public int moveCoroutineHold = 1;
        public float yAngle;
        [Header("Prefab Assignment")] [SerializeField]
        public GameObject bulletPrefab;

        public bool enemyAttacking;
        [SerializeField] private GameObject body;
        [SerializeField] private GameObject model;

        /// <summary>
        /// Returns the Boss Enemy Stats Manager instance.
        /// </summary>
        public BEStatsManager stats { get; protected set; }

        /// <summary>
        /// Returns the instance of the player
        /// </summary>
        public Player player { get; protected set; }

        public BossEntityManager bossManager { get; protected set; }
        public Transform pivot { get; protected set; }
        /// <summary>
        /// Returns the moving direction for the entity.
        /// </summary>

        [HideInInspector] public Transform slot;


        public Health health { get; protected set; }
        public bool retreating = false;
        public Coroutine MovementCoroutine;

        public static BossEnemy MainBoss;
        [HideInInspector] public BossStages stage = BossStages.FirstStage;
        [HideInInspector] public bool IsInvincible = true;
        public int numBullets = 4;

        /// <summary>
        /// Returns collider buffer storing entities in sight.
        /// </summary>
        protected Collider[] sightOverlaps = new Collider[1024];

        public BossEnemyEvents enemyEvents;
        private List<BulletBehaviour> bullets;
        public Rotate rotateAnimComponent;
        public bool step = true;
        public int index { get; private set; }
        [HideInInspector] public bool invoked = false;

        #region -- INITIALIZERS --

        protected virtual void InitializeStatsManager() => stats = GetComponent<BEStatsManager>();
        protected virtual void InitializeHealth() => health = GetComponent<Health>();

        protected virtual void InitializeBody()
        {
            if (body == null) body = transform.GetChild(1).gameObject;
        }

        protected virtual void InitializeBossManager() => bossManager = GetComponentInParent<BossEntityManager>();

        protected virtual void InitializeMainBoss()
        {
            if (MainBoss == null) MainBoss = this;
        }

        protected virtual void InitializePlayer() => player = FindObjectOfType<Player>();
        protected virtual void InitializeRotationComponent() => rotateAnimComponent = GetComponentInChildren<Rotate>();
        
        #endregion
        #region EVENT HANDLERS

   
        #endregion
        #region -- MONOBEHAVIOURS --

        protected override void Awake()
        {
            base.Awake();
            InitializeMainBoss();
            InitializeBossManager();
            InitializeStatsManager();
            InitializeHealth();
            InitializeBody();
            InitializePlayer();
            InitializeRotationComponent();
        }

        void Start()
        {
            CreateBullet();
            if (MainBoss != this) return;
            health.Set(300);
        }

        protected override void Update()
        {
            base.Update();
            LookAtPlayer(model.transform);
            LookAtPlayerSmooth(transform);
        }
        protected override void OnUpdate()
        {
            if (MainBoss != this) return;
            DetectPlayer();
            SetStages();
            
        }

        protected void OnTriggerEnter(Collider other)
        {
            ContactAttack(other, controller.bounds);

        }

        protected void OnEnable()
        {
           // controller.enabled = true;
        }

        #endregion

        #region Getters&Setters

        public virtual Transform GetBody() { return body.transform; }    
        #endregion
        #region -- PARENT OVERRIDES --

        public virtual void FaceDirectionSmooth(Vector3 direction) => FaceDirectionSmooth(direction, stats.current.rotationSpeed);
        public virtual void Accelerate(Vector3 direction, float acceleration, float topSpeed) =>
            Accelerate(direction, stats.current.turningDrag, acceleration, topSpeed);
        public virtual void Decelerate() => Decelerate(stats.current.deceleration);
        
        #endregion
        #region -- PARTICULAR FUNCTIONS --
        
        /// <summary>
        /// [UnityEngine] Rotates the entity to face the player. Uses legacy transform.LookAt.
        /// </summary>
        private void LookAtPlayer(Transform obj) // Consider adding parameters for target and making the function protected for inheritance.
        {
            //if(player) obj.LookAt(player.position,Vector3.up); 
            var head = player.position - obj.position;
            var upOffset = Vector3.Dot(obj.up, head);
            var direction = head - obj.up * upOffset; 
            var localDirection = Quaternion.FromToRotation(obj.up, Vector3.up) * direction; 
            
            direction = localDirection.normalized;
            if (direction != Vector3.zero)
            {
                direction = Quaternion.FromToRotation(Vector3.up, obj.up) * direction; // Transformamos la direccion al espacio local
                var rotation = obj.rotation;
                var rotationDelta = stats.current.rotationSpeed * Time.deltaTime; // Calculamos el maximo de rotacion por frame
                var target = Quaternion.LookRotation(direction, obj.up); // Calculamos la rotacion objetivo
                obj.rotation = Quaternion.RotateTowards(rotation, target, rotationDelta); // Rotamos el personaje suavemente
            }
        }
    
        /// <summary>
        /// [Recommended] Rotates the entity to face the player. Uses custom made FaceDirectionSmooth function.
        /// </summary>
        private void LookAtPlayerSmooth(Transform obj)
        {
            if (!player) return;
            //if(states.current is BEAttackingState) return; 
            var head = player.position - obj.position;
            var upOffset = Vector3.Dot(obj.up, head);
            var direction = head - obj.up * upOffset; 
            var localDirection = Quaternion.FromToRotation(obj.up, Vector3.up) * direction; 
            
            localDirection = localDirection.normalized;
            
            FaceDirectionSmooth(localDirection);
        }

        private void CreateBullet()
        {
            bullets = new List<BulletBehaviour>();
            for (int i = 0; i < numBullets; i++)
            {
                var bullet = Instantiate(bulletPrefab, transform.position, transform.rotation,null);
                bullet.SetActive(false);
                bullet.GetComponent<MeshRenderer>().material = body.GetComponent<MeshRenderer>().material;
                var bulletScript = bullet.GetComponent<BulletBehaviour>();
                bulletScript.parentBoss = this;
                bullets.Add(bulletScript);
            }
        }

        public void ShootBullet(int idx)
        {
            for (int i = idx%bullets.Count(); i < bullets.Count(); i++)
            {
                if (bullets[i].shot) {index = i; continue;} 
                bullets[i].gameObject.SetActive(true);
                index++;
                return;
            }
        }

        private void SetStages()
        {
           
            if (health.current > 200) stage = BossStages.FirstStage;
            if (health.current > 100 && health.current < 200) stage = BossStages.SecondStage;
            if (health.current < 100) stage = BossStages.FinalStage;
        }

        public void Disable()
        {
            step = false;
            //enabled = false;
            controller.enabled = false;
            //rotateAnimComponent.enabled = false;
        }
        #endregion
        
        #region -- COROUTINES --
        

        void DetectPlayer()
        {
            if (invoked) return;
            var distanceToPlayer = (player.position - position).magnitude;
            if (distanceToPlayer < stats.current.spotRange)
            {
                enemyEvents.HandlePlayerSeen(true);
                invoked = true;
            }
            
        }

       
        #endregion 
        

        public void SetController(bool value)
        {
            controller.enabled = value;
        }
        
        public virtual void ContactAttack(Collider other, Bounds bounds, BulletBehaviour bullet = null)
        {
            if(!other.CompareTag(GameTags.Player)) return;
            if(!other.TryGetComponent(out Player player)) return;

            var stepping = bounds.max + Vector3.down * stats.current.contactSteppingTolerance; // Posicion del enemigo en el suelo

            if (player.isGrounded || !BoundsHelper.IsBellowPoint(controller.collider, stepping)) // Si el jugador esta en el suelo y no está por encima del enemigo
            {
                if (bullet)
                {
                    var dir = (transform.position - player.transform.position).normalized;
                    bullet.speed = bullet.stats.reboundSpeed;
                    bullet.followBoss = true;
                    bullet.SetMovingDirection(dir);
                    player.ApplyDamage(stats.current.contactDamage, transform.position);
                }
                else
                {
                    if (stats.current.contactPushback) // Si puede mandar para atras al jugador
                    lateralVelocity = -localForward * stats.current.contactPushBackForce; // Empujamos al jugador hacia atras
                    player.ApplyDamage(stats.current.contactDamage, transform.position);
                    enemyEvents.OnPlayerContact?.Invoke();
                }
            }
        }

        public void CheckStage()
        {
            if (health.current <= stats.current.secondStageThreshold && health.current > stats.current.finalStageThreshold)
            {
                enemyEvents.HandleSecondStage();
            } else if (health.current <= stats.current.finalStageThreshold && health.current > 0)
            {
               enemyEvents.HandleFinalStage();
            }
        }

        public void UpdateInvincibilityStatus()
        {
            IsInvincible = true;
        }
        public void ApplyDamage(int amount)
        {
            if (!health.isEmpty && !health.recovering && !IsInvincible)
            {
                health.Damage(amount);
                Debug.Log($"{name} HAS BEEN DAMAGED, CURRENT HEALTH IS {health.current}");
                enemyEvents.OnDamage?.Invoke();

                if (health.isEmpty)
                {
                    enemyEvents.HandleAttack(false);
                    controller.enabled = false;
                    bossManager.RemoveBossFromBuffer(this);
                    bossManager.CheckInvincibilityStatus();
                    //this.enabled = false;
                    this.gameObject.SetActive(false);
                    enemyEvents.OnDie?.Invoke();
                }
            }

            if (IsInvincible)
            {
             // bossManager.InstantiateBosses(bossManager.AliveBossCount());
              bossManager.ReviveBosses();
              bossManager.DisableBossGameObjects();
              bossManager.Retreat();
             
               
            }
        }

     

    }

    public enum BossStages
    {
        FirstStage, SecondStage, FinalStage
    }
}
