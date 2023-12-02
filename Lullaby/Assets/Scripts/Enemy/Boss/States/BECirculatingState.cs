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
    protected override void OnEnter(BossEnemy boss)
    {
        timeRemaining = boss.stats.current.FSApproachTime;
    }

    protected override void OnExit(BossEnemy boss)
    {
    }

    public override void OnStep(BossEnemy boss)
    {   
        if(!boss.step) return;
        if(boss.bossManager.enemyAttacking)
        {
            Debug.Log($"{boss.name} IS DECELERATING. AN ENEMY IS ATTACKING");boss.Decelerate(); return;
            
        }

        if (boss.bossManager.enemyRetreating)
        {
            Debug.Log($"{boss.name} IS DECELERATING. AN ENEMY IS RETREATING");boss.Decelerate(); return;
        }
      
        Vector3 dir = Vector3.zero;
        var aux = ComputeDirections(boss);
        //if(boss.stats.current.detectOtherBosses)//dir += 
         //   DetectOtherBosses(boss);//*(1 - boss.stats.current.bossCollisionTolerance);
        dir = aux;
        //Debug.DrawRay(boss.transform.position, aux *2f, Color.blue);
        Debug.DrawRay(boss.transform.position, dir *2f, Color.green);
        var playerPos = boss.player.position;
        playerPos.y = boss.position.y;
        Debug.DrawRay(boss.transform.position,
            (playerPos - boss.transform.position).normalized * (playerPos - boss.transform.position).magnitude, Color.red);
        float distFromPlayer = (playerPos-boss.transform.position).magnitude;
        if(timeRemaining > 0) {
            speed = (distFromPlayer - boss.stats.current.FsMinDistToPlayer) /
                    timeRemaining;
            acceleration = speed/timeRemaining;
            timeRemaining -= Time.deltaTime;}
      
        if(distFromPlayer > boss.stats.current.FsMinDistToPlayer)
            boss.Accelerate(dir.normalized, acceleration, speed); 
        else
            boss.Decelerate();
        
      
    }

    public override void OnContact(BossEnemy boss, Collider other)
    {
    }

    private Vector3 ComputeDirections(BossEnemy boss)
    {
        if(!boss.player) boss.enemyEvents.OnPlayerEscaped?.Invoke();

        var playerPos = boss.player.position;
        playerPos.y = boss.position.y;
        Vector3 dir = playerPos - boss.transform.position;
        Vector3 normDir = dir.normalized;
        Vector3 pDir = Quaternion.AngleAxis(90, Vector3.up) * normDir; //Vector perpendicular to direction
        Vector3 finalDirection = new Vector3();
       
        
        
        //else finalDirection =  (normDir + pDir * moveDirection.normalized.x).normalized;
        switch(boss.stage)
            {
                case BossStages.FirstStage:
                    finalDirection = normDir;
                /*if (distFromPlayer < boss.stats.current.FsMinDistToPlayer) 
                    finalDirection = (-normDir + pDir * moveDirection.normalized.x).normalized;
                else if (distFromPlayer > boss.stats.current.FsMaxDistToPlayer) 
                    finalDirection =  (normDir + pDir * moveDirection.normalized.x).normalized;
                else
                    finalDirection = pDir * moveDirection.normalized.x;*/
                    break;
                default:
               /*if (distFromPlayer < boss.stats.current.SsMinDistToPlayer) 
                    finalDirection = -normDir; //+ pDir * moveDirection.normalized.x).normalized;
                else if (distFromPlayer > boss.stats.current.SsMaxDistToPlayer) 
                    finalDirection =  normDir; //+ pDir * moveDirection.normalized.x).normalized;
                else
                    finalDirection = pDir * moveDirection.normalized.x;*/
                
                    break;
            }

        return finalDirection;
    }

    private void DetectOtherBosses(BossEnemy boss)
    {
        Collider[] collisions = Physics.OverlapSphere(boss.position,boss.stats.current.bossDetectionRadius);
        List<Vector3> directionsToBosses = new List<Vector3>(); 
        foreach(Collider instance in collisions)
        {

            if(instance.GetComponent<BossEnemy>())
            {
                var dir = instance.transform.position - boss.position;
                directionsToBosses.Add(dir);
                Debug.DrawRay(boss.transform.position, dir.normalized *2f, Color.red);
            }
        }

       Vector3 directionMean = Vector3.zero;

        foreach(Vector3 direction in directionsToBosses)
        {
           
            directionMean += direction;
        }

        directionMean = directionMean/directionsToBosses.Count;
        directionMean = directionMean.normalized;
        Debug.DrawRay(boss.transform.position, -directionMean*1f, Color.yellow);
        if(directionsToBosses.Count > 0)AccelerateOrDecelerate(directionMean, boss);
        //return -directionMean;
    }

    private void AccelerateOrDecelerate(Vector3 bossesDirections, BossEnemy boss)
    {
        //Check whether on my right or left 
        var direction = AngleDir(boss.transform.forward, bossesDirections, boss.transform.up);
        Vector3 dir = boss.player.transform.position - boss.transform.position;
        Vector3 normDir = dir.normalized;
        Vector3 pDir = Quaternion.AngleAxis(90, Vector3.up) * normDir; 
        // If the direction of the bosses are in the direction of the movement decelerate
        if(direction == 1.0f){
            Debug.Log($"EL CLONE {boss.name} TIENE A OBSTÁCULOS EN LA DERECHA");
          //  if(boss.bossManager.moveDirection.normalized.x == 1.0f) boss.Decelerate(boss.stats.current.decelerationNerf);
            // else if(boss.bossManager.moveDirection.normalized.x == -1.0f) boss.Accelerate(pDir * boss.bossManager.moveDirection.normalized.x,boss.stats.current.accelerationBuff, boss.stats.current.topSpeedBuff);
        } else if(direction == -1.0f)
        {
             Debug.Log($"EL CLONE {boss.name} TIENE A OBSTÁCULOS EN LA IZQUIERDA");
            // if(boss.bossManager.moveDirection.normalized.x == 1.0f)boss.Accelerate(pDir * boss.bossManager.moveDirection.normalized.x,boss.stats.current.accelerationBuff, boss.stats.current.topSpeedBuff);
             //else if(boss.bossManager.moveDirection.normalized.x == -1.0f)  boss.Decelerate(boss.stats.current.decelerationNerf);
        }
        else{
            Debug.Log($"EL CLONE {boss.name} TIENE A OBSTÁCULOS DELANTE O DETRÁS");
            //boss.Accelerate(pDir * boss.bossManager.moveDirection.normalized.x,boss.stats.current.accelerationBuff, boss.stats.current.topSpeedBuff);
        }

       
        // Else accelerate
    }
    
    private float AngleDir(Vector3 fwd, Vector3 targetDir, Vector3 up)
    {
        Vector3 perp = Vector3.Cross(fwd, targetDir);
        float dir = Vector3.Dot(perp, up);
 
        if (dir > 0.0f) {
            return 1.0f;
        } else if (dir < 0.0f) {
            return -1.0f;
        } else {
            return 0.0f;
        }
    }  

    }

    
}