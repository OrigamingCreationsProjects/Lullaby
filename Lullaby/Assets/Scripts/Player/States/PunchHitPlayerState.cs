using UnityEngine;

namespace Lullaby.Entities.States
{
    [AddComponentMenu("Lullaby/Entities/Player/States/Hurt Player State")]
    public class PunchHitPlayerState : PlayerState
    {
        protected override void OnEnter(Player player)
        {
        }

        protected override void OnExit(Player player) { player.skin.rotation = player.transform.rotation;}

        public override void OnStep(Player player)
        {
            player.ApplyGravity();
            if(timeSinceEntered >= 0.33f)
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