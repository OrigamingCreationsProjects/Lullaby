using UnityEngine;

namespace Lullaby.Entities.States
{
    [AddComponentMenu("Lullaby/CustomMovement/Player/States/Attack Player State")]
    public class AttackPlayerState: PlayerState
    {
        protected override void OnEnter(Player player) { }

        protected override void OnExit(Player player) {}

        public override void OnStep(Player player)
        {
            player.ApplyGravity();
            player.SnapToGround();
            //player.AccelerateToInputDirection();

            // if (timeSinceEntered >= player.stats.current.attackDuration)
            // {
            //     if(player.isGrounded)
            //     {
            //         player.states.Change<IdlePlayerState>();
            //     }
            //     else
            //     {
            //         player.states.Change<FallPlayerState>();
            //     }
            // }
        }

        public override void OnContact(Player player, Collider other) { }
    }
}