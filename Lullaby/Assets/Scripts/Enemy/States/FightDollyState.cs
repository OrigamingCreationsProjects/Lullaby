using UnityEngine;

namespace Lullaby.Entities.Enemies.States
{
    public class FightDollyState : EnemyState
    {
        protected override void OnEnter(Enemy entity)
        {
            
        }

        protected override void OnExit(Enemy entity)
        {
        }

        public override void OnStep(Enemy entity)
        {
            entity.ApplyGravity();
            entity.SnapToGround();
            //((Dolly)entity)
            //entity.transform.LookAt(new Vector3(entity.player.transform.position.x, entity.player.transform.position.y, entity.player.transform.position.z));

            var destination = entity.player.transform.position;
            var head = destination - entity.position;
            var upOffset = Vector3.Dot(entity.transform.up, head); // Sacamos la direccion a la que mirar manteniendo nuestro eje Y
           
            head -= entity.transform.up * upOffset;
            
            var distance = head.magnitude;
            var direction = head / distance; // Normalizamos porque solo nos interesa la direccion
            var localDirection = Quaternion.FromToRotation(entity.transform.up, Vector3.up) * direction;

            ((Dolly)entity).FaceDirectionSmooth(localDirection);
            //Debug.Log($"La direccion actual es: {((Dolly)entity).MoveDirection}");
            ((Dolly)entity).MoveDolly(((Dolly)entity).MoveDirection);
        }

        public override void OnContact(Enemy entity, Collider other)
        {
        }
    }
}