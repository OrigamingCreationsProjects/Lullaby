using UnityEngine;

namespace Lullaby.Entities.States
{
    public class FallPlayerState : PlayerState
    {
        protected override void OnEnter(Player player){}

        protected override void OnExit(Player player){}

        public override void OnStep(Player player)
        {
            player.ApplyGravity();
            player.SnapToGround();
            player.FaceDirectionSmooth(player.lateralVelocity);
            player.AccelerateToInputDirection();
            player.Jump();
            player.LedgeGrab();
            player.Dash();
            player.Glide();
            
            if (player.isGrounded)
            {
                player.states.Change<IdlePlayerState>();
            }
        }

        public override void OnContact(Player player, Collider other)
        {
            player.PushRigidbody(other);
            player.WallDrag(other);
        }
    }
}