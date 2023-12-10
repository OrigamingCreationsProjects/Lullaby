using Lullaby.Entities.Events;
using UnityEngine;
using UnityEngine.Splines;

namespace Lullaby.Entities
{
    public abstract class Entity : MonoBehaviour
    {
        public EntityEvents entityEvents;
        
        protected Collider[] _contactBuffer = new Collider[10];

        protected readonly float _groundOffset = 0.1f;

        protected readonly float _slopingGroundAngle = 20f;

        protected float lockGravityTime;
     
        /// <summary>
        /// Returns the Character Controller of this Entity.
        /// </summary>
        public EntityController controller { get; protected set; }

        #region -- VELOCITES --
        
        /// <summary>
        /// The current velocity of this Entity.
        /// </summary>
        /// <value></value>
        public Vector3 velocity { get; set; }

        /// <summary>
        /// The current velocity of this Entity in the local space.
        /// </summary>
        /// <value></value>
        public Vector3 localVelocity
        {
            get { return Quaternion.FromToRotation(transform.up, Vector3.up) * velocity; }
            set { velocity = Quaternion.FromToRotation(Vector3.up, transform.up) * value; }
        }

        /// <summary>
        /// The current XZ velocity of this Entity.
        /// </summary>
        public Vector3 lateralVelocity
        {
            get
            {
                var value = new Vector3(localVelocity.x, 0, localVelocity.z);

                if (value.sqrMagnitude < 0.0001f)
                {
                    return Vector3.zero;
                }

                return value;
            }
            set { localVelocity = new Vector3(value.x, localVelocity.y, value.z); }
        }

        /// <summary>
        /// The current Y velocity of this Entity.
        /// </summary>
        public Vector3 verticalVelocity
        {
            get { return new Vector3(0, localVelocity.y, 0); }
            set { localVelocity = new Vector3(localVelocity.x, value.y, localVelocity.z); }
        }

        #endregion

        #region -- POSITIONS --

        /// <summary>
        /// The initial parent of this Entity.
        /// </summary>
        public Transform initialParent { get; set; }

        /// <summary>
        /// Returns the Entity position in the previous frame.
        /// </summary>
        public Vector3 lastPosition { get; set; }

        /// <summary>
        /// Return the Entity position.
        /// </summary>
        public Vector3 position => transform.position + transform.rotation * center;

        /// <summary>
        /// Returns the Entity position ignoring any collision resizing.
        /// </summary>
        public Vector3 unsizedPosition =>
            position - transform.up * height * 0.5f + transform.up * originalHeight * 0.5f;

        /// <summary>
        /// Returns the bottom position of this Entity considering the stepOffset.
        /// </summary>
        public Vector3 stepPosition => position - transform.up * (height * 0.5f - controller.stepOffset);

        /// <summary>
        /// The distance between the last and current Entity position.
        /// </summary>
        public float positionDelta { get; protected set; }
        
        #endregion

        #region -- TIMES AND BOOLEANS --

        /// <summary>
        /// Returns the last frame this Entity was grounded.
        /// </summary>
        public float lastGroundTime { get; protected set; }

        /// <summary>
        /// Returns true if the Entity is on the ground.
        /// </summary>
        public bool isGrounded { get; protected set; } = true;

        /// <summary>
        /// Returns true if the Entity is on Rail.
        /// </summary>
        public bool onRails { get; set; }

        #endregion

        #region --MULTIPLIERS--

        public float accelerationMultiplier { get; set; } = 1f;

        public float gravityMultiplier { get; set; } = 1f;

        public float topSpeedMultiplier { get; set; } = 1f;

        public float turningDragMultiplier { get; set; } = 1f;

        public float decelerationMultiplier { get; set; } = 1f;

        #endregion

        #region -- ENVIROMENT INFO --

        /// <summary>
        /// Returns the hit info of the ground.
        /// </summary>
        public RaycastHit groundHit;

        /// <summary>
        /// Returns the current rails this Entity is attached to.
        /// </summary>
        public SplineContainer rails { get; protected set; }

        /// <summary>
        /// Returns the current Gravity Field of this Entity (Not required by now).
        /// </summary>
        //public GravityField gravityField { get; set; }

        /// <summary>
        /// Returns the angle of the current ground.
        /// </summary>
        public float groundAngle { get; protected set; }

        /// <summary>
        /// Returns the ground normal of the current ground.
        /// </summary>
        public Vector3 groundNormal { get; protected set; }

