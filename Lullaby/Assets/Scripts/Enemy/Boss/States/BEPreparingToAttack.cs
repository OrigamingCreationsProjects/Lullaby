using System.Collections;
using System.Collections.Generic;
using Lullaby.Entities.Enemies;
using UnityEngine;

namespace Lullaby.Entities.Enemies.States
{
    public class BEPreparingToAttack : BEState
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
            switch (boss.stage)
            {
                case BossStages.FirstStage:
                    boss.states.Change<BEAttackingState>();
                    break;
                case BossStages.SecondStage:
                    boss.states.Change<BEDistAttackState>();
                    break;
                case BossStages.FinalStage:
                    boss.states.Change<BEDistAttackState>();
                    break;
            }
        }

        public override void OnContact(BossEnemy entity, Collider other)
        {
         
        }
    }
}

