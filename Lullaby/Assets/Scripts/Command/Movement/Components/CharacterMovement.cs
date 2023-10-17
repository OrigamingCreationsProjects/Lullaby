using System;
using Lullaby.Entities;

using UnityEngine;

namespace Movement.Components
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(PlayerStatsManager))]
    public sealed class CharacterMovement: MonoBehaviour, IMoveableReceiver, IJumperReceiver
    {
        public float speed = 10.0f;
        public float jumpAmount = 1.0f;
        public float gravityScale = 1.0f;
        private Rigidbody _characterRb;
        private Vector2 _direction = Vector2.zero;
        
        /// <summary>
        /// Returns the Player Stats Manager instance.
        /// </summary>
        public PlayerStatsManager stats { get; protected set; }
        
        //COMPONENTES DE LA ENTIDAD
        public float accelerationMultiplier { get; set; } = 1f;
        public Vector3 velocity { get; set; }
        public Vector3 localVelocity
        {
            get { return Quaternion.FromToRotation(transform.up, Vector3.up) * velocity; }
            set { velocity = Quaternion.FromToRotation(Vector3.up, transform.up) * value; }
        }
        public Vector3 lateralVelocity
        {
            get
            {
                var value = new Vector3(localVelocity.x, 0, localVelocity.z);

                if (value.sqrMagnitude < 0.0001f)
                    return Vector3.zero;

                return value;
            }

            set { localVelocity = new Vector3(value.x, localVelocity.y, value.z); }
        }
        public float topSpeedMultiplier { get; set; } = 1f;

        public float turningDragMultiplier { get; set; } = 1f;
        void Start()
        {
            _characterRb = GetComponent<Rigidbody>();
        }
        /// <summary>
        /// Returns the original height of this Entity.
        /// </summary>
        public float originalHeight { get; protected set; }
        
        /// <summary>
        /// Returns the Character Controller of this Entity.
        /// </summary>
        public EntityController controller { get; protected set; }

        protected void InitializeController()
        {
            controller = GetComponent<EntityController>();

            if (!controller)
            {
                controller = gameObject.AddComponent<EntityController>();
            }

            originalHeight = controller.height;
        }

        private void Awake()
        {
            InitializeController();
        }

        private void Update()
        {
            HandleController();
        }

        private void FixedUpdate()
        {
            //_characterRb.velocity = new Vector3(_direction.x, _characterRb.velocity.y, _direction.y);
        }
        protected void HandleController()
        {
            if (controller.enabled)
            {
                controller.Move(velocity * Time.deltaTime);
                return;
            }
            transform.position += velocity * Time.deltaTime;
        }
        public void Move(Vector3 direction)
        {
            Debug.Log("Move: " + direction.ToString());
            // if (direction == Vector2.zero)
            // { 
            //     _direction = Vector3.zero;
            //     return;
            // }
            // _direction =  speed * direction;
            var dot = Vector3.Dot(direction, lateralVelocity);
            if (dot >= stats.current.brakeThreshold)
            {
                var turningDrag = stats.current.turningDrag;
                var acceleration = stats.current.acceleration;
                var finalAcceleration = acceleration;
                var topSpeed = stats.current.topSpeed;

                Accelerate(direction, turningDrag, finalAcceleration, topSpeed);
            }
            

        }
        public void Accelerate(Vector3 direction, float turningDrag, float acceleration, float topSpeed)
        {
            if (direction.sqrMagnitude > 0)
            {
                var speed = Vector3.Dot(direction, lateralVelocity);
                var velocity = direction * speed;
                var turningVelocity = lateralVelocity - velocity;
                var turningDelta = turningDrag * turningDragMultiplier * Time.deltaTime;
                var targetTopSpeed = topSpeed * topSpeedMultiplier;

                if (lateralVelocity.magnitude < targetTopSpeed || speed < 0)
                {
                    speed += acceleration * accelerationMultiplier * Time.deltaTime;
                    speed = Mathf.Clamp(speed, -targetTopSpeed, targetTopSpeed);
                }

                velocity = direction * speed;
                turningVelocity = Vector3.MoveTowards(turningVelocity, Vector3.zero, turningDelta);
                lateralVelocity = velocity + turningVelocity;
            }
        }
        public void Jump(IJumperReceiver.JumpStage stage)
        {
            float jumpForce = Mathf.Sqrt(jumpAmount * -2.0f * (Physics.gravity.y * gravityScale));
            _characterRb.AddForce(Vector2.up * jumpForce, ForceMode.Impulse);
        }
    }
}