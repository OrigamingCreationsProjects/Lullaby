using UnityEngine;

namespace Lullaby.Entities.States
{
    public class DiePlayerState: PlayerState
    {
        protected override void OnEnter(Player player) { }

        protected override void OnExit(Player player) { }

        public override void OnStep(Player player)
        {
            player.ApplyGravity();
            player.ApplyFriction();
            player.SnapToGround();
        }

        public override void OnContact(Player player, Collider other) { }
    }
}