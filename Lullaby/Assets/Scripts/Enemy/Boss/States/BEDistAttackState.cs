using System.Collections;
using System.Collections.Generic;
using Lullaby.Entities.Enemies;
using UnityEngine;

namespace Lullaby.Entities.Enemies.States
{
    public class BEDistAttackState : BEState
    {
        protected override void OnEnter(BossEnemy entity)
        {
            if(!entity.step) return;
            if (entity.bossManager.enemyRetreating) return;
            entity.ShootBullet();
            entity.enemyEvents.OnAttackPerformed?.Invoke();
        }

        protected override void OnExit(BossEnemy entity)
        {
            
        }

        public override void OnStep(BossEnemy entity)
        {
           
        }

        public override void OnContact(BossEnemy entity, Collider other)
        {
        }
    }
}

