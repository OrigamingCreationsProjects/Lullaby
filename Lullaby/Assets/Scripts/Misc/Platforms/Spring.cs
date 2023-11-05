using System;
using System.Collections;
using System.Collections.Generic;
using Lullaby.Entities;
using Lullaby.Entities.States;
using UnityEngine;

namespace Lullaby
{
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("Lullaby/Misc/Spring")]
    public class Spring : MonoBehaviour, IEntityContact
    {
        public float force = 25f;
        public AudioClip clip;

        protected AudioSource _audio;
        protected Collider _collider;

        public void ApplyForce(Player player)
        {
            if(player.verticalVelocity.y <= 0)
            {
                _audio.PlayOneShot(clip);
                player.verticalVelocity = transform.up * force;
            }
        }
        
        public void OnEntityContact(Entity entity)
        {
            if(!entity.CompareTag(GameTags.Player)) return;

            if (entity is Player player && player.isAlive &&
                BoundsHelper.IsBellowPoint(_collider, entity.stepPosition)) // Si el jugador esta vivo y esta debajo del collider
            {
                ApplyForce(player);
                player.SetJumps(1);
                player.ResetAirDash();
                player.states.Change<FallPlayerState>();
            }
        }
        
        protected virtual void Start()
        {
            tag = GameTags.Spring;
            _collider = GetComponent<Collider>();
            
            if(!TryGetComponent(out _audio))
            {
                _audio = gameObject.AddComponent<AudioSource>();
            }
        }
    }
}
