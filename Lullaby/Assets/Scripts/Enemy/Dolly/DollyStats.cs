using UnityEngine;

namespace Lullaby.Entities.Enemies
{
    [CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Character Stats/Enemy/New Enemy Stats")]
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
        
        [Header("Retreat Movement Stats")]
        public float retreatPreparationTime = 1.4f;
        public float minDistanceToRetreat = 4f;
        public float retreatSpeed = 2f;
        
        [Header("Attack Stats")]
        public float attackMovementDuration = 0.5f;
    }
}