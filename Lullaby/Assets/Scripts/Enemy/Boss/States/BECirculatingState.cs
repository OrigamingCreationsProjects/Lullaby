using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Lullaby.Entities.Enemies.States
{
    public class CirculatingState : BEState
    {
        private float acceleration;
        private float speed;
        private float timeRemaining;

        private Vector3 playerPos;
    protected override void OnEnter(BossEnemy boss)
    {
        boss.EnableCollider(true);
        timeRemaining = boss.stats.current.FSApproachTime;
    }

    protected override void OnExit(BossEnemy boss)
    {
        boss.EnableCollider(false);
    }

    public override void OnStep(BossEnemy boss)
    {   
        if(!boss.step) return;
        if(boss.bossManager.enemyAttacking || boss.bossManager.enemyRetreating) { boss.Decelerate(); return; }
      
        Vector3 dir = Vector3.zero;
        dir = ComputeDirections(boss);
       
        float distFromPlayer = (playerPos-boss.transform.position).magnitude;
      
        HandleTimer(distFromPlayer, boss);

        if (distFromPlayer > boss.stats.current.FsMinDistToPlayer)
        {
            boss.Accelerate(dir.normalized, acceleration, speed);
            boss.InPlace = false;
        }
                  
        else
        {
            boss.Decelerate();
            boss.InPlace = true;
        }
           
    }

    public override void OnContact(BossEnemy boss, Collider other)
    {
       boss.ContactAttack(other, boss.controller.bounds);
    }

    private Vector3 ComputeDirections(BossEnemy boss)
    {
        Vector3 dir;
        Vector3 normDir;
        Vector3 finalDirection;
        
        playerPos = boss.player.position;
        playerPos.y = boss.position.y;
        
        dir = playerPos - boss.transform.position;
        normDir = dir.normalized; 
        finalDirection = normDir;
                
              
            

        return finalDirection;
    }

    private void HandleTimer(float distFromPlayer, BossEnemy boss)
    {
        if(timeRemaining > 0) {
            speed = Mathf.Abs((distFromPlayer - boss.stats.current.FsMinDistToPlayer)) /
                    timeRemaining;
            acceleration = speed/timeRemaining;
            timeRemaining -= Time.deltaTime;}
    }

    }

    
}