using Cinemachine;
using DG.Tweening;
using Lullaby.Entities.Enemies;
using Lullaby.Entities.Events;
using Lullaby.Entities.NPC;
using Lullaby.Entities.States;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Sequence = DG.Tweening.Sequence;
using Vector3 = UnityEngine.Vector3;

namespace Lullaby.Entities
{
    [AddComponentMenu("Lullaby/Custom Movement/Player/Player")]
    [RequireComponent(typeof(PlayerInputManager))]
    [RequireComponent(typeof(PlayerStatsManager))]
    [RequireComponent(typeof(PlayerStateManager))]
    [RequireComponent(typeof(Health))]
    [RequireComponent(typeof(PlayerDialogueTrigger))]
    public class Player : Entity<Player>
    {
        //Eventos
        public PlayerEvents playerEvents;
        public Transform pickableSlot; //Slot para posicionar el objeto que se puede recoger
        public Transform skin;
        public Transform objectLauncherTest;
        
        protected Vector3 _respawnPosition;
        protected Quaternion _respawnRotation;
        
        protected Vector3 _skinInitialPosition;
        protected Quaternion _skinInitialRotation;
        protected Vector3 _skinInitialScale;
        
        /// <summary>
        /// Returns the Player Input Manager instance.
        /// </summary>
        public PlayerInputManager inputs { get; protected set; }
        /// <summary>
        /// Returns the Player Stats Manager instance.
        /// </summary>
        public PlayerStatsManager stats { get; protected set; }
        
        /// <summary>
        /// Returns the Health instance.
        /// </summary>
        public Health health { get; protected set; }
        
        /// <summary>
        /// Returns the PlayerDialogueTrigger instance.
        /// </summary>
        public PlayerDialogueTrigger dialogueTrigger { get; protected set; }
        
        /// <summary>
        /// Returns the PlayerEnemyDetector instance.
        /// </summary>
        public PlayerEnemyDetector playerEnemyDetector { get; protected set; }
        
        /// <summary>
        /// Returns the PlayerCombat instance.
        /// </summary>
        public PlayerCombat playerCombat { get; protected set; }
        
        /// <summary>
        /// Returns true if the Player is holding an object.
        /// </summary>
        public bool holding { get; protected set; }
        
        
        /// <summary>
        /// Returns how many times the Player jumped.
        /// </summary>
        public int jumpCounter { get; protected set; }
        
        /// <summary>
        /// Returns how many times the Player performed a Dash.
        /// </summary>
        /// <value></value>
        public int airDashCounter { get; protected set; }
        
        /// <summary>
        /// Returns how many times the Player performed an air attack.
        /// </summary>
        public int airAttackCounter { get; protected set; }
        
        /// <summary>
        /// The last time the Player performed an dash.
        /// </summary>
        /// <value></value>
        public float lastDashTime { get; protected set; }
        
        /// <summary>
        /// Returns the normal of the last wall the Player touched.
        /// </summary>
        public Vector3 lastWallNormal { get; protected set; }
        
        /// <summary>
        /// Return the Pickable instance which the Player is holding.
        /// </summary>
        public Pickable pickable { get; protected set; }
        
        /// <summary>
        /// Returns true if the Player health is not empty.
        /// </summary>
        public virtual bool isAlive => !health.isEmpty;
        
        /// <summary>
        /// Returns true if the Player can stand up.
        /// </summary>
        public virtual bool canStandUp => !SphereCast(transform.up, originalHeight * 0.5f);

        #region -- MOON LAUNCH --
        
        /// <summary>
        /// Returns the PlayerMoonLauncher instance.
        /// </summary>
        public PlayerMoonLauncher moonLauncher { get; protected set; }
        
        public virtual CinemachineDollyCart moonPathCart { get; set; }

        public virtual Transform launchObject { get; protected set; }
        
        
        Sequence CenterSequence()
        {
            SetInputEnabled(false);
            Sequence s = DOTween.Sequence();
            s.Append(transform.DOLocalMove(new Vector3(0, 0, -.5f), .5f));
            s.Join(transform.DOLocalRotate(new Vector3(90, 0, 0), .5f));
            s.Append(LaunchSequence());
            return s;
        }
        
        Sequence LaunchSequence()
        {
            Sequence s = DOTween.Sequence();
            s.AppendCallback(() => DOVirtual.Float(0, 1, 3, PathPosition).SetEase(Ease.InOutSine));
            return s;
        }
        
        
        void PathPosition(float x)
        {
            moonPathCart.m_Position = x;
        }
        
