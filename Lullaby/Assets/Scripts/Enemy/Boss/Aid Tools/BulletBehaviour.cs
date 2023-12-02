using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lullaby.Entities.Enemies
{
    public class BulletBehaviour : MonoBehaviour
    {
        public BossEnemy parentBoss;
        public CapsuleCollider collider {get; private set;}
        public bool followBoss = false;
        public ProjectileStats stats;
        public Vector3 dir { get; private set; }
        public bool shot {get; private set;}
        private float currentTime;
        public float speed;
        protected virtual void InitializeCollider() => collider = GetComponent<CapsuleCollider>();
        protected virtual void MoveProjectile(float speed) => transform.position += dir * (Time.deltaTime * speed);

        protected virtual void HandleTimeAlive()
        {
            
            if(currentTime <= 0f){ UpdateShotStatus(false); 
                currentTime = stats.timeAlive; gameObject.SetActive(false); }
            else currentTime -= Time.deltaTime;
        }

        public virtual void SetMovingDirection(Vector3 direction = default)
        {
            if (direction != default) {
                dir = direction;
            }
            else
                dir = (parentBoss.player.transform.position - parentBoss.transform.position).normalized;
            UpdateShotStatus(true);
        }
        protected void UpdateShotStatus(bool value) => shot = value;
        protected virtual void ContactAttack(Collider other){parentBoss.ContactAttack(other, collider.bounds, this);}

        protected void CenterAtParent()
        {
            var offset = (parentBoss.player.position - parentBoss.position).normalized * 5f;
            transform.position = parentBoss.position + offset;
        }
        
        protected void CheckPlayerCollision(Collider other) {
            if(!other.CompareTag(GameTags.Player)) return; 
            if(!other.TryGetComponent(out Player player)) return; 
            UpdateShotStatus(false);
            gameObject.SetActive(false);
        }
        
        #region -- MONOBEHAVIOUR --

        private void Awake()
        { 
            InitializeCollider();
            currentTime = stats.timeAlive;
            UpdateShotStatus(false);
        }
        
        void Start()
        {
            speed = stats.projectileSpeed;
           //SetMovingDirection();
        }
        void Update()
        {
            MoveProjectile(speed);
            HandleTimeAlive();
            if(followBoss) SetMovingDirection((parentBoss.position - transform.position).normalized);
        }
        private void OnTriggerEnter(Collider other)
        {
            var dir = (parentBoss.position - parentBoss.player.position).normalized;
            if (other.TryGetComponent<BossEnemy>(out BossEnemy enemy))
            {
                if(enemy == parentBoss) enemy.ApplyDamage(101);
                UpdateShotStatus(false);
                gameObject.SetActive(false);
                followBoss = false;
            }
            if(!other.CompareTag(GameTags.Player)) return; 
         
            ContactAttack(other);
            //CheckPlayerCollision(other);
        }

        private void OnEnable()
        {
            CenterAtParent();
            SetMovingDirection();
        }

        private void OnCollisionEnter(Collision other)
        {
           UpdateShotStatus(false);
           gameObject.SetActive(false);
           
        }

        #endregion
        
    }

}
