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

            switch (boss.stage)
            {
                case BossStages.FirstStage:
                    boss.states.Change<CirculatingState>();
                    break;
                case BossStages.SecondStage:
                    boss.states.Change<BESecondCirculating>();
                    break;
                case BossStages.FinalStage:
                    boss.states.Change<BEFinalCirculating>();
                    break;
            }
        }

        public override void OnContact(BossEnemy entity, Collider other)
        {
        }
    }
}

