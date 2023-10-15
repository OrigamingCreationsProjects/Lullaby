using UnityEngine;

namespace Lullaby.Entities.States
{
    public class HurtPlayerState : PlayerState
    {
        protected override void OnEnter(Player player) { }

        protected override void OnExit(Player player) { }

        public override void OnStep(Player player)
        {
            player.ApplyGravity();

            if (player.isGrounded && (player.verticalVelocity.y <= 0))
            {
                if (player.health.current > 0)
                {
                    player.states.Change<IdlePlayerState>();
                }
                else
                {
                    player.states.Change<DiePlayerState>();
                }
            }
        }

        public override void OnContact(Player player, Collider other) { }
    }
}