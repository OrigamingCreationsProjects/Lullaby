using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using Lullaby.Entities;
using Lullaby.Systems.DialogueSystem;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Lullaby
{
    [AddComponentMenu("Lullaby/Custom Movement/Player/Player Input Manager")]
    public class PlayerInputManager : MonoBehaviour
    {
        public InputActionAsset actions;

        protected InputAction Move;
        protected InputAction Run;
        protected InputAction Jump;
        protected InputAction Look;
        protected InputAction Attack;
        protected InputAction Dash;
        protected InputAction ReleaseLedge;
        protected InputAction Pause;
        protected InputAction GrindBrake;
        protected InputAction Interact;
        protected InputAction PickAndDrop;

        protected Camera camera;
        protected float movementDirectionUnlockTime;
        protected float? lastJumpTime;
        protected float? lastInteractTime;

        protected const string mouseDeviceName = "Mouse";

        protected const float jumpBuffer = 0.15f;
        protected const float interactCooldown = 0.2f;

        protected virtual void CacheActions()
        {
            Move = actions["Move"];
            Run = actions["Run"];
            Jump = actions["Jump"];
            Look = actions["Look"];
            Attack = actions["Attack"];
            Dash = actions["Dash"];
            ReleaseLedge = actions["ReleaseLedge"];
            Interact = actions["Interact"];
            Pause = actions["Pause"];
            GrindBrake = actions["GrindBrake"];
            PickAndDrop = actions["PickAndDrop"];
        }

        #region -- GET DIRECTIONS --
        public virtual Vector3 GetMovementDirection()
        {
            if (Time.time < movementDirectionUnlockTime) return Vector3.zero;

            var value = Move.ReadValue<Vector2>();
            return GetAxisWithCrossDeadZone(value);
        }

        public virtual Vector3 GetLookDirection()
        {
            var value = Look.ReadValue<Vector2>();
            if (IsLookingWithMouse())
                return new Vector3(value.x, 0, value.y) * GameManager.instance.cameraSensitivity;

            return GetAxisWithCrossDeadZone(value);
        }

        public virtual Vector3 AdjustSensitivity(Vector3 direction, float sensitivity) =>
            direction * sensitivity; // Adjust the sensitivity of the direction
        
        public virtual Vector3 GetMovementCameraDirection() // Get the direction of the movement relative to the camera
        {
            var direction = GetMovementDirection();

            if (direction.sqrMagnitude > 0) // If we are moving
            {
                var rotation =
                    Quaternion.FromToRotation(camera.transform.up,
                        transform.up); // Necessary rotation from camera rot to player rot
                direction = rotation * camera.transform.rotation *
                            direction; // Rotate the direction to match the camera
                direction = Vector3.ProjectOnPlane(direction,
                    transform.up); // Project the direction on the plane of the player
                direction = Quaternion.FromToRotation(transform.up, Vector3.up) * direction;
                direction = direction.normalized;
            }

            return direction;
        }
        #endregion
        
        
        /// <summary>
        /// Remaps a given axis considering the Input System's default deadzone.
        /// This method uses a cross shape instead of a circle one to evaluate the deadzone range.
        /// </summary>
        /// <param name="axis">The axis you want to remap.</param>
        public virtual Vector3 GetAxisWithCrossDeadZone(Vector2 axis)
        {
            var deadzone = InputSystem.settings.defaultDeadzoneMin;
            axis.x = Mathf.Abs(axis.x) > deadzone ? RemapToDeadzone(axis.x, deadzone) : 0; // If the axis is greater than the deadzone, remap it
            axis.y = Mathf.Abs(axis.y) > deadzone ? RemapToDeadzone(axis.y, deadzone) : 0;
            return new Vector3(axis.x, 0, axis.y);
        }

        protected float RemapToDeadzone(float value, float deadzone) => // Is like normalize a vector between a custom values
            Mathf.Sign(value) * ((Mathf.Abs(value) - deadzone) / (1 - deadzone)); // Remap the value to the deadzone


        public virtual bool IsLookingWithMouse()
        {
            if (Look.activeControl == null)
            {
                return false;
            }

            return Look.activeControl.device.name.Equals(mouseDeviceName);
        }

        #region -- GET INPUT PRESSED --
        
        public virtual bool GetRun() => Run.IsPressed();
        public virtual bool GetRunUp() => Run.WasReleasedThisFrame();
        
        public virtual bool GetJumpDown()
        {
            //Cuidado con HASVALUE, si no habra que usar != null
            if (lastJumpTime != null && Time.time - lastJumpTime.Value < jumpBuffer)
            {
                lastJumpTime = null;
                return true;
            }

            return false;
        }

        public virtual bool GetJumpUp() => Jump.WasReleasedThisFrame(); 

        public virtual bool GetDashDown() => Dash.WasPressedThisFrame();
        
        public virtual bool GetAttackDown() => Attack.WasPressedThisFrame();
        public virtual bool GetPickAndDropDown() => PickAndDrop.WasPressedThisFrame();
        public virtual bool GetReleaseLedgeDown() => ReleaseLedge.WasPressedThisFrame();
        
        //public virtual bool GetInteractDown() => Interact.WasPerformedThisFrame();

        public virtual bool GetInteractDown()
        {
            if (lastInteractTime != null && Time.time - lastInteractTime.Value > interactCooldown)
            {
                lastInteractTime = null;
                return true;
            }

            return false;
        }
        
        public virtual bool GetPauseDown() => Pause.WasPressedThisFrame();

        public virtual bool GetGrindBrake() => GrindBrake.IsPressed();

        public virtual void SetInteractDialogue(bool value)
        {
            if (value)
            {
                //Interact.performed += DialogueInterfaceManager.Instance.SkipNewDialogue;
            }
            else
            {
                //Interact.performed -= DialogueInterfaceManager.Instance.SkipNewDialogue;
            }
        }
        #endregion
        
        /// <summary>
        /// Temporary locks the movement direction input
        /// </summary>
        /// <param name="duration">The duration of the locking state in seconds</param>

        public virtual void LockedMovementDirection(float duration = 0.25f) =>
            movementDirectionUnlockTime = Time.time + duration;

        public virtual void Awake() => CacheActions();

        // Start is called before the first frame update
        protected virtual void Start()
        {
            camera = Camera.main;
            actions.Enable();
        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (Jump.WasPressedThisFrame())
            {
                
                lastJumpTime = Time.time;
            }

            if (Interact.WasPressedThisFrame())
            {
                lastInteractTime = Time.time;
            }
        }

        protected void OnEnable() => actions?.Enable();
        protected void OnDisable() => actions?.Disable();

    }
}
