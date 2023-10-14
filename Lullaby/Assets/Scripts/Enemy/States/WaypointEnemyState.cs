using UnityEngine;

namespace Lullaby.Entities.Enemies.States
{
    public class WaypointEnemyState: EnemyState
    {
        protected override void OnEnter(Enemy enemy){}

        protected override void OnExit(Enemy enemy) { }

        public override void OnStep(Enemy enemy)
        {
            //IMPLEMENTAR LOGICA DE WAYPOINTS
        }

        public override void OnContact(Enemy enemy, Collider other) { }
    }
}