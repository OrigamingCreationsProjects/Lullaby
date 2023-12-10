using UnityEngine;

namespace Lullaby.Entities.Enemies
{
    [AddComponentMenu("Lullaby/Enemies/Dolly Particles")]
    public class DollyParticles : MonoBehaviour
    {
        public float walkDustMinSpeed = 3.5f;
        public float landingParticleMinSpeed = 5f;

        public ParticleSystem appearSmoke;
        public ParticleSystem deathSmoke;
        public ParticleSystem hitParticle;
        public ParticleSystem[] attackParticles;

        protected Dolly _dolly;

        
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

        protected virtual void PlayDisappearSmoke()
        {
            Play(deathSmoke);
        } 
        protected virtual void PlayAppearSmoke()
        {
            Play(appearSmoke);
        }
        protected virtual void PlayAttackParticle(bool attacking)
        {
            if (attacking)
            {
                for (int i = 0; i < attackParticles.Length; i++)
                {
                    Play(attackParticles[i]);
                }
            } 
            else 
            {
                for (int i = 0; i < attackParticles.Length; i++)
                {
                    Stop(attackParticles[i]);
                }
            }
        }

        protected virtual void Start()
        {
            _dolly = GetComponent<Dolly>();
            if (_dolly == null)
            {
                _dolly = GetComponentInParent<Dolly>();
            }
            _dolly.dollyEvents.OnDisappear.AddListener(PlayDisappearSmoke);
            _dolly.dollyEvents.OnAppear.AddListener(PlayAppearSmoke);
            _dolly.dollyEvents.OnAttack.AddListener(PlayAttackParticle);
        }
        
    }
}