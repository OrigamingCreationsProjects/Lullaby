using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lullaby.Entities.Enemies
{
    public class BulletBehaviour : MonoBehaviour
    {
        public BossEnemy _parentBoss { get; private set;}
        public CapsuleCollider collider {get; private set;}
       
        public ProjectileStats stats;
        public Vector3 dir { get; private set; }
        public bool shot {get; private set;}
        
        public float speed;
        public ParticleSystem[] particles;
        private bool followBoss = false;
        private float currentTime;
        private float time;

        #region SET

        public void SetParentBoss(BossEnemy parent)
        {
            if (_parentBoss == null) _parentBoss = parent;
        }
        
        protected void SetShotStatus(bool value) => shot = value;

        #endregion
        protected virtual void InitializeCollider() => collider = GetComponent<CapsuleCollider>();

        protected virtual void HandleTimeAlive()
        {
            if (currentTime <= 0f)
            { 
                currentTime = stats.timeAlive; 
                ChangeActiveState(false);
                //gameObject.SetActive(false); 
                //SetShotStatus(false); 
            }
            else currentTime -= Time.deltaTime;
        }

        public virtual void SetMovingDirection(Vector3 direction = default)
        {
            if (direction != default) {
                dir = direction;
            }
            else if(time < 0f)
            {
                var dirPlayer = (_parentBoss.player.transform.position - transform.position).normalized;
                dir += dirPlayer * stats.curveSpeed;
                time = stats.playerPosUpdateDelay;
            }
            else
            {
                time -= Time.deltaTime;
            }

            dir = dir.normalized;
            transform.position += dir.normalized * (Time.deltaTime * speed);
           
        }
     
        protected virtual void ContactAttack(Collider other) => _parentBoss.ContactAttack(other, collider.bounds, this);

        protected void CenterAtParent()
        {
            var offset = (_parentBoss.player.position - _parentBoss.position).normalized * stats.distanceFromBoss;
            transform.position = _parentBoss.position + offset;
        }
        
        public void Rebound()
        {
            followBoss = true;
            speed = stats.reboundSpeed;
            currentTime = stats.timeAlive;
        }
        
        public void SetParticleColor(Color color)
        {
            foreach (var particle in particles)
            {
                var main = particle.main;
                main.startColor = color;
            }
        }
        
        #region -- MONOBEHAVIOUR --

        private void Awake()
        { 
            InitializeCollider();
            currentTime = stats.timeAlive;
            SetShotStatus(false);
        }
        
        void Start()
        {
            speed = stats.projectileSpeed;
            time = -1f;
        }
        void Update()
        {
            if(followBoss) SetMovingDirection((_parentBoss.position - transform.position).normalized);
            else SetMovingDirection();
            HandleTimeAlive();
           
        }
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out BossEnemy enemy))
            {
                if(enemy == _parentBoss && followBoss) enemy.ApplyDamage(stats.damage, transform.position);
                ChangeActiveState(false);
            }
         
            ContactAttack(other);
        }

        public void ChangeActiveState(bool value)
        {
            gameObject.SetActive(value);
            SetShotStatus(value);
        }
        
        private void OnEnable()
        {
            CenterAtParent();
            followBoss = false;
            dir = Vector3.zero;
            //SetShotStatus(true);
        }

        private void OnDisable()
        {
            //SetShotStatus(false);
            currentTime = stats.timeAlive;
        }

        private void OnCollisionEnter(Collision other)
        {
           ChangeActiveState(false);
        }

    
        #endregion
        
    }

}