        #endregion
        
        
        
        #region -- INITIALIZERS --
        
        protected virtual void InitializeInputs() => inputs = GetComponent<PlayerInputManager>();
        protected virtual void InitializeStats() => stats = GetComponent<PlayerStatsManager>();
        protected virtual void InitializeHealth() => health = GetComponent<Health>();
        protected virtual void InitializeDialogueTrigger() => dialogueTrigger = GetComponent<PlayerDialogueTrigger>();
        protected virtual void InitializeTag() => tag = GameTags.Player;
        protected virtual void InitializePlayerEnemyDetector() => playerEnemyDetector = GetComponentInChildren<PlayerEnemyDetector>();
        protected virtual void InitializePlayerCombat() => playerCombat = GetComponent<PlayerCombat>();
        protected virtual void InitializeMoonLauncher() => moonLauncher = GetComponent<PlayerMoonLauncher>();
        
        
        protected virtual void InitializeRespawn()
        {
            _respawnPosition = transform.position;
            _respawnRotation = transform.rotation;
        }

        protected virtual void InitializeSkin()
        {
            if(!skin) return;
            _skinInitialPosition = skin.localPosition;
            _skinInitialRotation = skin.localRotation;
            _skinInitialScale = skin.localScale;
        }
        
        #endregion

        #region -- RESPAWN --
        /// <summary>
        /// Resets Player state, health, position, and rotation.
        /// </summary>
        public virtual void Respawn()
        {
            health.ResetHealth();
            velocity = Vector3.zero;
            transform.SetPositionAndRotation(_respawnPosition, _respawnRotation);
            states.Change<IdlePlayerState>();
        }
        /// <summary>
        /// Resets Player state, position, and rotation.
        /// </summary>
        public virtual void RespawnWithCurrentHealth()
        {
            velocity = Vector3.zero;
            transform.SetPositionAndRotation(_respawnPosition, _respawnRotation);
            states.Change<IdlePlayerState>();
        }
        
        /// <summary>
        /// Sets the position and rotation of the Player for the next respawn.
        /// </summary>
        public virtual void SetRespawn(Vector3 position, Quaternion rotation)
        {
            _respawnPosition = position;
            _respawnRotation = rotation;
        }
        #endregion
        
        #region -- HEALTH MODIFIERS METHODS --

        /// <summary>
        /// Applies damage to this Player decreasing its health with proper reaction.
        /// </summary>
        /// <param name="amount">The amount of health you want to decrease</param>
        /// <param name="origin">The origin hit position where we recieve damage</param>
        public override void ApplyDamage(int amount, Vector3 origin)
        {
            if(health.isEmpty || health.recovering) return;
            
            health.Damage(amount);

            var head = origin - transform.position; // Get the direction of the hit relative to the Player
            var upOffset = Vector3.Dot(transform.up, head); // Get the offset of the hit relative to the Player's up direction
            var damageDir = (head - (transform.up * upOffset)).normalized; // Get the direction of the hit relative to the Player's up direction
            var localDamageDir = Quaternion.FromToRotation(transform.up, Vector3.up) * damageDir; // Get the direction of the hit relative to the world up direction for face the player to the hit
            
            FaceDirection(localDamageDir);
            lateralVelocity = -localForward * stats.current.hurtBackwardsForce; // Apply a force to the Player to push it back when it gets hit relative to the hit direction
            
            //En caso de estar en una situacion especial como estar flotando o en agua
            //habria que poner estas 2 lineas bajo un if que comprobara que no esta en esa condicion especial
            verticalVelocity = Vector3.up * stats.current.hurtUpwardForce; // Apply a force to the Player to push it up when it gets hit
            states.Change<HurtPlayerState>(); 
            
            playerEvents.OnHurt?.Invoke();
            if (holding)
            {
                Throw();
                RemovePickable();
            }
            if (health.isEmpty)
            {
                //Si al final implementamos que se puedan coger objetos aqui habria que poner que se suelte o lance el objeto
                Die();
            }
        }

