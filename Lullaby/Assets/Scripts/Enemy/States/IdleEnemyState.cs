using UnityEngine;

namespace Lullaby.Entities.Enemies.States
{
    public class IdleEnemyState: EnemyState
    {
        protected override void OnEnter(Enemy enemy){}

        protected override void OnExit(Enemy enemy) { }

        public override void OnStep(Enemy enemy)
        {
            enemy.ApplyGravity();
            enemy.SnapToGround();
            enemy.Friction();
        }

        public override void OnContact(Enemy enemy, Collider other) { }
    }
}