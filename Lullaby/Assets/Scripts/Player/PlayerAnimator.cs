using System;
using System.Collections.Generic;
using Lullaby.Entities.States;
using Lullaby.Entities.Weapons;
using UnityEngine;

namespace Lullaby.Entities
{
    [RequireComponent(typeof(Player))]
    [AddComponentMenu("Lullaby/Platformer Project/Player/Player Animator")]
    public class PlayerAnimator : MonoBehaviour
    {
        [Serializable]
        public class ForcedTransition
        {
            [Tooltip("The index of the Player State from the Player State Manager that you want to force a transition from.")]
            public int fromStateId; //En un futuro se podría cambiar por un desplegable con los estados
            
            [Tooltip("The index of the layer from your Animator Controller that contains the target animation. (It's 0 if the animation is inside the 'Base Layer')")]
            public int animationLayer;
            
            [Tooltip("The name of the Animation State you want to play right after finishing the Player State from above.")]
            public string toAnimationState;
        } 
            
        
        public Animator animator;

        [Header("Parameters Names")] 
        public string stateName = "State";
        public string lastStateName = "Last State";
        public string lateralSpeedName = "Lateral Speed";
        public string verticalSpeedName = "Vertical Speed";
        public string lateralAnimationSpeed = "Lateral Animation Speed";
        public string healthName = "Health";
        public string jumpCounterName = "Jump Counter";
        public string isGroundedName = "IsGrounded";
        public string isHoldingName = "IsHolding";
        public string onStateChangedName = "OnStateChanged";
        public string attackTriggerName = "AttackTrigger";
        public string railDashTriggerName = "RailDashTrigger";
        public string flyPathName = "FlyPath";
        
        [Header("Settings")] 
        public float minLateralAnimationSpeed = 0.5f;
        public List<ForcedTransition> forcedTransitions;

        protected int _stateHash;
        protected int _lastStateHash;
        protected int _lateralSpeedHash;
        protected int _verticalSpeedHash;
        protected int _lateralAnimationSpeedHash;
        protected int _healthHash;
        protected int _jumpCounterHash;
        protected int _isGroundedHash;
        protected int _isHoldingHash;
        protected int _onStateChangedHash;
        protected int _attackTriggerHash;
        protected int _railDashTriggerHash;
        protected int _flyPathHash;
        
        protected Dictionary<int, ForcedTransition> m_forcedTransitions;

        protected Player _player;
        protected MeleeWeapon _meleeWeapon;

        protected virtual void InitializePlayer()
        {
            _player = GetComponent<Player>();
            _player.states.events.onChange.AddListener(HandleForcedTransitions);
        }
        protected virtual void InitializeMeleeWeapon()
        {
            _meleeWeapon = GetComponentInChildren<MeleeWeapon>();
        }
        protected virtual void InitializeForcedTransitions()
        {
            m_forcedTransitions = new Dictionary<int, ForcedTransition>();

            foreach (var transition in forcedTransitions)
            {
                if (!m_forcedTransitions.ContainsKey(transition.fromStateId))
                {
                    m_forcedTransitions.Add(transition.fromStateId, transition);
                }
            }
        }
        
        protected virtual void InitializeAnimatorTriggers()
        {
            _player.states.events.onChange.AddListener(() => animator.SetTrigger(_onStateChangedHash));
        }

        protected virtual void InitializeParametersHash()
        {
            _stateHash = Animator.StringToHash(stateName);
            _lastStateHash = Animator.StringToHash(lastStateName);
            _lateralSpeedHash = Animator.StringToHash(lateralSpeedName);
            _verticalSpeedHash = Animator.StringToHash(verticalSpeedName);
            _lateralAnimationSpeedHash = Animator.StringToHash(lateralAnimationSpeed);
            _healthHash = Animator.StringToHash(healthName);
            _jumpCounterHash = Animator.StringToHash(jumpCounterName);
            _isGroundedHash = Animator.StringToHash(isGroundedName);
            _isHoldingHash = Animator.StringToHash(isHoldingName);
            _onStateChangedHash = Animator.StringToHash(onStateChangedName);
            _attackTriggerHash = Animator.StringToHash(attackTriggerName);
            _railDashTriggerHash = Animator.StringToHash(railDashTriggerName);
            _flyPathHash = Animator.StringToHash(flyPathName);
        }

        protected virtual void HandleForcedTransitions()
        {
            var lastStateIndex = _player.states.lastIndex;

            if (m_forcedTransitions.ContainsKey(lastStateIndex))
            {
                var layer = m_forcedTransitions[lastStateIndex].animationLayer;
                animator.Play(m_forcedTransitions[lastStateIndex].toAnimationState, layer);
            }
        }

        protected virtual void HandleAnimatorParameters()
        {
            var lateralSpeed = _player.lateralVelocity.magnitude;
            var verticalSpeed = _player.verticalVelocity.y;
            var lateralAnimationSpeed =
                Mathf.Max(minLateralAnimationSpeed, lateralSpeed / _player.stats.current.topSpeed);
            
            animator.SetInteger(_stateHash, _player.states.index);
            animator.SetInteger(_lastStateHash, _player.states.lastIndex);
            animator.SetFloat(_lateralSpeedHash, lateralSpeed);
            animator.SetFloat(_verticalSpeedHash, verticalSpeed);
            animator.SetFloat(_lateralAnimationSpeedHash, lateralAnimationSpeed);
            animator.SetInteger(_healthHash, _player.health.current);
            animator.SetInteger(_jumpCounterHash, _player.jumpCounter);
            animator.SetBool(_isGroundedHash, _player.isGrounded);
            animator.SetBool(_isHoldingHash, _player.holding);
            // if (_player.inputs.GetAttackDown())
            // {
            //     //animator.SetTrigger(_attackTriggerHash);
            // }

            if (_player.inputs.GetDashDown() && _player.states.IsCurrentOfType(typeof(RailGrindPlayerState)))
            {
                animator.SetTrigger(_railDashTriggerHash);
            }

            if (_player.states.IsCurrentOfType(typeof(MoonFlyPlayerState)))
            {
                animator.SetFloat(_flyPathHash, _player.moonLauncher.moonPathCart.m_Position);
            }
        }

        protected virtual void Start()
        {
            InitializePlayer();
            InitializeMeleeWeapon();
            InitializeForcedTransitions();
            InitializeParametersHash();
            InitializeAnimatorTriggers();
           
        }

        protected void LateUpdate() => HandleAnimatorParameters();
        
    }
}