        /// <summary>
        /// Applies damage to this Player decreasing its health with proper reaction when a DOLLY attack.
        /// </summary>
        /// <param name="damageAmount">The amount of health you want to decrease</param>
        /// <param name="origin">The origin hit position where we receive damage</param>
        public void ReceivePunch(int damageAmount, Vector3 origin)
        {
            Debug.Log("Se recibe el golpe");
            if(health.isEmpty || health.recovering) return;
            
            health.Damage(damageAmount);

            var head = origin - transform.position; // Get the direction of the hit relative to the Player
            var upOffset = Vector3.Dot(transform.up, head); // Get the offset of the hit relative to the Player's up direction
            var damageDir = (head - (transform.up * upOffset)).normalized; // Get the direction of the hit relative to the Player's up direction
            var localDamageDir = Quaternion.FromToRotation(transform.up, Vector3.up) * damageDir; // Get the direction of the hit relative to the world up direction for face the player to the hit
            
            FaceDirection(localDamageDir);
            
            states.Change<PunchHitPlayerState>(); 
            
            playerEvents.OnHurt?.Invoke();

            if (health.isEmpty)
            {
                //Si al final implementamos que se puedan coger objetos aqui habria que poner que se suelte o lance el objeto
                Die();
            }
        }
        
        /// <summary>
        /// Kills the Player.
        /// </summary>
        public virtual void Die()
        {
            health.Set(0);
            playerEvents.OnDie?.Invoke();
        }
        #endregion
        protected override bool EvaluateLanding(RaycastHit hit)
        {
            // Hacemos que se compruebe lo que ya se comprobaba en el original y si es una zona de muelle para no aterrizar
            return base.EvaluateLanding(hit) && !hit.collider.CompareTag(GameTags.Spring); 
        }
        
        /// <summary>
        /// Moves the Player smoothly in a given direction.
        /// </summary>
        /// <param name="direction">The direction you want to move.</param>
        public virtual void Accelerate(Vector3 direction)
        {
            var turningDrag = isGrounded && inputs.GetRun() ? stats.current.runningTurningDrag : stats.current.turningDrag;
            var acceleration = isGrounded && inputs.GetRun() ? stats.current.runningAcceleration : stats.current.acceleration;
            var finalAcceleration = isGrounded ? acceleration : stats.current.airAcceleration;
            var topSpeed = inputs.GetRun()? stats.current.runningTopSpeed : stats.current.topSpeed;
            
            Accelerate(direction, turningDrag, finalAcceleration, topSpeed);

            if (inputs.GetRunUp())
            {
                lateralVelocity = Vector3.ClampMagnitude(lateralVelocity, topSpeed); //Clamp the speed to the top speed
            }
        }

        /// <summary>
        /// Moves the Player smoothly in the input direction relative to the camera.
        /// </summary>
        public virtual void AccelerateToInputDirection()
        {
            var inputDirection = inputs.GetMovementCameraDirection();
            Accelerate(inputDirection);
        }

        /// <summary>
        /// Applies the standard slope factor to the Player.
        /// </summary>
        public virtual void RegularSlopeFactor()
        {
            if(stats.current.applySlopeFactor)
                SlopeFactor(stats.current.slopeUpwardForce, stats.current.slopeDownwardForce);
        }
        
        /// <summary>
        /// Smoothly sets Lateral Velocity to zero by its deceleration stats.
        /// </summary>
        public virtual void Decelerate() => Decelerate(stats.current.deceleration);

        /// <summary>
        /// Smoothly sets Lateral Velocity to zero by its friction stats.
        /// </summary>
        public virtual void ApplyFriction()
        {
            if(OnSlopingGround())
                Decelerate(stats.current.slopeFriction);
            else
                Decelerate(stats.current.friction);
        }
        
        /// <summary>
        /// Applies a downward force by its gravity stats.
        /// </summary>
        public virtual void ApplyGravity()
        {
            if (!isGrounded && verticalVelocity.y > -stats.current.gravityTopSpeed)
            {
                var speed = verticalVelocity.y;
                var force = verticalVelocity.y > 0 ? stats.current.gravity : stats.current.fallGravity;
                speed -= force * gravityMultiplier * Time.deltaTime;
                speed = Mathf.Max(speed, -stats.current.gravityTopSpeed);
                verticalVelocity = new Vector3(0, speed, 0);
            }
        }
        
