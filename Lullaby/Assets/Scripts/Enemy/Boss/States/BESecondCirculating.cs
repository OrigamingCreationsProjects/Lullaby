
using System.Collections.Generic;
using UnityEngine;

namespace  Lullaby.Entities.Enemies.States
{
    public class BESecondCirculating : BEState
    {
        protected override void OnEnter(BossEnemy boss)
        {
            boss.EnableCollider(true);
        }

        protected override void OnExit(BossEnemy boss)
        {
            boss.EnableCollider(false);
        }

        public override void OnStep(BossEnemy boss)
        {
            if (!boss.step) return;
            if (boss.bossManager.enemyRetreating) {boss.Decelerate(); return;}
            Vector3 dir;
            Vector3 normDir;
            Vector3 pDir; 
            Vector3 finalDirection;
        
            dir = boss.bossManager.fightPlatform.position - boss.transform.position;
            normDir = dir.normalized;
            pDir = Quaternion.AngleAxis(90, Vector3.up) * normDir; 
            finalDirection = pDir;
            
            boss.Accelerate(finalDirection.normalized, boss.stats.current.secondStageAcceleration, boss.stats.current.secondStageSpeed);
          
        }

        public override void OnContact(BossEnemy entity, Collider other)
        {
            entity.ContactAttack(other, entity.controller.bounds);
        }
    }
}

