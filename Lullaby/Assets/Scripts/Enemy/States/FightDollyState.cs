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
            throw new System.NotImplementedException();
        }

        public override void OnStep(Enemy entity)
        {
            entity.ApplyGravity();
            entity.SnapToGround();
            //((Dolly)entity)
            entity.transform.LookAt(new Vector3(entity.player.transform.position.x, entity.player.transform.position.y, entity.player.transform.position.z));
            ((Dolly)entity).MoveDolly(((Dolly)entity).MoveDirection);
        }

        public override void OnContact(Enemy entity, Collider other)
        {
            throw new System.NotImplementedException();
        }
    }
}