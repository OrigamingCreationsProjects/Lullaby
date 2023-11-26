using Lullaby.Entities.Enemies.States;
using Lullaby.Entities.Events;
using UnityEngine;

namespace Lullaby.Entities.Enemies
{
    [RequireComponent(typeof(EnemyStatsManager))]
    [RequireComponent(typeof(EnemyStateManager))]
    [RequireComponent(typeof(WaypointManager))]
    [RequireComponent(typeof(Health))]
    [AddComponentMenu("Lullaby/Enemies/Enemy")]
    public class Enemy : Entity<Enemy>
    {
        //Eventos del enemigo (publicos al poder asignarse en el inspector)
        public EnemyEvents enemyEvents; 
        
        protected Player _player;

        protected Collider[] sightOverlaps = new Collider[1024];
        protected Collider[] contactAttackOverlaps = new Collider[1024];
        
        /// <summary>
        /// Returns the Enemy Stats Manager instance.
        /// </summary>
        public EnemyStatsManager stats { get; protected set; }
        
        /// <summary>
        /// Returns the Waypoint Manager instance.
        /// </summary>
        public WaypointManager waypoints { get; protected set; }
        /// <summary>
        /// Returns the Health instance.
        /// </summary>
        public Health health { get; protected set; }
        
        /// <summary>
        /// Returns the instance of the Player on the Enemies sight.
        /// </summary>
        public Player player { get; protected set; }
        

        #region -- INITIALIZERS --
        
        protected virtual void InitializeStatsManager() => stats = GetComponent<EnemyStatsManager>();
        protected virtual void InitializeWaypointManager() => waypoints= GetComponent<WaypointManager>();
        protected virtual void InitializeHealth() => health = GetComponent<Health>();
        protected virtual void InitializeTag() => tag = GameTags.Enemy;
        
        #endregion

        /// <summary>
        /// Applies damage to this Enemy decreasing its health with proper reaction
        /// </summary>
        /// <param name="amount">The amount of health you want to decrease.</param>
        /// <param name="origin">The origin hit position where we recieve damage.</param>
        public override void ApplyDamage(int amount, Vector3 origin)
        {
            if (!health.isEmpty && !health.recovering)
            {
                health.Damage(amount);
                enemyEvents.OnDamage?.Invoke();

                if (stats.current.canReceivePushBack)
                {
                    states.Change<HurtEnemyState>();
                    //Debug.Log("Deberia cambiar de estado");
                    // verticalVelocity = Vector3.up * stats.current.hurtUpwardsForce;
                    // lateralVelocity = -localForward * stats.current.hurtBackwardsForce;
                }
                
                //Debug.Log("Enemigo dañado");
                if (health.isEmpty)
                {
                    //controller.enabled = false;
                    enemyEvents.OnDie?.Invoke();
                    states.Change<DieEnemyState>();
                    //gameObject.SetActive(false);
                }
                else
                {
                    //states.Change<HurtEnemyState>();
                }
            }
        }
        
        public virtual void Revive()
        {
            if(!health.isEmpty) return;
            
            health.ResetHealth();
            controller.enabled = true;
            enemyEvents.OnRevive.Invoke();
        }

        public virtual void Accelerate(Vector3 direction, float accelaration, float topSpeed) =>
            Accelerate(direction, stats.current.turningDrag, accelaration, topSpeed);
        
        /// <summary>
        /// Smoothly sets Lateral Velocity to zero by its deceleration stats.
        /// </summary>
        public virtual void Decelerate() => Decelerate(stats.current.deceleration);

        /// <summary>
        /// Smoothly sets Lateral Velocity to zero by its friction stats.
        /// </summary>
        public virtual void Friction() => Decelerate(stats.current.friction);
        
        /// <summary>
        /// Applies a downward force by its gravity stats.
        /// </summary>
        public virtual void ApplyGravity() => ApplyGravity(stats.current.gravity);

        /// <summary>
        /// Applies a downward force when ground by its snap stats.
        /// </summary>
        public virtual void SnapToGround() => SnapToGround(stats.current.snapForce);

        /// <summary>
        /// Rotate the Enemy forward to a given direction.
        /// </summary>
        /// <param name="direction"></param>
        public virtual void FaceDirectionSmooth(Vector3 direction) => FaceDirectionSmooth(direction, stats.current.rotationSpeed);


        public virtual void ContactAttack(Collider other)
        {
            if(!other.CompareTag(GameTags.Player)) return;
            if(!other.TryGetComponent(out Player player)) return;

            var stepping = controller.bounds.max + Vector3.down * stats.current.contactSteppingTolerance; // Posicion del enemigo en el suelo

            if (player.isGrounded || !BoundsHelper.IsBellowPoint(controller.collider, stepping)) // Si el jugador esta en el suelo y no está por encima del enemigo
            {
                if (stats.current.contactPushback) // Si puede mandar para atras al jugador
                    lateralVelocity = -localForward * stats.current.contactPushBackForce; // Empujamos al jugador hacia atras
                
                player.ApplyDamage(stats.current.contactDamage, transform.position);
                enemyEvents.OnPlayerContact?.Invoke();
            }
        }

        public virtual void ApplyDieForces()
        {
            verticalVelocity = Vector3.up * stats.current.dieUpwardsForce;
            lateralVelocity = -localForward * stats.current.dieBackwardsForce;
            //lateralVelocity = new Vector3(0, 0, -localForward.z * stats.current.hurtBackwardsForce);
        }

        //Meter logica de desaparición como las partículas y tal
        //(quiza mejor invocar el evento de desaparicion y que el componente de particulas se encargue de ello)
        public virtual void Disappear()
        {
            gameObject.SetActive(false);
            enemyEvents.OnDisappear?.Invoke();
        }

        public virtual bool IsAlive()
        {
            return health.current > 0;
        }
        
        /// <summary>
        /// Handles the view sight and Player detection behaviour.
        /// </summary>
        protected virtual void HandleSight()
        {
            if (!player)
            {
                var overlaps = Physics.OverlapSphereNonAlloc(position, stats.current.spotRange, sightOverlaps); // Detectamos al jugador

                for (int i = 0; i < overlaps; i++)
                {
                    if (sightOverlaps[i].CompareTag(GameTags.Player)) 
                    {
                        if (sightOverlaps[i].TryGetComponent<Player>(out var player))
                        {
                            this.player = player;
                            enemyEvents.OnPlayerDetected?.Invoke();
                            return;
                        }
                    }
                }
            }
            else
            {
                var distance = Vector3.Distance(position, player.position);

                if ((player.health.current == 0) || (distance > stats.current.viewRange))
                {
                    player = null;
                    if(isGrounded)
                        enemyEvents.OnPlayerEscaped?.Invoke();
                }
            }
        }

        protected override void OnUpdate()
        {
            HandleSight();
        }

        protected override void Awake()
        {
            base.Awake();
            InitializeTag();
            InitializeStatsManager();
            InitializeWaypointManager();
            InitializeHealth();
        }

        protected virtual void OnTriggerEnter(Collider other)
        {
            ContactAttack(other);
        }
    }
}