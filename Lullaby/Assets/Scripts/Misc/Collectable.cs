using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lullaby.Entities;
using UnityEngine;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Lullaby
{
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("Lullaby/Misc/Collectable")]
    public class Collectable : MonoBehaviour
    {
        [Header("General Settings")]
        public bool collectOnContact = true;
        public bool resetTransform;
        public bool isLifeOrb = true;
        public int lifeQuantity = 10;
        public int times = 1;
        public float ghostingDuration = 0.5f;
        public GameObject display;
        public AudioClip clip;
        public ParticleSystem particle;

        [Header("Visibility Settings")] 
        public bool hidden;
        public float quickShowHeight = 2f;
        public float quickShowDuration = 0.25f;
        public float hideDuration = 0.5f;

        [Header("Life Time")] 
        public bool hasLifeTime;
        public float lifeTimeDuration = 5f;

        [Header("Physics Settings")] 
        public bool usePhysics;
        public float minForceToStopPhysics = 3f;
        public float collisionRadius = 0.5f;
        public float gravity = 15f;
        public float bounciness = 0.98f;
        public float maxBounceYVelocity = 10f;
        public bool randomizeInitialDirection = true;
        public Vector3 initialVelocity = new Vector3(0, 12, 0);
        public AudioClip collisionClip;
        
        [Space(15)]
        /// <summary>
        /// Called when it has been collected.
        /// </summary>
        public PlayerEvent onCollect;
        
        protected Collider _collider;
        protected AudioSource _audio;

        protected bool _vanished;
        protected bool _ghosting = true;
        protected float _elapsedLifeTime;
        protected float _elapsedGhostingTime;
        protected Vector3 _velocity;
        
        protected static Transform _container;
        
        protected const string _containerName = "__COLLECTABLES_CONTAINER__";
        
        protected const int _verticalMinRotation = 0;
        protected const int _verticalMaxRotation = 30;
        protected const int _horizontalMinRotation = 0;
        protected const int _horizontalMaxRotation = 360;
        
        public static Transform container
        {
            get
            {
                if (!_container)
                {
                    _container = new GameObject(_containerName).transform;
                }

                return _container;
            }
        }

        protected virtual void InitializeAudio()
        {
            if (!TryGetComponent(out _audio))
            {
                _audio = gameObject.AddComponent<AudioSource>();
            }
        }
        
        protected virtual void InitializeCollider()
        {
            _collider = GetComponent<Collider>();
            _collider.isTrigger = true;
        }

        
        protected virtual void InitializeTransform()
        {
            if(!resetTransform) return;
            
            var initialRotation = usePhysics ? Quaternion.identity : transform.rotation;

            if (usePhysics) //&& transform.parent.TryGetComponent<GravityHa>)
            {
                var upDirection = transform.up;
                initialRotation =  Quaternion.FromToRotation(transform.up, upDirection);
                initialRotation *= transform.rotation;
            }
            transform.parent = container;
            transform.rotation = initialRotation;
        }

        protected virtual void InitializeDisplay()
        {
            display.SetActive(!hidden);
        }

        protected virtual void InitializeVelocity()
        {
            var direction = initialVelocity.normalized;
            var force = initialVelocity.magnitude;

            if (randomizeInitialDirection)
            {
                var randomZ = Random.Range(_verticalMinRotation, _verticalMaxRotation);
                var randomY = Random.Range(_horizontalMinRotation, _horizontalMaxRotation);
                direction = Quaternion.Euler(0, 0, randomZ) * direction;
                direction = Quaternion.Euler(0, randomY, 0) * direction;
            }
            
            _velocity = transform.rotation * direction * force;
        }

        /// <summary>
        /// The collection routine which is trigger the callbacks and activate reactions.
        /// </summary>
        /// <param name="player">The player which collected</param>
        /// <returns></returns>
        protected virtual IEnumerator CollectRoutine(Player player)
        {
            for (int i = 0; i < times; i++)
            {
                _audio.Stop();
                _audio.PlayOneShot(clip);
                onCollect.Invoke(player);
                yield return new WaitForSeconds(0.1f);
            }
        }

        protected virtual IEnumerator QuickShowRoutine()
        {
            var elapsedTime = 0f;
            var initialPosition = transform.position;
            var targetPosition = initialPosition + transform.up * quickShowHeight;
            
            display.SetActive(true);
            _collider.enabled = false;

            while (elapsedTime < quickShowDuration)
            {
                var t = elapsedTime / quickShowDuration;
                transform.position = Vector3.Lerp(initialPosition, targetPosition, t);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            transform.position = targetPosition;
            yield return new WaitForSeconds(hideDuration);
            transform.position = initialPosition;
            Vanish();
        }

        protected virtual void QuickShowTween()
        {
            var initialPosition = transform.position;
            var targetPosition = initialPosition + transform.up * quickShowHeight;
            
            display.SetActive(true);
            _collider.enabled = false;
            
            Sequence s = DOTween.Sequence();
            s.Append(transform.DOMove(targetPosition, quickShowDuration));
            s.AppendInterval(hideDuration);
            s.AppendCallback(() => transform.position = initialPosition);
            s.AppendCallback(() => Vanish());
        }
        
        public virtual void Vanish()
        {
            if (!_vanished)
            {
                _vanished = true;
                _elapsedLifeTime = 0;
                display.SetActive(false);
                _collider.enabled = false;
            }
        }

        public virtual void Collect(Player player)
        {
            if (!_vanished && !_ghosting)
            {
                if (!hidden)
                {
                    Vanish();

                    if (particle != null)
                    {
                        particle.Play();
                    }
                }
                else
                {
                    StartCoroutine(QuickShowRoutine());
                }
                
                StartCoroutine(CollectRoutine(player));
            }

            if (isLifeOrb)
            {
                player.health.Increase(lifeQuantity);
            }
        }
        
        
        protected virtual void HandleGhosting()
        {
            if (_ghosting)
            {
                _elapsedGhostingTime += Time.time;

                if (_elapsedGhostingTime >= ghostingDuration)
                {
                    _elapsedGhostingTime = 0;
                    _ghosting = false;
                }
            }
        }

        protected virtual void HandleLifeTime()
        {
            if (hasLifeTime)
            {
                _elapsedLifeTime += Time.deltaTime;

                if (_elapsedLifeTime >= lifeTimeDuration)
                {
                    Vanish();
                }
            }
        }

        protected virtual void HandleMovement()
        {
            _velocity -= transform.up * gravity * Time.deltaTime;
        }

        protected virtual void HandleSweep()
        {
            var direction = _velocity.normalized;
            var magnitude = _velocity.magnitude;
            var distance = magnitude * Time.deltaTime;

            if (Physics.SphereCast(transform.position, collisionRadius, direction, 
                    out var hit, distance, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Ignore))
            {
                if (!hit.collider.CompareTag(GameTags.Player))
                {
                    var bounceDirection = Vector3.Reflect(direction, hit.normal);
                    _velocity = bounceDirection * magnitude * bounciness;
                    var currentYBounce = Vector3.Dot(transform.up, _velocity);
                    _velocity -= transform.up * currentYBounce;
                    _velocity += transform.up * Mathf.Min(currentYBounce, maxBounceYVelocity);
                    _audio.Stop();
                    _audio.PlayOneShot(collisionClip);

                    if (currentYBounce <= minForceToStopPhysics)
                        usePhysics = false;
                }
            }
            
            transform.position += _velocity * Time.deltaTime;
        }

        protected void Awake()
        {
            InitializeAudio();
            InitializeCollider();
            InitializeTransform();
            InitializeDisplay();
            InitializeVelocity();
        }

        protected virtual void Update()
        {
            if(_vanished) return;
            
            HandleGhosting();
            HandleLifeTime();

            if (usePhysics)
            {
                HandleMovement();
                HandleSweep();
            }
        }

        protected void OnTriggerStay(Collider other)
        {
            if (collectOnContact && other.CompareTag(GameTags.Player))
            {
                if (other.TryGetComponent<Player>(out var player))
                {
                    Collect(player);
                }
            }
        }

        protected virtual void OnDrawGizmos()
        {
            if (usePhysics)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, collisionRadius);
            }
        }
    }
}