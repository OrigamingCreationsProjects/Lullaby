using UnityEngine;

namespace Lullaby.Entities.States
{
    [AddComponentMenu("Lullaby/CustomMovement/Player/States/Dash Player State")]
    public class DashPlayerState : PlayerState
    {
        protected override void OnEnter(Player player)
        {
            player.verticalVelocity = Vector3.zero;
            player.lateralVelocity = player.localForward * player.stats.current.dashForce; // Aplicamos la fuerza hacia adelante del jugador
            player.playerEvents.OnDashStarted?.Invoke();
        }

        protected override void OnExit(Player player)
        {
            player.lateralVelocity = Vector3.ClampMagnitude(
                player.lateralVelocity, player.stats.current.topSpeed); // Limitamos la velocidad del jugador
            player.playerEvents.OnDashEnded?.Invoke(); 
        }

        public override void OnStep(Player player)
        {
            player.Jump();
            if (timeSinceEntered > player.stats.current.dashDuration)
            {
                player.states.Change<FallPlayerState>();
            }
        }

        public override void OnContact(Player player, Collider other)
        {
            player.PushRigidbody(other);
            player.WallDrag(other);
        }
    }
}