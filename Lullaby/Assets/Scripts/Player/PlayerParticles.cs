using System;
using Unity.VisualScripting;
using UnityEngine;

namespace Lullaby.Entities
{
    [RequireComponent(typeof(Player))]
    [AddComponentMenu("Lullaby/Entities/Player Particles")]
    public class PlayerParticles : MonoBehaviour
    {
        public float walkDustMinSpeed = 3.5f;
        public float landingParticleMinSpeed = 5f;

        public ParticleSystem walkDust;
        public ParticleSystem landDust;
        public ParticleSystem dashDust;
        public ParticleSystem dashTrail;
        public ParticleSystem speedTrails;
        public ParticleSystem grindTrails;
        public ParticleSystem jumpDust;
        public ParticleSystem jumpRing;
        public ParticleSystem pillowSpawn;

        protected Player _player;

        
        /// <summary>
        /// Start playing a given particle
        /// </summary>
        /// <param name="particle">The particle you want to play.</param>
        public virtual void Play(ParticleSystem particle)
        {
            if (!particle.isPlaying)
            {
                particle.Play();
            }
        }

        /// <summary>
        /// Stop playing a given particle
        /// </summary>
        /// <param name="particle"></param>
        public virtual void Stop(ParticleSystem particle, bool clear = false)
        {
            if (particle.isPlaying)
            {
                var mode = clear
                    ? ParticleSystemStopBehavior.StopEmittingAndClear
                    : ParticleSystemStopBehavior.StopEmitting;
                particle.Stop(true, mode);
            }
        }

        protected virtual void HandleWalkParticle()
        {
            if (_player.isGrounded && !_player.onRails)
            {
                if (_player.lateralVelocity.magnitude > walkDustMinSpeed)
                {
                    Play(walkDust);
                }
                else
                {
                    Stop(walkDust);
                }
            }
            else
            {
                Stop(walkDust);
            }
        }

        protected virtual void HandleRailParticle()
        {
            if(_player.onRails)
                Play(grindTrails);
            else
                Stop(grindTrails, true);
        }

        protected virtual void HandleLandParticle()
        {
            if (Math.Abs(_player.velocity.y) >= landingParticleMinSpeed)
            {
                Play(landDust);
            }
        }
        protected virtual void HandleJumpParticle()
        {
            Play(jumpDust);
            Play(jumpRing);
        }

        protected virtual void OnDashStarted()
        {
            if(dashDust)
                Play(dashDust);
            Play(speedTrails);
            Play(dashTrail);
        }

        protected virtual void OnDashEnded()
        {
            Stop(speedTrails, true);
            Stop(dashTrail);
        }

        protected virtual void Start()
        {
            _player = GetComponent<Player>();
            _player.entityEvents.OnGroundEnter.AddListener(HandleLandParticle);
            _player.playerEvents.OnDashStarted.AddListener(OnDashStarted);
            _player.playerEvents.OnDashEnded.AddListener(OnDashEnded);
            _player.playerEvents.OnJump.AddListener(HandleJumpParticle);
            //_player.playerEvents.OnAttackStarted.AddListener(pillowSpawn.Play);
        }

        protected void Update()
        {
            HandleWalkParticle();
            HandleRailParticle();
            
        }
    }
}