        /// <summary>
        /// Returns the local slope direction of the current ground.
        /// </summary>
        public Vector3 localSlopeDirection { get; protected set; }

        #endregion

        #region -- DIMENSIONS --

        /// <summary>
        /// Returns the original height of this Entity.
        /// </summary>
        public float originalHeight { get; protected set; }

        /// <summary>
        /// Returns the collider height of this Entity.
        /// </summary>
        public float height => controller.height;

        /// <summary>
        /// Returns the collider radius of this Entity.
        /// </summary>
        public float radius => controller.radius;

        /// <summary>
        /// The center of the Character Controller collider.
        /// </summary>
        public Vector3 center => controller.center;

        #endregion

        public virtual Vector3 localForward =>
            Quaternion.FromToRotation(transform.up, Vector3.up) * transform.forward;

        public virtual Vector3 localRight =>
            Quaternion.FromToRotation(transform.up, Vector3.up) * transform.right;

        protected BoxCollider penetratorCollider;

        protected Rigidbody entityRigidbody;

        /// <summary>
        /// Returns true if the Player is on a sloping ground.
        /// </summary>
        /// <returns></returns>
        public virtual bool OnSlopingGround()
        {
            if (isGrounded && groundAngle > _slopingGroundAngle)
            {
                if (Physics.Raycast(transform.position, -transform.up, out var hit, height * 2f,
                        Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
                {
                    return Vector3.Angle(hit.normal, transform.up) > _slopingGroundAngle; // Comprobamos si el angulo es
                    // mayor al maximo permitido para las cuestas.
                }
                else
                    return true;
            }

            return false;
        }

        // Recordar meter por aqui posible comprobador de cambio de campo gravitatorio

        /// <summary>
        /// Resizes the Character Controller (his collider) to a given height.
        /// </summary>
        /// <param name="height">The desired height.</param>
        public virtual void ResizeCollider(float height) => controller.Resize(height);

        #region -- RAYCASTERS --
        
        public virtual bool CapsuleCast(Vector3 direction, float distance, int layer = Physics.DefaultRaycastLayers,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            return CapsuleCast(direction, distance, out _, layer, queryTriggerInteraction);
        }

        public virtual bool CapsuleCast(Vector3 direction, float distance,
            out RaycastHit hit, int layer = Physics.DefaultRaycastLayers,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            var origin = position - direction * radius; // Calculamos el origen del raycast
            var offset = transform.up * (height * 0.5f - radius); // Calculamos el offset del raycast
            var top = origin + offset; // Calculamos el punto mas alto del raycast
            var bottom = origin - offset; // Calculamos el punto mas bajo del raycast
            return Physics.CapsuleCast(top, bottom, radius, direction,
                out hit, distance + radius, layer, queryTriggerInteraction);
        }

        public virtual bool SphereCast(Vector3 direction, float distance, int layer = Physics.DefaultRaycastLayers,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            return SphereCast(direction, distance, out _, layer, queryTriggerInteraction);
        }

        public virtual bool SphereCast(Vector3 direction, float distance,
            out RaycastHit hit, int layer = Physics.DefaultRaycastLayers,
            QueryTriggerInteraction queryTriggerInteraction = QueryTriggerInteraction.Ignore)
        {
            var castDistance = Mathf.Abs(distance - radius);
            return Physics.SphereCast(position, radius, direction,
                out hit, castDistance, layer, queryTriggerInteraction);
        }

        public virtual int OverlapEntity(Collider[] result, float skinOffset = 0) =>
            OverlapEntity(position, result, skinOffset);

        public virtual int OverlapEntity(Vector3 pos, Collider[] result, float skinOffset = 0)
        {
            var overlapRadius = radius + skinOffset;
            var offset = (height + skinOffset) * 0.5f - overlapRadius; // Calculamos el offset del raycast
            var top = pos + transform.up * offset; // Calculamos el punto mas alto del raycast
            var bottom = pos - transform.up * offset; // Calculamos el punto mas bajo del raycast
            
            return Physics.OverlapCapsuleNonAlloc(top, bottom, overlapRadius,
                result, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);
        }
        #endregion
        public virtual void ApplyDamage(int damage, Vector3 origin) { }
        
    }
    // Hacemos que T debe ser una clase que herede de Entity<T>.
    // Esto se utiliza para garantizar que T se relacione con Entity<T> y no con cualquier otro tipo.
    public abstract class Entity<T> : Entity where T : Entity<T> 
    {
        //public static T instance { get; protected set; }
        protected IEntityContact[] _contactListeners;
           
        //Controlador de estados de las entidades
        public EntityStateManager<T> states { get; protected set; }
        
        #region -- INITIALIZATION --
        
        protected virtual void InitializeController()
        {
            controller = GetComponent<EntityController>();

            if (!controller)
            {
                controller = gameObject.AddComponent<EntityController>();
            }

            originalHeight = controller.height;
        }
        protected virtual void InitializeRigidbody()
        {
            entityRigidbody = gameObject.AddComponent<Rigidbody>();
            entityRigidbody.isKinematic = true;
        }
        protected virtual void InitializeStateManager() => states = GetComponent<EntityStateManager<T>>();
        protected virtual void InitializeParent() => initialParent = transform.parent;
        
        #endregion

        #region -- HANDLERS --
        
        protected virtual void HandleStates() => states.Step();
        
        protected virtual void HandleController()
        {
            if (controller.enabled)
            {
                controller.Move(velocity * Time.deltaTime);
                return;
            }

            transform.position += velocity * Time.deltaTime;
        }

        protected virtual void HandleSpline()
        {
            var distance = (height * 0.5f) + height * 0.5f;// Calculamos la distancia a la que se encuentra el centro del collider

            if (SphereCast(-transform.up, distance, out var hit) &&
                hit.collider.CompareTag(GameTags.InteractiveRail))
            {
                if (!onRails && verticalVelocity.y <= 0) // comprobamos la velocidad para que no entre al rail cuando está saliendo hacia arriba
                {
                    EnterRail(hit.collider.GetComponent<SplineContainer>());
                }
            }
            else
            {
                ExitRail();
            }
        }
        
        protected virtual void HandleGround()
        {
            if(onRails) return;

            var distance = (height * 0.5f) + _groundOffset; // Calculamos la distancia a la que se encuentra el centro del collider
            var sphereColliding = SphereCast(-transform.up, distance, out var sphereHit);
            var hitColliding = Physics.Raycast(position, -transform.up, out var rayHit,
                distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore);

            var movingTowardGround = Vector3.Dot(velocity, sphereHit.normal) <= 0; // Comprobamos si el personaje se esta moviendo hacia el suelo
            var validSphereAngle = Vector3.Angle(sphereHit.normal, transform.up) < controller.slopeLimit; 
            var validAngle = validSphereAngle || hitColliding && 
                Vector3.Angle(rayHit.normal, transform.up) < controller.slopeLimit; // Comprobamos si el angulo del suelo es valido 
            
            if (sphereColliding && movingTowardGround && validAngle)
            {
                if (!isGrounded && EvaluateLanding(sphereHit))
                {
                    EnterGround(sphereHit);
                }
                UpdateGround(sphereHit);
            }
            else
            {
                ExitGround();
            }
        }
        
        protected virtual void HandleContacts()
        {
            var skinOffset = controller.skinWidth + Physics.defaultContactOffset;
            var overlaps = OverlapEntity(_contactBuffer, skinOffset); // Obtenemos en numero de colliders que
                                                                     // estan en contacto con el personaje
            for (int i = 0; i < overlaps; i++)
            {
                if(_contactBuffer[i].transform == transform) continue; // Si el collider es el mismo que el del personaje
                                                                     // continuamos con el siguiente collider
                OnContact(_contactBuffer[i]);

                _contactListeners = _contactBuffer[i].GetComponents<IEntityContact>();
                foreach (var contact in _contactListeners)
                    contact.OnEntityContact((T)this);

            }    
        }

        protected virtual void HandlePosition()
        {
            positionDelta = (position - lastPosition).magnitude; // Calculamos la distancia entre la posición actual y la anterior
            lastPosition = position;
        }
        #endregion
        
        #region -- ENTER / EXIT --
        protected virtual void EnterGround(RaycastHit hit)
        {
            if (!isGrounded)
            {
                groundHit = hit;
                isGrounded = true;
                entityEvents.OnGroundEnter?.Invoke();
            }
        }

        protected virtual void ExitGround()
        {
            if (isGrounded)
            {
                isGrounded = false;
                transform.parent = initialParent; // Desvinculamos al personaje de la plataforma
                lastGroundTime = Time.time;
                verticalVelocity = Vector3.Max(verticalVelocity, Vector3.zero); // Si la velocidad vertical es menor a 0
                                                                                    // la igualamos a 0
                entityEvents.OnGroundExit?.Invoke();
            }
        }
        protected virtual void EnterRail(SplineContainer rails)
        {
            if (!onRails)
            {
                onRails = true;
                this.rails = rails;
                entityEvents.OnRailsEnter?.Invoke();
            }
        }
        public virtual void ExitRail()
        {
            if (onRails)
            {
                onRails = false;
                entityEvents.OnRailsExit.Invoke();
            }
        }
        #endregion
        
        public virtual void LockGravity(float duration = 0.1f) =>
            lockGravityTime = Time.time + duration;
        
        //protected virtual void UpdateGravityField() Aqui actualizariamos el campo gravitatorio,
        //pero todavia no lo necesitamos
        protected virtual void UpdateGround(RaycastHit hit)
        {
            if (isGrounded)
            {
                groundHit = hit; // Actualizamos la informacion de la superficie
                groundNormal = groundHit.normal; // Actualizamos la normal de la superficie
                groundAngle = Vector3.Angle(Vector3.up, groundHit.normal); // Calculamos el angulo de la superficie
                localSlopeDirection = new Vector3(groundNormal.x, 0, groundNormal.z).normalized; // Calculamos la direccion de la pendiente
                transform.parent = hit.collider.CompareTag(GameTags.Platform) || hit.collider.CompareTag(GameTags.Piano) 
                    ? hit.transform : initialParent;   // Si la superficie es una plataforma
                                                      // vinculamos al personaje con la plataforma
            }
        }

        protected virtual bool EvaluateLanding(RaycastHit hit) =>
            Vector3.Angle(hit.normal, transform.up) < controller.slopeLimit;
        
        protected virtual void OnUpdate(){}

        protected virtual void OnContact(Collider other)
        {
            if (other)
            {
                states.OnContact(other);
            }
        }
        /// <summary>
        /// Moves the Player smoothly in a given direction.
        /// </summary>
        /// <param name="direction">The direction you want to move.</param>
        /// <param name="turningDrag">How fast it will turn towards the new direction.</param>
        /// <param name="acceleration">How fast it will move over time.</param>
        /// <param name="topSpeed">The max movement magnitude.</param>
        public virtual void Accelerate(Vector3 direction, float turningDrag, float acceleration, float topSpeed)
        {
            if (direction.sqrMagnitude > 0)
            {
                var speed = Vector3.Dot(direction, lateralVelocity); // Calculamos la rapidez
                var velocity = direction * speed; // Calculamos la velocidad con direccion
                var turningVelocity = lateralVelocity - velocity; // Calculamos la velocidad de giro
                var turningDelta = turningDrag * turningDragMultiplier * Time.deltaTime; // Calculamos el máximo de giro por frame
                var targetTopSpeed = topSpeed * topSpeedMultiplier;

                if (lateralVelocity.magnitude < targetTopSpeed || speed < 0)
                {
                    speed += acceleration * accelerationMultiplier * Time.deltaTime; // Calculamos la aceleracion
                    speed = Mathf.Clamp(speed, -targetTopSpeed, targetTopSpeed); // Limitamos la velocidad
                }
                velocity = direction * speed; // Calculamos la velocidad con direccion
                turningVelocity = Vector3.MoveTowards(turningVelocity, Vector3.zero, turningDelta); // Calculamos la velocidad de giro
                                                                                                    // en cada frame tiene un maximo de turningDelta
                lateralVelocity = velocity + turningVelocity; // Calculamos la velocidad lateral
            }
        }
        /// <summary>
        /// Smoothly moves Lateral Velocity to zero.
        /// </summary>
        /// <param name="deceleration">How fast it will decelerate over time.</param>
        public virtual void Decelerate(float deceleration)
        {
            var delta = deceleration * decelerationMultiplier * Time.deltaTime; // Calculamos el maximo de desaceleracion por frame
            lateralVelocity = Vector3.MoveTowards(lateralVelocity, Vector3.zero, delta);
        }
        
        /// <summary>
        /// Smoothly moves vertical velocity to zero.
        /// </summary>
        /// <param name="gravity">How fast it will move over time.</param>
        public virtual void ApplyGravity(float gravity)
        {
            if (!isGrounded)
            {
                verticalVelocity += Vector3.down * gravity * gravityMultiplier * Time.deltaTime;
            }
        }
        /// <summary>
        /// Increases the lateral velocity based on the slope angle.
        /// </summary>
        /// <param name="upwardForce">The force applied when moving upwards.</param>
        /// <param name="downwardForce">The force applied when moving downwards.</param>
        public virtual void SlopeFactor(float updwardForce, float downwardForce)
        {
            if(!isGrounded || !OnSlopingGround()) return; // Si no esta en el suelo o no esta en una pendiente no hacemos nada

            var factor = Vector3.Dot(Vector3.up, groundNormal);
            //Usamos dot product para comprobar si la diferencia en la direccion de ambos vectores, entre -1 y 1.
            //(1 misma direccion y -1 direcciones opuestas. 0 perpendicular)
            var downwards = Vector3.Dot(localSlopeDirection, lateralVelocity) > 0; // Si la velocidad lateral es mayor a 0
                                                                                    // el personaje esta bajando
            var multiplier = downwards? downwardForce : updwardForce; // Si el personaje esta bajando aplicamos fuerza hacia abajo
            var delta = factor * multiplier * Time.deltaTime; // Calculamos la fuerza a aplicar en la direccion de la pendiente
            lateralVelocity += localSlopeDirection * delta; // Aplicamos la fuerza
        }
        /// <summary>
        /// Applies a force towards the ground.
        /// </summary>
        /// <param name="force">The force you want to apply.</param>
        public virtual void SnapToGround(float force)
        {
            if (isGrounded && (verticalVelocity.y <= 0))
            {
                verticalVelocity = Vector3.down * force; // Aplicamos la fuerza hacia abajo
            }
        }
        /// <summary>
        /// Rotate the Player towards to a given direction.
        /// </summary>
        /// <param name="direction">The direction you want to face.</param>
        public virtual void FaceDirection(Vector3 direction, Space space = Space.Self)
        {
            if (direction != Vector3.zero)
            {
                if(space == Space.Self)
                    direction = Quaternion.FromToRotation(Vector3.up, transform.up) * direction; // Transformamos la direccion al espacio local
                
                var rotation = Quaternion.LookRotation(direction, transform.up);
                transform.rotation = rotation;

            }
        }
        
        /// <summary>
        /// Rotate the Player towards to a given direction SMOOTHLY.
        /// </summary>
        /// <param name="direction">The direction you want to face.</param>
        /// <param name="degreesPerSecond">How fast it should rotate over time.</param>
        public virtual void FaceDirectionSmooth(Vector3 direction, float degreesPerSecond)
        {
            if (direction != Vector3.zero)
            {
                direction = Quaternion.FromToRotation(Vector3.up, transform.up) * direction; // Transformamos la direccion al espacio local
                var rotation = transform.rotation;
                var rotationDelta = degreesPerSecond * Time.deltaTime; // Calculamos el maximo de rotacion por frame
                var target = Quaternion.LookRotation(direction, transform.up); // Calculamos la rotacion objetivo
                transform.rotation = Quaternion.RotateTowards(rotation, target, rotationDelta); // Rotamos el personaje suavemente
            }
        }
        /// <summary>
        /// Returns true if this Entity collider fits into a given position. For example, for the ledge grab and climb.
        /// </summary>
        /// <param name="position">The position you want to test if the Entity collider fits.</param>
        public virtual bool FitsIntoPosition(Vector3 position)
        {
            var skinOffset = controller.skinWidth + Physics.defaultContactOffset;
            var overlaps = OverlapEntity(position, _contactBuffer, -skinOffset);

            for (int i = 0; i < overlaps; i++)
            {
                if (_contactBuffer[i].gameObject.isStatic && !GameTags.IsHazard(_contactBuffer[i]))
                    return false;
            }

            return true;
        }
        /// <summary>
        /// Enables or disables the custom collision. Disabling the Character Controller.
        /// </summary>
        /// <param name="value">If true, enables the custom collision.</param>
        public virtual void UseCustomCollision(bool value) => controller.handleCollision = !value;

        protected virtual void Awake()
        {
            InitializeController();
            InitializeStateManager();
            InitializeParent();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if(!controller.enabled) return;
            
            HandleGround();
            HandleStates();
            //UpdateGravityField(); //(Not necessary by now)
            HandleController();
            HandleContacts();
            HandleSpline();
            OnUpdate();
        }

        protected virtual void LateUpdate()
        {
            if(!controller.enabled) return;
            
            HandlePosition();
        }
    }
}