        /// <summary>
        /// Applies a downward force when ground by its snap stats.
        /// </summary>
        public virtual void SnapToGround() => SnapToGround(stats.current.snapForce);
        /// <summary>
        /// Rotate the Player forward to a given direction SMOOTHLY.
        /// </summary>
        /// <param name="direction">The direction you want it to face.</param>
        public virtual void FaceDirectionSmooth(Vector3 direction) => FaceDirectionSmooth(direction ,stats.current.rotationSpeed);

        
        /// <summary>
        /// Makes a transition to the Fall State if the Player is not grounded.
        /// </summary>
        public virtual void Fall()
        {
            if (!isGrounded)
            {
                states.Change<FallPlayerState>();
            }
        }
        
        /// <summary>
        /// Handles ground jump with proper evaluations and height control.
        /// </summary>
        public virtual void Jump()
        {
            var canMultiJump = (jumpCounter > 0) && (jumpCounter < stats.current.multiJumps);
            var canCoyoteJump = (jumpCounter == 0) && (Time.time < lastGroundTime + stats.current.coyoteJumpThreshold);
            var holdJump =  !holding || stats.current.canJumpWhileHolding; //Comentamos esto porque no sabemos si se cogerán objetos

            if ((isGrounded || onRails || canMultiJump || canCoyoteJump) && holdJump)
            {
                if(inputs.GetJumpDown())
                {
                    Debug.Log($"Vertical velocity en el salto es: {verticalVelocity}");
                    if (canMultiJump)
                    {
                        //Revisar esto bien
                        //if(verticalVelocity.y <= 0)
                            DoubleJump(stats.current.doubleJumpHeight);
                    }else
                        Jump(stats.current.maxJumpHeight);
                }
            }
            
            if(inputs.GetJumpUp() && (jumpCounter > 0) && (verticalVelocity.y > stats.current.minJumpHeight))
            {
                verticalVelocity = Vector3.up * stats.current.minJumpHeight; //Esto es para que no salte tan alto en el
                                                                             //doble salto o si mantenemos poco el botón salte menos 
            }
        }
        /// <summary>
        /// Applies an upward force to the Player.
        /// </summary>
        /// <param name="height">The force you want to apply.</param>
        public virtual void Jump(float height)
        {
            jumpCounter++;
            verticalVelocity = Vector3.up * height;
            //verticalVelocity = Vector3.up * Mathf.Sqrt(2 * height * stats.current.gravity); //Alternativa
            states.Change<FallPlayerState>();
            playerEvents.OnJump?.Invoke();
        }
        public virtual void DoubleJump(float height)
        {
            jumpCounter++;
            Sequence sequence = DOTween.Sequence();
            Quaternion initialRotation = transform.localRotation;
            sequence.AppendCallback(() => initialRotation = transform.localRotation);
            sequence.Append(skin.transform.DOLocalRotate(new Vector3(360, 0,0), stats.current.frontFlipTime, RotateMode.FastBeyond360).SetEase(Ease.OutCirc));
            sequence.AppendCallback(() => transform.rotation = initialRotation);
            verticalVelocity = new Vector3(verticalVelocity.x, height, verticalVelocity.z);
            //verticalVelocity = Vector3.up * Mathf.Sqrt(2 * height * stats.current.gravity); //Alternativa
            states.Change<FallPlayerState>();
            playerEvents.OnJump?.Invoke();
        }
        
        /// <summary>
        /// Applies jump force to the Player in a given direction.
        /// </summary>
        /// <param name="direction">The direction that you want to jump.</param>
        /// <param name="height">The upward force that you want to apply.</param>
        /// <param name="distance">The force towards the direction that you want to apply.</param>
        public virtual void DirectionalJump(Vector3 direction, float height, float distance)
        {
            jumpCounter++;
            verticalVelocity = Vector3.up * height;
            lateralVelocity = direction * distance;
            playerEvents.OnJump?.Invoke();
        }

        #region -- RESETERS --
        /// <summary>
        /// Sets the air dash counter to zero.
        /// </summary>
        public virtual void ResetAirDash() => airDashCounter = 0;
        
        /// <summary>
        /// Sets the jump counter to zero affecting further jump evaluations.
        /// </summary>
        public virtual void ResetJumps() => jumpCounter = 0;
        
        #endregion
        
        /// <summary>
        /// Sets the jump counter to a specific value.
        /// </summary>
        /// <param name="amount">The amount of jumps.</param>
        public virtual void SetJumps(int amount) => jumpCounter = amount;

        #region -- COMBAT METHODS --
        
