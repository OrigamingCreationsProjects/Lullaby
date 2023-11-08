using System;
using Lullaby.Entities.States;
using UnityEngine;

namespace Lullaby.Entities
{
    [RequireComponent(typeof(Player))]
    [AddComponentMenu("Lullaby/Custom Movement/Player/Player Lean")]
    public class PlayerLean : MonoBehaviour
    {
        public Transform target;
        public float maxTiltAngle = 15;
        public float tiltSmoothTime = 0.2f;
        
        protected Player _player;
        protected Quaternion _initialRotation;
        
        protected float _velocity;

        /// <summary>
        /// Returns true if the Player should be able to lean.
        /// </summary>
        public virtual bool CanLean()
        {
            var walking = _player.states.IsCurrentOfType(typeof(WalkPlayerState));
            return walking;
        }

        protected void Awake()
        {
            _player = GetComponent<Player>();
        }

        protected void LateUpdate()
        {
            var inputDirection = _player.inputs.GetMovementCameraDirection();
            var moveDirection = _player.lateralVelocity.normalized;
            var angle = Vector3.SignedAngle(inputDirection, moveDirection, Vector3.up);
            var amount = CanLean() ? Mathf.Clamp(angle, -maxTiltAngle, maxTiltAngle) : 0;
            var rotation = target.localEulerAngles;
            rotation.z = Mathf.SmoothDampAngle(rotation.z, amount, ref _velocity, tiltSmoothTime);
            target.localEulerAngles = rotation;
        }
    }
}