using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace  Lullaby.Entities.Enemies.States
{
    public class BEFinalCirculating : BEState
    {
        private Coroutine Movement_Coroutine;
        private Vector3 moveDirection;
        private Vector2 platformPos;
        protected override void OnEnter(BossEnemy boss)
        {
            boss.EnableCollider(true);
            int randomDir = Random.Range(0, 2);
            moveDirection = randomDir == 1 ? Vector3.right : Vector3.left;
            platformPos = new Vector2(boss.bossManager.fightPlatform.position.x,
                boss.bossManager.fightPlatform.position.z);
           // Movement_Coroutine = boss.StartCoroutine(boss.EnemyMovement());
        }

    protected override void OnExit(BossEnemy boss)
    {
        boss.EnableCollider(false);
    }

    public override void OnStep(BossEnemy boss)
    {   
        if(!boss.step) return;
        if (boss.bossManager.enemyRetreating) {boss.Decelerate(); return;}
        Vector3 dir = Vector3.zero;
        
        dir = ComputeDirections(boss);
        
        boss.Accelerate(dir.normalized, boss.stats.current.followAcceleration, boss.stats.current.followTopSpeed);
        
    }

    public override void OnContact(BossEnemy boss, Collider other)
    {
        boss.ContactAttack(other, boss.controller.bounds);
    } 
    private Vector3 ComputeDirections(BossEnemy boss)
    {
        Vector3 dir;
        Vector3 normDir;
        Vector3 pDir; 
        Vector3 finalDirection;
        
        CheckLimits(boss);
        dir = boss.bossManager.fightPlatform.position - boss.transform.position;
        normDir = dir.normalized;
        pDir = Quaternion.AngleAxis(90, Vector3.up) * normDir; 
        finalDirection = pDir * moveDirection.normalized.x;
        
        Debug.DrawRay(boss.position, finalDirection * 2f, Color.green);
        
        return finalDirection;
    }

    private void CheckLimits(BossEnemy boss)
    {
       
        var enemyPos = new Vector2(boss.position.x, boss.position.z);
        var dirToEnemy = (enemyPos - platformPos).normalized;
        if (!boss.InsideZone(dirToEnemy)) moveDirection *= -1f;
    }
    }
}