        /// <summary>
        /// Method to attack
        /// </summary>
        //ES POSIBLE QUE ESTE METODO DEBA ESTAR EN ENTITY PORQUE QUIZA TODAS LAS ENTIDADES PUEDAN ATACAR
        public virtual void Attack()
        {
            var canMakeAnAttack = (isGrounded || stats.current.canAirAttack) &&
                               (airAttackCounter < stats.current.allowedAirAttacks);
            
            // Si queremos que se cojan objetos tambien habra que comprobar !holding para que no ataque al tener objetos.
            // O implementar logica para que el objeto salga volando al atacar.
            if(stats.current.canAttack && canMakeAnAttack && !holding && inputs.GetAttackDown()) 
            {
                if (!isGrounded)
                {
                    airAttackCounter++;
                }
                
                Sequence s = DOTween.Sequence();
                s.AppendCallback(() => playerCombat.AttackCheck());
                //s.AppendCallback(() => playerCombat.RegularAttackCheck());
                //s.AppendCallback(() => states.Change<AttackPlayerState>());
                //s.AppendCallback(() => playerEvents.OnAttackStarted?.Invoke());
                // states.Change<AttackPlayerState>();
                // playerEvents.OnAttackStarted?.Invoke();
            }
        }
        
        public void MoveTowardsTarget(Enemy target, float duration)
        {
            Debug.Log("Nos movemos hacia el objetivo");
            FaceToTarget(target.transform);
            transform.DOMove(TargetOffset(target), duration); //.SetEase(Ease.Linear);
            //_player.states.Change<AttackPlayerState>();
        }
        public virtual void FaceToTarget(Transform target)
        {
            var destination = target.position;
            var head = destination - transform.position;
            var upOffset = Vector3.Dot(transform.up, head); // Sacamos la direccion a la que mirar manteniendo nuestro eje Y
           
            head -= transform.up * upOffset;
            
            var distance = head.magnitude;
            var direction = head / distance; // Normalizamos porque solo nos interesa la direccion
            var localDirection = Quaternion.FromToRotation(transform.up, Vector3.up) * direction;
            
           FaceDirectionSmooth(localDirection);

        }
        
        public virtual Vector3 TargetOffset(Enemy target)
        {
            Vector3 offsetPosition;
            offsetPosition = target.position;
            offsetPosition = new Vector3(offsetPosition.x, transform.position.y, offsetPosition.z);
            return Vector3.MoveTowards(offsetPosition, transform.position, .95f);
        }

        //Nos llevamos esto aqui en lugar de en el update del enemy detector para solo hacer las comprobaciones
        //en los estados adecuados.
        public virtual void CheckAttackTarget()
        {
            RaycastHit info;
            if (Physics.SphereCast(transform.position, stats.current.sightDetectionRadius,
                    inputs.GetMovementCameraDirection(), out info, stats.current.sightMaxDistance, playerEnemyDetector.targetLayerMask))
            {
                if (info.collider.transform.GetComponent<Enemy>().IsAlive())
                    playerEnemyDetector.SetCurrentTarget(info.collider.transform.GetComponent<Enemy>());
            }
        }
        
        #endregion
        
        public virtual void PickAndThrow()
        {
            if (stats.current.canPickUp && inputs.GetPickAndDropDown())
            {
                if (!holding)
                {
                    if (CapsuleCast(transform.forward,
                            stats.current.pickDistance, out var hit))
                    {
                        if (hit.transform.TryGetComponent(out Pickable pickable))
                        {
                            PickUp(pickable);
                        }
                    }
                } 
                else
                {
                    Throw();
                }
            }
           
        }

        public virtual void PickUp(Pickable pickable)
        {
            if (!holding && (isGrounded || stats.current.canPickUpOnAir))
            {
                holding = true;
                this.pickable = pickable;
                pickable.PickUp(pickableSlot);
                pickable.onRespawn.AddListener(RemovePickable);
                playerEvents.OnPickUp?.Invoke();
            }
        }

        public virtual void Throw()
        {
            if (holding)
            {
                var force = lateralVelocity.magnitude * stats.current.throwVelocityMultiplier;
                pickable.Release(transform.forward, force);
                pickable = null;
                holding = false;
                playerEvents.OnThrow?.Invoke();
            }
        }

        public virtual void RemovePickable()
        {
            if (holding)
            {
                pickable = null;
                holding = false;
            }
        }
        
