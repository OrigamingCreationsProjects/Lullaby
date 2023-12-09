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
            retreat = false;
            
            entity.EnableCollider(true);
            dir = (entity.player.position - entity.transform.position);
            lastPos = entity.transform.localPosition;
            lastPos.y = 0f;
            timePersecuting = entity.stats.current.FsPursuitTime;
            playerPosUpdateDelay =  entity.stats.current.FSPlayerPosUpdateDelay;
            
        }

        protected override void OnExit(BossEnemy entity)
        {
            entity.EnableCollider(true);
            entity.lateralVelocity = Vector3.zero;
            if (!retreat) return;
            Sequence s = DOTween.Sequence();
            s.AppendCallback(()=>   entity.enemyEvents.HandleRetreat(true));
            s.AppendCallback(() =>
            {
               
                    entity.transform.localPosition = lastPos;
                      
                        

                });
            s.AppendInterval(entity.stats.current.returnToPosTime);
            s.AppendCallback(() => entity.enemyEvents.HandleRetreat(false));
            s.AppendCallback(() => entity.enemyEvents.HandleAttack(false));
        }

        public override void OnStep(BossEnemy entity)
        {
            if(!entity.step) return;
            
                if(playerPosUpdateDelay < 0f) {
                    dir = (entity.player.position - entity.transform.position);
                    playerPosUpdateDelay =  entity.stats.current.FSPlayerPosUpdateDelay;
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
                
               
            HandleTimers();
           
        }

        public override void OnContact(BossEnemy entity, Collider other)
        {
            entity.ContactAttack(other, entity.controller.bounds);
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
