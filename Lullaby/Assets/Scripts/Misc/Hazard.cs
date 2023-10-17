using System;
using Lullaby.Entities;
using UnityEngine;

namespace Lullaby
{
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("Lullaby/Misc/Hazard")]
    public class Hazard : MonoBehaviour, IEntityContact
    {
        public bool isSolid; // Guardamos esta variable para que luego establezcamos el collider como trigger o no
        public bool damageOnlyFromAbove; // Si es true, solo dañará al jugador si este está cayendo
        public int damage = 5;

        protected Collider collider;

        protected virtual void Awake()
        {
            tag = GameTags.Hazard;
            collider = GetComponent<Collider>();
            collider.isTrigger = !isSolid; // Si no es solido, es un trigger
        }

        protected virtual void TryToApplyDamageTo(Player player)
        {
            if (!damageOnlyFromAbove || player.verticalVelocity.y < 0 &&
                BoundsHelper.IsBellowPoint(collider, player.stepPosition))
            {
                player.ApplyDamage(damage, transform.position);
            }
        }
        
        public void OnEntityContact(Entity entity)
        {
            if (entity is Player player)
            {
                TryToApplyDamageTo(player);
            }
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(GameTags.Player))
            {
                if (other.TryGetComponent<Player>(out var player))
                {
                    TryToApplyDamageTo(player);
                }
            }
        }
    }
}