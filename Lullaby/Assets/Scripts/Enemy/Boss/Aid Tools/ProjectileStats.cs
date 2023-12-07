using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Stats/Enemy/Boss/Projectile Stats", fileName = "New Projectile Stats")]
public class ProjectileStats : ScriptableObject
{
    [Header("Position Parameters")] 
    [Tooltip("Distance from the boss for the projectile to spawn. Should be greater than boss collider to avoid conflicts.")]
    public float distanceFromBoss = 5f;
    [Header("Speed Parameters")]
    public float projectileSpeed = 10f; 
    public float reboundSpeed = 20f;
    public float curveSpeed = 5f;
    [Header("Time Parameters")]
    public float timeAlive = 6f;
    public float playerPosUpdateDelay = .4f;
  
}

