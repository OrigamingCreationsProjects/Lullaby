using System;
using Lullaby.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace Lullaby
{
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("Lullaby/Misc/Checkpoint")]
    public class Checkpoint : MonoBehaviour
    {
        public Transform respawn;
        public AudioClip checkpointReachedClip;

        /// <summary>
        /// Invoked when the checkpoint is activated.
        /// </summary>
        public UnityEvent OnActivate;

        protected Collider _collider;
        protected AudioSource _audioSource;
        
        /// <summary>
        /// Return if the checkpoint is activated
        /// </summary>
        public bool activated { get; protected set; }

        /// <summary>
        /// Activates this Checkpoint and set the Player respawn transform.
        /// </summary>
        /// <param name="player">The player you want to set the respawn.</param>
        public virtual void Activate(Player player)
        {
            if (!activated)
            {
                activated = true;
                _audioSource.PlayOneShot(checkpointReachedClip);
                player.SetRespawn(respawn.position, respawn.rotation);
                OnActivate?.Invoke();
            }
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (!activated && other.CompareTag(GameTags.Player))
            {
                if (other.TryGetComponent<Player>(out var player))
                {
                    Activate(player);
                }
            }
        }

        protected void Awake()
        {
            if (!TryGetComponent(out _audioSource))
            {
                _audioSource = gameObject.AddComponent<AudioSource>();
            }

            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
        }
    }
}