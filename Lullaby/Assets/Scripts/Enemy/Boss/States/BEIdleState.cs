using UnityEngine;
using DG.Tweening;

namespace Lullaby.Entities.Enemies.States
{
    public class BEIdleState : BEState
    {
        protected override void OnEnter(BossEnemy boss)
        {
          
        }

        protected override void OnExit(BossEnemy boss)
        {
            
        }

        public override void OnStep(BossEnemy boss)
        {   
            if(boss.velocity != Vector3.zero) boss.Decelerate();
            if(!boss.step) return;
            var distanceToPlayer = (boss.player.position - boss.position).magnitude;
            if(BossEnemy.MainBoss != boss) boss.states.Change<CirculatingState>();
            if(BossEnemy.MainBoss == boss && boss.invoked) boss.states.Change<CirculatingState>();
        }

        public override void OnContact(BossEnemy entity, Collider other)
        {
            throw new System.NotImplementedException();
        }
    }
}