        public virtual void LedgeGrab()
        {
            if (stats.current.canLedgeHang && verticalVelocity.y < 0 && !holding &&
                states.ContainsStateOfType(typeof(LedgeHangingPlayerState)) &&
                DetectingLedge(stats.current.ledgeMaxForwardDistance,
                stats.current.ledgeMaxDownwardDistance, out var hit)) // Si puede agarrarse a un borde y está cayendo  //!holding agregar si cogemos objetos
            {
                Debug.Log("Entrando al metodo ledgeGrab");
                if(Vector3.Angle(hit.normal, transform.up) > 0) return; // Si el angulo entre la normal y el vector up es mayor que 0 no se puede agarrar.
                if(hit.collider is CapsuleCollider || hit.collider is SphereCollider) return; // Si el collider es una capsula o una esfera no se puede agarrar. 

                var ledgeDistance = radius + stats.current.ledgeMaxForwardDistance; // Distancia del borde
                var lateralOffset = transform.forward * ledgeDistance; // Offset lateral
                var verticalOffset = -transform.up * (height * 0.5f) - center; // Offset vertical
                velocity = Vector3.zero;
                transform.parent = hit.collider.CompareTag(GameTags.Platform) ? hit.transform : initialParent; // Si el collider es una plataforma el jugador se vuelve hijo.
                transform.position = hit.point - lateralOffset + verticalOffset; // Colocamos al jugador en función del punto de contacto, el offset lateral y el vertical.
                states.Change<LedgeHangingPlayerState>(); // Cambiamos al estado de agarrarse a un borde.
                playerEvents.OnLedgeGrabbed?.Invoke(); // Invocamos el evento de agarrarse a un borde.
            } 
           
        }
        
