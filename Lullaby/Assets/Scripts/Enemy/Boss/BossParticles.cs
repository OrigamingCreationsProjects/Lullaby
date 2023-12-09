using System;
using UnityEngine;

namespace Lullaby.Entities.Enemies
{
    [AddComponentMenu("Lullaby/Entities/Boss Particles")]
    public class BossParticles : MonoBehaviour
    {
        public ParticleSystem spawnParticles;
        public ParticleSystem deathParticles;
        public ParticleSystem projectileParticles;
        public ParticleSystem projectileExplosionParticles;
        
        public Gradient[] smokeGradients;
        
        protected BossEnemy _boss;
        
        
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

        private void OnCloningParticle()
        {
            Play(spawnParticles);
        }
        
        private void OnDamageParticle()
        {
            //deathParticles.Play();
            
            Play(deathParticles);
        }
        
        protected virtual void Awake()
        {
            _boss = GetComponent<BossEnemy>();
            _boss.enemyEvents.OnCloning.AddListener(OnCloningParticle);
            _boss.enemyEvents.OnDamage.AddListener(OnDamageParticle);
        }

        protected void Start()
        {
            var main = spawnParticles.main;
            main.startColor =  new ParticleSystem.MinMaxGradient(GetComponent<BossEnemy>().smokeGradient);
        }
    }
}