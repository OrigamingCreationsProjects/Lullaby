using UnityEngine;

namespace Lullaby.Entities.NPC.States
{
    public class SpeakTalkerState : TalkerState
    {
        protected override void OnEnter(Talker talker) { }

        protected override void OnExit(Talker talker) { }

        public override void OnStep(Talker talker)
        {
        }

        public override void OnContact(Talker talker, Collider other)
        { }
    }
}