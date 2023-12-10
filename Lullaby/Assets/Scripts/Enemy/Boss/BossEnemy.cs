using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using Lullaby.Entities.Enemies.States;
using Lullaby.Entities.Events;
using Lullaby.Entities.States;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Lullaby.Entities.Enemies
{
    [RequireComponent(typeof(BEStateManager))]
    [RequireComponent(typeof(BEStatsManager))]
    [AddComponentMenu("Lullaby/Enemies/BossEnemy")]
    public class BossEnemy : Entity<BossEnemy>
    {
      
        [Header("Prefab Assignment")]
        public GameObject bulletPrefab;
        public Color bulletColor;
        public Gradient smokeGradient;
        [SerializeField] private GameObject body;
        [SerializeField] public GameObject model;
        
        public Animator animator;
        public Collider hitCollider;
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
        public Coroutine MovementCoroutine;

        public static BossEnemy MainBoss;
        public BossStages stage = BossStages.FirstStage;
        //[HideInInspector]
        public bool IsInvincible = true;
        [HideInInspector]public bool InPlace = false;
        public int numBullets = 4;

        /// <summary>
        /// Returns collider buffer storing entities in sight.
        /// </summary>
        protected Collider[] sightOverlaps = new Collider[1024];

        public BossEnemyEvents enemyEvents;
        private List<BulletBehaviour> bullets;
        public Rotate rotateAnimComponent;
        public bool step = false;
        public int index { get; private set; }
        [HideInInspector] public bool invoked = false;
        public bool controlled = true;
        public Vector3 moveDirection { get; private set; }
        [HideInInspector] public float angleAssigned;
        [HideInInspector] public Vector2 dirFacing;
        
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
        protected virtual void InitializeAnimator() => animator = GetComponentInChildren<Animator>();
        protected virtual void InitializeRotationComponent() => rotateAnimComponent = GetComponentInChildren<Rotate>();
        
        protected virtual void InitializeParametersHash()
        {
            _cloningHash = Animator.StringToHash(cloningName);
            _shootHash = Animator.StringToHash(shootName);
            _turningHash = Animator.StringToHash(turningName);
        }
        
        #endregion

        #region -- ANIMATIONS --
        [Header("Parameters Names")] 
        public string cloningName = "Cloning";
        public string shootName = "Shoot";
        public string turningName = "TurningBitch";
        
        //HASHES
        protected int _cloningHash;
        protected int _shootHash;
        protected int _turningHash;


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
            InitializeAnimator();
            InitializeParametersHash();
        }

        void Start()
        {
            CreateBullet();
            EnableCollider(false);
            if (MainBoss != this) return;
            health.Set(30);
        }

        protected override void Update()
        {
            HandleStates();
            HandleController();
           
            //HandleContacts();
            OnUpdate();
        }
        protected override void OnUpdate()
        {
            LookAtPlayer(model.transform);
            //LookAtPlayerSmooth(transform);
            if (MainBoss != this) return;
            DetectPlayer();
            
        }

        private void OnTriggerEnter(Collider other)
        {
           states.current.OnContact(this, other);   
        }

        #endregion
        private string _customColorPropertyName = "_CustomizableColor";
        #region Getters&Setters

        public virtual Transform GetBody() { return body.transform; }

        public void EnableCollider(bool value)
        {
            controller.collider.enabled = value;
        }
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
                bullet.GetComponent<MeshRenderer>().material.color = body.GetComponent<SkinnedMeshRenderer>().material.GetColor(_customColorPropertyName);
                var bulletScript = bullet.GetComponent<BulletBehaviour>();
                bulletScript.SetParentBoss(this);
                bullets.Add(bulletScript);
            }
        }

        public void ShootBullet()
        {
            BulletBehaviour bullet = bullets[0];
            for (int i = 0; i < bullets.Count(); i++)
            {
                if (bullets[i].shot) {index = i; continue;} 
                //bullets[i].gameObject.SetActive(true);
               
                bullet = bullets[i];
               
                index++;
                break;
            }
            bullet.SetParticleColor(bulletColor);
            Sequence s = DOTween.Sequence();
            s.AppendCallback(() => animator.SetTrigger(_shootHash));
            s.AppendInterval(0.5f);
            s.AppendCallback(() => bullet.ChangeActiveState(true));
            // animator.SetTrigger(_shootHash);
            // bullet.ChangeActiveState(true);
        }
        

        public void Disable()
        {
            step = false;
            if (stage != BossStages.FirstStage)
            {
                foreach (var bullet in bullets)
                {
                    bullet.gameObject.SetActive(false);
                }
            }
            StopAllCoroutines();
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
                step = true;
            }
            
        }

        public IEnumerator EnemyMovement(Vector3 movement = default)
        { 
            var platformPos = new Vector2(bossManager.fightPlatform.position.x,
                bossManager.fightPlatform.position.z);
            while (true)
            {
                var enemyPos = new Vector2(position.x, position.z);
               
                var dirToEnemy = (enemyPos - platformPos).normalized;

                if (moveDirection == default)
                {
                    int randomDir = Random.Range(0, 2);
                    moveDirection = randomDir == 1 ? Vector3.right : Vector3.left;
                }
                else
                {

                    while (InsideZone(dirToEnemy)) yield return null;
                    moveDirection *= -1f;
               
                }
            }
            
        }
        #endregion

        public void DivideCloneAnim()
        {
            animator.SetTrigger(_cloningHash);
        }  
        public void TurningCloneAnim()
        {
            animator.SetTrigger(_turningHash);
        }
        
        public bool InsideZone(Vector2 dirToEnemy)
        {
            if (!controlled && Vector2.Angle(dirFacing, dirToEnemy) > (angleAssigned - stats.current.angleOffset) / 2)
            {
                controlled = true;
                return false;
            }  
            if (Vector2.Angle(dirFacing, dirToEnemy) < (angleAssigned - stats.current.angleOffset) / 2 )
            {   
                controlled = false;
            }

            return true;
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
                    //bullet.Rebound();
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
                stage = BossStages.SecondStage;
                enemyEvents.HandleSecondStage();
                
            } else if (health.current <= stats.current.finalStageThreshold && health.current > 0)
            { 
                stage = BossStages.FinalStage;
                enemyEvents.HandleFinalStage();
            }
        }

        public void UpdateInvincibilityStatus()
        {
            IsInvincible = true;
        }
        public override void ApplyDamage(int amount, Vector3 origin)
        {
            //Debug.Log("Entramos en ApplyDamage");
            bossManager.StopAllCoroutines();
            if (!health.isEmpty && !health.recovering && !IsInvincible)
            {
                health.Damage(amount);
                enemyEvents.OnDamage?.Invoke();
                //Debug.Log("OnDamge Invocado");
                
                if (health.isEmpty)
                {
                    bossManager.ResetBossesStates();
                    enemyEvents.HandleAttack(false);
                    enemyEvents.HandleRetreat(false);
                    
                    bossManager.RemoveBossFromBuffer(this);
                    bossManager.CheckInvincibilityStatus();
                  
                    
                    bossManager.StartAI();
                    enemyEvents.OnDie?.Invoke();
                    gameObject.SetActive(false);
                }
            } 
            else if (IsInvincible)
            { 
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
