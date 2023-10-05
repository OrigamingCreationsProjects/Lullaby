using UnityEngine;

namespace Lullaby.Entities.States
{
    public class WalkPlayerState : PlayerState
    {
        protected override void OnEnter(Player player){}

        protected override void OnExit(Player player){}

        public override void OnStep(Player player)
        {
            player.ApplyGravity();
            player.SnapToGround();
            player.Jump();
            player.Fall();
            //player.Dash();
            player.RegularSlopeFactor();
            
            var inputDirection = player.inputs.GetMovementCameraDirection();

            if (inputDirection.sqrMagnitude > 0)
            {
                var dot = Vector3.Dot(inputDirection, player.lateralVelocity); // Difference between input direction and current velocity

                if (dot >= player.stats.current.brakeThreshold)
                {
                    player.Accelerate(inputDirection);
                    player.FaceDirectionSmooth(player.lateralVelocity);
                }
                else
                {
                    player.states.Change<BrakePlayerState>();
                }
            }
            else
            {
                player.ApplyFriction();

                if (player.lateralVelocity.sqrMagnitude <= 0)
                {
                    player.states.Change<IdlePlayerState>();
                }
            }
        }

        public override void OnContact(Player player, Collider other)
        {
            player.PushRigidbody(other);
        }
    }
}