        protected virtual bool DetectingLedge(float forwardDistance, float downwardDistance, out RaycastHit ledgeHit)
        {
            var contactOffset = Physics.defaultContactOffset + positionDelta;
            var ledgeMaxDistance = radius + forwardDistance; // Maxima distancia del edge en horizontal
            var ledgeHeightOffset = height * 0.5f + contactOffset; //Offset del edge en vertical
            var upwardOffset = transform.up * ledgeHeightOffset; // Maxima distancia del edge en vertical
            var forwardOffset = transform.forward * ledgeMaxDistance; // Maxima distancia del edge en horizontal con direccion

            if (Physics.Raycast(position + upwardOffset, transform.forward, ledgeMaxDistance,
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore) || 
                Physics.Raycast(position + forwardOffset * .01f, transform.up, ledgeHeightOffset, 
                    Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                ledgeHit = new RaycastHit();
                return false;
            }
            
            var origin = position + upwardOffset + forwardOffset; // Posicion del raycast
            var distance = downwardDistance + contactOffset; // Distancia del raycast

            return Physics.Raycast(origin, -transform.up, out ledgeHit, distance,
                stats.current.ledgeHangingLayers, QueryTriggerInteraction.Ignore); // Devuelve true si choca con un ledge
        }
        
        public virtual void Dash()
        {
            var canAirDash = stats.current.canAirDash && !isGrounded && (airDashCounter < stats.current.allowedAirDashes);
            var canGroundDash = stats.current.canGroundDash && isGrounded && 
                                Time.time - lastDashTime > stats.current.groundDashCoolDown;
            
            if(inputs.GetDashDown() && (canAirDash || canGroundDash))
            {
                if(!isGrounded) airDashCounter++;
            
                lastDashTime = Time.time;
                states.Change<DashPlayerState>();
                playerEvents.OnDashStarted?.Invoke();
            }
        }

        public virtual void Talk()
        {
            if (inputs.GetInteractDown() && dialogueTrigger.CheckDialogue(out Talker talker))
            {
                talker.FaceDirectionSmooth(transform.position, 800);
                talker.talkerEvents.OnDialogueStarted?.Invoke();
                states.Change<DialoguePlayerState>();
                playerEvents.OnDialogueStarted?.Invoke();
            }
        }
        
        public virtual void HandleMoonLauncherDetection()
        {
            var distance = 5f; //Cambiar por variable

            if (SphereCast(transform.forward, distance, out var hit, Physics.DefaultRaycastLayers, 
                    QueryTriggerInteraction.Collide) && hit.collider.CompareTag(GameTags.MoonLauncher))
            {
                Debug.Log("Detectamos Launcher");
                if (inputs.GetDashDown())
                {
                    Debug.Log("Detectamos Interact");
                    states.Change<MoonFlyPlayerState>();   
                }
            }
        }
        
        public virtual void Glide()
        {
            // if(!isGrounded && inputs.GetGlide() && 
            //    verticalVelocity.y <= 0 && stats.current.canGlide){}
                //states.Change<GlidingPlayerState>();
        }

        /// <summary>
        /// Sets the Skin parent to a given transform.
        /// </summary>
        /// <param name="parent">The transform you want to parent the skin to.</param>
        public virtual void SetSkinParent(Transform parent)
        {
            if(!skin) return;
            
            skin.parent = parent;
        }

        public virtual void ResetSkinParent()
        {
            if(!skin) return;
            skin.parent = transform;
            skin.localPosition = _skinInitialPosition;
            skin.localRotation = _skinInitialRotation;
            skin.localScale = _skinInitialScale;
        }

        public virtual void SetInputEnabled(bool value)
        {
            inputs.enabled = value;
        }
        
        
        public virtual void WallDrag(Collider other)
        {
            if(holding || !stats.current.canWallDrag || verticalVelocity.y > 0) return;

            var maxWallDistance = radius + stats.current.ledgeMaxForwardDistance; // La distancia máxima de la pared para deslizar por esta
            var minGroundDistance = height * 0.5f + stats.current.minGroundDistanceToDrag; // La altura mínima para deslizar por pared
            
            var detectingLedge = DetectingLedge(maxWallDistance, height, out _);
            var detectingWall = SphereCast(transform.forward, maxWallDistance, 
                    out var hit, stats.current.wallDragLayers);
            var detectingGround = SphereCast(-transform.up, minGroundDistance);
            var wallAngle = Vector3.Angle(transform.up, hit.normal);
            
            if(!detectingWall || detectingGround || detectingLedge || wallAngle < stats.current.minWallAngleToDrag) 
                return;
            if (hit.collider.CompareTag(GameTags.Platform))
                transform.parent = hit.transform;

            lastWallNormal = hit.normal;
            states.Change<WallDragPlayerState>();
        }
  
        /// <summary>
        ///  Funcion para las fisicas de desplazamiento al chocar con otros objetos que tienen rigidbody.
        /// </summary>
        /// <param name="other">Collider of the other object.</param>
        public virtual void PushRigidbody(Collider other) =>
            PushRigidbody(other, Quaternion.FromToRotation(Vector3.up, transform.up) * lateralVelocity);
       
        /// <summary>
        ///  Funcion para las fisicas de desplazamiento al chocar con otros objetos que tienen rigidbody.
        /// </summary>
        /// <param name="other">Collider of the other object.</param>
        /// <param name="motion">Velocity and direction of the player (+-).</param>
        public virtual void PushRigidbody(Collider other, Vector3 motion)
        {
            var point = transform.InverseTransformPoint(stepPosition);
            var otherPoint = transform.InverseTransformPoint(other.bounds.center);

            if (point.y <= otherPoint.y && other.TryGetComponent(out Rigidbody rigidbody))
            {
                var motionMag = Mathf.Max(1, motion.magnitude);
                var pushForce = stats.current.pushForce * motionMag;
                var force = motion.normalized * pushForce;
                var closestPoint = other.ClosestPoint(position); //Sacamos el punto del collider mas proximo a la posicion del player
                rigidbody.AddForceAtPosition(force, closestPoint);
            }
        }
        
        public virtual void StartGrind() => states.Change<RailGrindPlayerState>();

        protected override void Awake()
        {
            base.Awake(); // Inicializamos controller, estados y parent
            InitializeInputs();
            InitializeStats();
            InitializeSkin();
            InitializeHealth();
            InitializeDialogueTrigger();
            InitializeTag();
            InitializeRespawn();
            InitializePlayerEnemyDetector();
            InitializePlayerCombat();
            InitializeMoonLauncher();
            moonPathCart = FindObjectOfType<CinemachineDollyCart>();
            launchObject = objectLauncherTest;
            entityEvents.OnGroundEnter.AddListener(() =>
            {
                ResetJumps();
                ResetAirDash();
            });
            
            entityEvents.OnRailsEnter.AddListener(() =>
            {
                ResetJumps();
                ResetAirDash();
                StartGrind();
            });

        }
    }
    
}
