using Lullaby.Entities.NPC;
using UnityEngine;

namespace Lullaby.Entities.NPC.States
{
    public class IdleTalkerState : TalkerState
    {
        protected override void OnEnter(Talker talker) { }

        protected override void OnExit(Talker talker) { }

        public override void OnStep(Talker talker)
        {
            talker.HandleSight();
        }

        public override void OnContact(Talker talker, Collider other)
        { }
    }
}