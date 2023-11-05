using System;
using Lullaby.Entities;
using UnityEngine;

namespace Lullaby
{
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("Lullaby/Misc/Kill Zone")]
    public class KillZone : MonoBehaviour
    {
        protected Collider _collider;

        protected void Start()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag(GameTags.Player))
            {
                if (other.TryGetComponent(out Player player))
                {
                    player.playerEvents.OnDeadlyFall?.Invoke();
                }
            }
        }
    }
}