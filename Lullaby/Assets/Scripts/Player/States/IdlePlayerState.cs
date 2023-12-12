using Lullaby.Entities;
using UnityEngine;

namespace Lullaby.Entities.States
{
    [AddComponentMenu("Lullaby/CustomMovement/Player/States/Idle Player State")]
    public class IdlePlayerState: PlayerState
    {
        protected override void OnEnter(Player player)
        {
            PlayerFace pf = player.GetComponentInChildren<PlayerFace>();
            pf.SetCurrentBlink(pf.blinkNeutralHash);
        }

        protected override void OnExit(Player player)
        {
            player.playerEvents.OnRandomIdleExit?.Invoke();
        }

        public override void OnStep(Player player)
        {
            player.ApplyGravity();
            player.SnapToGround();
            player.Jump();
            player.Fall();
            player.CheckAttackTarget();
            player.Attack();
            player.PickAndThrow();
            player.Talk();
            player.HandleMoonLauncher();
            player.RegularSlopeFactor();
            player.ApplyFriction();
            
            var inputDirection = player.inputs.GetMovementDirection();

            if (inputDirection.sqrMagnitude > 0 || player.lateralVelocity.sqrMagnitude > 0)
            {
                player.states.Change<WalkPlayerState>();
            }
        }

        public override void OnContact(Player player, Collider other){ }
    }
}