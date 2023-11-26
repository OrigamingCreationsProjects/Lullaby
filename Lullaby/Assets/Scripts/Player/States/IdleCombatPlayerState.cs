using UnityEngine;

namespace Lullaby.Entities.States
{
    public class IdleCombatPlayerState : PlayerState
    {
        protected override void OnEnter(Player player) { }

        protected override void OnExit(Player player) { }

        public override void OnStep(Player player) { }

        public override void OnContact(Player player, Collider other) { }
    }
}