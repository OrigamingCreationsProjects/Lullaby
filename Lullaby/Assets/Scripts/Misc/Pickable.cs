using System;
using Lullaby.Entities;
using Lullaby.Entities.Enemies;
using UnityEngine;
using UnityEngine.Events;

namespace Lullaby
{
    [RequireComponent(typeof(Collider))]
    [RequireComponent(typeof(Rigidbody))]
    [AddComponentMenu("Lullaby/Misc/Pickable")]
    public class Pickable : MonoBehaviour, IEntityContact
    {
        //Settings relativos a la posición del objeto al cogerlo
        [Header("General Settings")]
        public Vector3 offset;
        public float releaseOffset = 0.5f;
        //Settings relativos al respawn del objeto cuando se destruye o desaparece.
        [Header("Respawn Settings")]
        public bool autoRespawn;
        public bool respawnOnHitHazards;
        public float respawnHeightLimit = -100;
        //Settings relativos al daño que puede producir el objeto a los enemigos
        [Header("Attack Settings")]
        public bool attackEnemies = true;
        public int damage = 3;
        public float minDamageSpeed = 5f;

        [Space(15)] 
        
        /// <summary>
        /// Called when this object is Picked.
        /// </summary>
        public UnityEvent onPicked;

        /// <summary>
        /// Called when this object is Released.
        /// </summary>
        public UnityEvent onReleased; 
        
        /// <summary>
        /// Called when this object is Respawned.
        /// </summary>
        public UnityEvent onRespawn;

        protected Collider _collider;
        protected Rigidbody _rigidBody;
        
        protected Vector3 _initialPosition;
        protected Quaternion _initialRotation;
        protected Transform _initialParent;
        
        protected RigidbodyInterpolation _interpolation; // Para evitar el jitter y efectos raros entre frames.
        
        public bool beingHold { get; protected set; }

        public virtual void PickUp(Transform slot)
        {
            if (!beingHold)
            {
                beingHold = true;
                transform.parent = slot;
                transform.localPosition = Vector3.zero + offset;
                transform.localRotation = Quaternion.identity;
                _rigidBody.isKinematic = true;
                _collider.isTrigger = true;
                _interpolation = _rigidBody.interpolation; // Guardamos la interpolación inicial.
                _rigidBody.interpolation = RigidbodyInterpolation.None; 
                onPicked?.Invoke();
            }
        }

        public virtual void Release(Vector3 direction, float force)
        {
            if (beingHold)
            {
                transform.parent = _initialParent;
                transform.position += direction * releaseOffset; // Para que no se quede dentro del jugador lo lanzamos.
                _collider.isTrigger = _rigidBody.isKinematic = beingHold = false;
                _rigidBody.interpolation = _interpolation; // Recuperamos la interpolación inicial.
                _rigidBody.velocity = direction * force;
                onReleased?.Invoke();
            }
        }

        public virtual void Respawn()
        {
            _rigidBody.velocity = Vector3.zero;
            transform.parent = _initialParent;
            transform.SetPositionAndRotation(_initialPosition, _initialRotation);
            _rigidBody.isKinematic = _collider.isTrigger = beingHold = false;
            onRespawn?.Invoke();
        }
        
        public void OnEntityContact(Entity entity)
        {
            if(attackEnemies && entity is Enemy && _rigidBody.velocity.magnitude > minDamageSpeed)
                entity.ApplyDamage(damage, transform.position);
            
            
        }

        protected virtual void EvaluateHazardRespawn(Collider other)
        {
            if (autoRespawn && respawnOnHitHazards && other.CompareTag(GameTags.Hazard))
            {
                Respawn();
            }
        }

        protected virtual void Start()
        {
            _collider = GetComponent<Collider>();
            _rigidBody = GetComponent<Rigidbody>();
            _initialPosition = transform.localPosition;
            _initialRotation = transform.localRotation;
            _initialParent = transform.parent;
        }

        protected virtual void Update()
        {
            if(autoRespawn && transform.position.y <= respawnHeightLimit)
                Respawn();
        }

        protected void OnTriggerEnter(Collider other) => EvaluateHazardRespawn(other);

        protected void OnCollisionEnter(Collision collision) => EvaluateHazardRespawn(collision.collider);
    }
}