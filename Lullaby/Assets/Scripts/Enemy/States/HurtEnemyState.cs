using Lullaby.Entities.Events;
using UnityEngine;

namespace Lullaby.Entities.Enemies.States
{
    public class HurtEnemyState : EnemyState
    {
        protected override void OnEnter(Enemy enemy)
        {
            // Debug.Log($"Forward es: {enemy.transform.forward}");
            // Debug.Log($"Local forward es: {enemy.localForward}");
            // //enemy.lateralVelocity = -enemy.transform.forward * enemy.stats.current.hurtBackwardsForce;
            // // enemy.verticalVelocity = Vector3.zero;
            // // enemy.lateralVelocity = Vector3.zero;
            // enemy.verticalVelocity = Vector3.up * enemy.stats.current.hurtUpwardsForce;
            // //enemy.lateralVelocity = new Vector3(0, 0, -enemy.localForward.z * enemy.stats.current.hurtBackwardsForce);
            // Debug.Log($"Lateral velocity es: {enemy.lateralVelocity}");
            // enemy.lateralVelocity = -enemy.localForward * enemy.stats.current.hurtBackwardsForce;
            enemy.lateralVelocity = -enemy.localForward * enemy.stats.current.hurtBackwardsForce;
            
        }

        protected override void OnExit(Enemy enemy) { }

        public override void OnStep(Enemy enemy)
        {
            enemy.ApplyGravity();

            if (timeSinceEntered >= enemy.stats.current.timeInHurtState)
            {
                enemy.states.Change<WaypointEnemyState>();
            }
            
            // if (enemy.isGrounded && (enemy.verticalVelocity.y <= 0))
            // {
            //     if (enemy.health.current > 0)
            //     {
            //         enemy.states.Change<WaypointEnemyState>();
            //     }
            //     else
            //     {
            //         enemy.states.Change<DieEnemyState>();
            //     }
            // }
        }

        public override void OnContact(Enemy enemy, Collider other) { }
    }
}