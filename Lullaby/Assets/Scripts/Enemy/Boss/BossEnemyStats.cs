using UnityEngine;

namespace Lullaby.Entities.Enemies
{
    [CreateAssetMenu(fileName = "NewBossEnemyStats", menuName = "Character Stats/Enemy/Boss/New Boss Enemy Stats")]
    public class BossEnemyStats : EntityStats<BossEnemyStats>
    {
        [Header("General Stats")]
        public float gravity = 35f;
        public float snapForce = 15f;
        public float rotationSpeed = 970f;
        public float deceleration = 28f;
      
        public float friction = 16f;
        public float turningDrag = 28f;

        [Header("Contact Attack Stats")]
        public bool canAttackOnContact = true;
        public bool contactPushback = true;
        public float contactOffset = 0.15f;
        public int contactDamage = 20;
        public float contactPushBackForce = 18f;
        public float contactSteppingTolerance = 0.1f;
        
        [Header("Attack Stats")]
        public float attackLaunchSpeed = 30f;
        public float attackLaunchAcceleration = 40f;
        public float attackMaxWaitingTime = 6f;
        public float attackMinWaitingTime = 4f;
        public float distAttackMaxWaitingTime = 2.5f; 
        public float distAttackMinWaitingTime = 1f;
     
        [Header("View Stats")]
        public float spotRange = 5f;
        //public float viewRange = 8f;

        [Header("Follow Stats")]
        public float followAcceleration = 10f;
        public float followTopSpeed = 4f;
        public float DestinyReachedThreshold = 2f;
        public float secondStageAcceleration = 5f;
        public float secondStageSpeed = 8f;

        [Header("Stages Parameters")] 

        [Header("First Stage")]
        public float FsMinDistToPlayer = 5f;
        public float FsMaxDistToPlayer = 10f;
        public float FSApproachTime = 3f;
        public float FsPursuitTime = 1.5f;
        public float FSPlayerPosUpdateDelay = .4f;
        public float returnToPosTime = 1.15f;
   
        [Header("Rest of Stages")]
        public float SsMaxDistToPlayer = 10f;
        public int secondStageThreshold = 200;
        public float angleOffset = 20f;
        public int finalStageThreshold = 100;

    }   
}
