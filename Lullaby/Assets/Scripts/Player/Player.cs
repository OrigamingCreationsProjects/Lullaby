using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using DG.Tweening;
using Lullaby.Entities.Events;
using Lullaby.Entities.States;
using PLAYERTWO.PlatformerProject;
using Systems;
using UnityEngine;
using UnityEngine.InputSystem;
using PlayerInputManager = MovementEntitys.PlayerInputManager;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Lullaby.Entities
{
    [AddComponentMenu("Lullaby/Custom Movement/Player/Player")]
    [RequireComponent(typeof(PlayerInputManager))]
    [RequireComponent(typeof(PlayerStatsManager))]
    [RequireComponent(typeof(PlayerStateManager))]
    public class Player : Entity<Player>
    {
        //Eventos
        public PlayerEvents playerEvents;
        public Transform pickableSlot; //Slot para posicionar el objeto que se puede recoger

        public Transform skin;
        
        protected Vector3 respawnPosition;
        protected Quaternion respawnRotation;
        
        protected Vector3 skinInitialPosition;
        protected Quaternion skinInitialRotation;
        
        /// <summary>
        /// Returns the Player Input Manager instance.
        /// </summary>
        public PlayerInputManager inputs { get; protected set; }
        /// <summary>
        /// Returns the Player Stats Manager instance.
        /// </summary>
        public PlayerStatsManager stats { get; protected set; }
        
        //SALUD
        
        
        //COGER OBJETOS?
        
        
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
        /// The last time the Player performed an dash.
        /// </summary>
        /// <value></value>
        public float lastDashTime { get; protected set; }
        
        /// <summary>
        /// Returns the normal of the last wall the Player touched.
        /// </summary>
        public Vector3 lastWallNormal { get; protected set; }
        
        //HEALTH ALIVE
        
        /// <summary>
        /// Returns true if the Player can stand up.
        /// </summary>
        public virtual bool canStandUp => !SphereCast(transform.up, originalHeight * 0.5f);

        #region -- INITIALIZERS --
        
        //INPUT
        protected virtual void InitializeInputs() => inputs = GetComponent<PlayerInputManager>();
        protected virtual void InitializeStats() => stats = GetComponent<PlayerStatsManager>();
        // HEALTH
        protected virtual void InitializeTag() => tag = GameTags.Player;
        
        protected virtual void InitializeRespawn()
        {
            respawnPosition = transform.position;
            respawnRotation = transform.rotation;
        }

        protected virtual void InitializeSkin()
        {
            if(!skin) return;
            skinInitialPosition = skin.localPosition;
            skinInitialRotation = skin.localRotation;
        }
        
        #endregion

        #region -- RESPAWN --
        /// <summary>
        /// Resets Player state, health, position, and rotation.
        /// </summary>
        public virtual void Respawn()
        {
            //RESET HEALTH

            velocity = Vector3.zero;
            transform.SetPositionAndRotation(respawnPosition, respawnRotation);
            states.Change<IdlePlayerState>();
        }

        /// <summary>
        /// Sets the position and rotation of the Player for the next respawn.
        /// </summary>
        public virtual void SetRespawn(Vector3 position, Quaternion rotation)
        {
            respawnPosition = position;
            respawnRotation = rotation;
        }
        #endregion
        
        //public override void ApplyDamage(int amount, Vector3 origin){}
        
        //public virtual void Die() { }

        protected override bool EvaluateLanding(RaycastHit hit)
        {
            return base.EvaluateLanding(hit) && !hit.collider.CompareTag(GameTags.Spring); //Si implementamos las zonas de viento
        }
        
        /// <summary>
        /// Moves the Player smoothly in a given direction.
        /// </summary>
        /// <param name="direction">The direction you want to move.</param>
        public virtual void Accelerate(Vector3 direction)
        {
            var turningDrag = stats.current.turningDrag;
            var acceleration = stats.current.acceleration;
            var finalAcceleration = isGrounded ? acceleration : stats.current.airAcceleration;
            var topSpeed = stats.current.topSpeed;
            
            Accelerate(direction, turningDrag, finalAcceleration, topSpeed);
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
            var holdJump = stats.current.canJumpWhileHolding;   // !holding ; Comentamos esto porque no sabemos si se cogerán objetos

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
            
            if(inputs.GetJumpUp() && (jumpCounter > 0) && (verticalVelocity.y > stats.current.minJumpHeight)) //
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
        /// Sets the jump couter to a specific value.
        /// </summary>
        /// <param name="amount">The amount of jumps.</param>
        public virtual void SetJumps(int amount) => jumpCounter = amount;
        
        //IMPLEMENTAR
        public virtual void LedgeGrab()
        {
            if (stats.current.canLedgeHang && verticalVelocity.y < 0 && 
                states.ConstainsStateOfType(typeof(LedgeHangingPlayerState)) &&
                DetectingLedge(stats.current.ledgeMaxForwardDistance,
                stats.current.ledgeMaxDownwardDistance, out var hit)) // Si puede agarrarse a un borde y está cayendo  //!holding añadir si cogemos objetos
            {
                Debug.Log("Entrando al metodo ledgeGrab");
                if(Vector3.Angle(hit.normal, transform.up) > 0) return; // Si el ángulo entre la normal y el vector up es mayor que 0 no se puede agarrar.
                if(hit.collider is CapsuleCollider || hit.collider is SphereCollider) return; // Si el collider es una cápsula o una esfera no se puede agarrar. 

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
            skin.localPosition = skinInitialPosition;
            skin.localRotation = skinInitialRotation;
        }

        public virtual void WallDrag(Collider other)
        {
            if(!stats.current.canWallDrag || verticalVelocity.y > 0) return; //holding añadir esta condicion si metemos coger objetos

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
        
        //public virtual void StartGrind() => states.Change<RailGrindPlayerState>();

        protected override void Awake()
        {
            base.Awake(); // Inicializamos controller, estados y parent
            InitializeInputs();
            InitializeStats();
            //InitializeHealth();
            InitializeTag();
            InitializeRespawn();

            entityEvents.OnGroundEnter.AddListener(() =>
            {
                ResetJumps();
                ResetAirDash();
            });
            
            entityEvents.OnRailsEnter.AddListener(() =>
            {
                ResetJumps();
                ResetAirDash();
                //StartGrind();
            });

        }
    }
    
}
