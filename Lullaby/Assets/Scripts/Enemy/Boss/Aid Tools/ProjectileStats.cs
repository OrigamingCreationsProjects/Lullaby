using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Character Stats/Enemy/Boss/Projectile Stats", fileName = "New Projectile Stats")]
public class ProjectileStats : ScriptableObject
{
    public float timeAlive = 6f;
    public float projectileSpeed = 10f; 
    public float reboundSpeed = 20f;
}
