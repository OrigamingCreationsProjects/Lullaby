using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Lullaby.Entities.Enemies;
using UnityEngine.ProBuilder.Shapes;

namespace Lullaby.Entities.Enemies.States
{
    public class BEAttackingState : BEState
    {
        private bool retreat;
        private Vector3 dir;
        private float timePersecuting;
        private Vector3 lastPos; 
        private float playerPosUpdateDelay;
        protected override void OnEnter(BossEnemy entity)
        {
            //entity.enemyEvents.HandleAttack(true);
            retreat = false;
            //entity.transform.parent = null;
            
            dir = (entity.player.position - entity.transform.position);
            lastPos = entity.position;
            lastPos.y = 0f;
            timePersecuting = entity.stats.current.FsPursuitTime;
            playerPosUpdateDelay =  entity.stats.current.FSPlayerPosUpdateDelay;

            if (entity.stage == BossStages.FirstStage) return;
       
            Sequence s = DOTween.Sequence();
            s.AppendCallback(() => entity.ShootBullet(entity.index));
            s.AppendCallback(() => entity.enemyEvents.OnAttackPerformed?.Invoke());
        }

        protected override void OnExit(BossEnemy entity)
        {
           
            entity.lateralVelocity = Vector3.zero;
            if(entity.stage != BossStages.FirstStage)entity.enemyEvents.HandleAttack(false);
            if (!retreat) return;
            Sequence s = DOTween.Sequence();
            // s.AppendCallback(() => entity.transform.parent = BossEnemy.MainBoss.transform);
            s.AppendCallback(() =>
            {
                if(entity.stage == BossStages.FirstStage)
                {
                    //if(entity.stats.current.goBackIntoSlot)
                    //{
                    entity.transform.position = lastPos;
                       // entity.transform.DOMove(lastPos,entity.stats.current.returnToPosTime);
                        //if(entity.stats.current.waitForRetreat)
                        entity.enemyEvents.HandleRetreat(true);
                    //}

                }

                });
            s.AppendCallback(() => entity.SetController(false));
            s.AppendInterval(entity.stats.current.returnToPosTime);
            s.AppendCallback(() => entity.SetController(true));
            s.AppendCallback(() => entity.enemyEvents.HandleRetreat(false));
            s.AppendCallback(() => entity.enemyEvents.HandleAttack(false));
            //entity.transform.position = entity.slot.position;
            //entity.transform.parent = BossEnemy.MainBoss.transform;   
        }

        public override void OnStep(BossEnemy entity)
        {
            if(!entity.step) return;
            if (entity.stage == BossStages.FirstStage)
            {
                if(playerPosUpdateDelay < 0f) {
                    dir = (entity.player.position - entity.transform.position);
                    playerPosUpdateDelay = entity.stats.current.FSPlayerPosUpdateDelay;
                }
                var dist = (entity.player.position - entity.transform.position).magnitude;
                if (dist < entity.stats.current.DestinyReachedThreshold || timePersecuting <= 0f)
                {
                    if (timePersecuting <= 0f) retreat = true;
                    entity.Decelerate();
                    entity.enemyEvents.OnAttackPerformed?.Invoke();
                    return;
                }
                entity.Accelerate(dir.normalized, entity.stats.current.attackLaunchAcceleration,entity.stats.current.attackLaunchSpeed);
            }
            else
            {
                entity.Decelerate();
                entity.enemyEvents.OnAttackPerformed?.Invoke();
            }
               
            HandleTimers();
           
        }

        public override void OnContact(BossEnemy entity, Collider other)
        {
        }

        private void HandleTimers()
        {
            if (timePersecuting > 0f)
            {
                timePersecuting -= Time.deltaTime; 
            }

            if (playerPosUpdateDelay > 0f)
            {
                playerPosUpdateDelay -= Time.deltaTime;
            }
        }
    }

}
