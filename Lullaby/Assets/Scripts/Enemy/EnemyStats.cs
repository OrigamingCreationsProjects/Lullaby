﻿using UnityEngine;
using UnityEngine.Serialization;

namespace Lullaby.Entities.Enemies
{
    [CreateAssetMenu(fileName = "NewEnemyStats", menuName = "Character Stats/Enemy/New Enemy Stats")]
    public class EnemyStats : EntityStats<EnemyStats>
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
        public int contactDamage = 1;
        public float contactPushBackForce = 18f;
        public float contactSteppingTolerance = 0.1f;

        [Header("Hurt Stats")] 
        public float timeInHurtState = 0.5f;
        public bool canReceivePushBack = true;
        public float hurtBackwardsForce = 15f;
        
        [Header("Die Stats")]
        public float timeUntilDisappear = 1.5f;
        public float dieBackwardsForce = 15f;
        public float dieUpwardsForce = 100f;
       
        [Header("View Stats")]
        public float spotRange = 5f;
        public float viewRange = 8f;

        [Header("Follow Stats")]
        public float followAcceleration = 10f;
        public float followTopSpeed = 4f;

        [Header("Waypoint Stats")]
        public bool faceWaypoint = true;
        public float waypointMinDistance = 0.5f; // Distace from the waypoint to stop moving
        public float waypointAcceleration = 10f;
        public float waypointTopSpeed = 2f;
    }
}