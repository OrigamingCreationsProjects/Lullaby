using UnityEngine;

namespace Lullaby.Entities.Enemies
{
    [CreateAssetMenu(fileName = "NewDollyStats", menuName = "Character Stats/Enemy/New Dolly Stats")]
    public class DollyStats : EntityStats<DollyStats>
    {
        [Header("General Stats")]
        public float gravity = 35f;
        public float snapForce = 15f;
        public float rotationSpeed = 970f;
        public float deceleration = 28f;
        public float friction = 16f;
        public float turningDrag = 28f;

        [Header("Lateral Movement Stats")]
        public float timeBeforeMoveAgain = 1f;
        public float backAfterAttackTime = 0.3f;
        public float backAfterAttackDelay = 0.1f;
        public float forwardSpeed = 5f;
        [Range(1, 10)]
        public int chanceToMoveLaterally = 7;
        [Range(0.001f, 1f)]
        public float lateralSpeedMultiplier = 0.5f;
        
        
        [Header("Retreat Movement Stats")]
        public float retreatPreparationTime = 1.4f;
        public float minDistanceToStopRetreating = 7f;
        public float retreatSpeed = 2f;

        [Header("Attack Stats")] 
        public int attackDamage = 15;
        public float attackMovementDuration = 0.5f;
        public float attackPreparationTime = 0.2f;
        public float minDistanceToAttack = 2f;
        
        [Header("Receive Damage Stats")]
        public float damageMovementDuration = 0.3f;
        public float damageMovementDelay = 0.1f;
    }
}