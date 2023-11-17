using UnityEngine;

namespace Lullaby.Entities.Enemies.States
{
    public class DieEnemyState : EnemyState
    {
        protected override void OnEnter(Enemy enemy)
        {
            enemy.ApplyDieForces();
        }

        protected override void OnExit(Enemy enemy) { }

        public override void OnStep(Enemy enemy)
        {
            enemy.ApplyGravity();
            if (timeSinceEntered >= enemy.stats.current.timeUntilDisappear || enemy.isGrounded)
            {
                enemy.Disappear();
            }
        }

        public override void OnContact(Enemy entity, Collider other) { }
    }
}