using UnityEngine;

namespace Lullaby.Entities.NPC.States
{
    public class DialogueTalkerState : TalkerState
    {
        protected override void OnEnter(Talker talker)
        {
           
        }

        protected override void OnExit(Talker talker)
        {
            talker.transform.localRotation = talker.originalRotation;
            // talker.FaceDirectionSmooth(talker.originalRotation, 400);
        }

        public override void OnStep(Talker talker)
        {
            var head = talker.player.position - talker.position; // Direction to player
            var upOffset = Vector3.Dot(talker.transform.up, head); // Sacamos la direccion a la que mirar manteniendo nuestro eje Y
            var direction = head - talker.transform.up * upOffset; // Direction to player without up offset
            var localDirection = Quaternion.FromToRotation(talker.transform.up, Vector3.up) * direction; // Direction to player without up offset in local space
            
            localDirection = localDirection.normalized; // Normalizamos porque solo nos interesa la direccion
            // porque si no al usar el dato sera mas grande cuanto mas lejos este
            talker.FaceDirectionSmooth(localDirection, 400);
        }

        public override void OnContact(Talker talker, Collider other)
        { }
    }
}