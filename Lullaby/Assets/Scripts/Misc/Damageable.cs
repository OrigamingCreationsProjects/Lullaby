using UnityEngine;
using UnityEngine.Serialization;

namespace Lullaby
{
    public class Damageable : MonoBehaviour
    {
        public int maxHitPoints;
        [Tooltip("Time that this gameObject is invulnerable for, after receiving damage.")]
        public float invulnerabiltyTime;
        
        [Tooltip("The angle from the which that damageable is hittable. Always in the world XZ plane, with the forward being rotate by hitForwardRoation")]
        [Range(0.0f, 360.0f)]
        public float hitAngle = 360.0f;
        [Tooltip("Allow to rotate the world forward vector of the damageable used to define the hitAngle zone")]
        [Range(0.0f, 360.0f)]
        [FormerlySerializedAs("hitForwardRoation")] //SHAME!
        public float hitForwardRotation = 360.0f;
        
        public bool isInvulnerable { get; set; }
        public int currentHitPoints { get; private set; }
        
        protected float _timeSinceLastHit = 0.0f;
        protected Collider _collider;

        System.Action schedule;
        
        private void Start()
        {
            ResetDamage();
            _collider = GetComponent<Collider>();
        }

        private void Update()
        {
            if (isInvulnerable)
            {
                _timeSinceLastHit += Time.deltaTime;
                if (_timeSinceLastHit > invulnerabiltyTime)
                {
                    _timeSinceLastHit = 0.0f;
                    isInvulnerable = false;
                }
            }
        }

        public void ResetDamage()
        {
            currentHitPoints = maxHitPoints;
            isInvulnerable = false;
            _timeSinceLastHit = 0.0f;
        }

        public void SetColliderState(bool enabled)
        {
            _collider.enabled = enabled;
        }

        public void ApplyDamage()
        {
            if (currentHitPoints <= 0)
            {
                return;
            }

            if (isInvulnerable)
            {
                return;
            }
            
            Vector3 forward = transform.forward;
            forward = Quaternion.AngleAxis(hitForwardRotation, transform.up) * forward;

            Vector3 positionToDamager = -transform.position;
            positionToDamager -= transform.up * Vector3.Dot(transform.up, positionToDamager);
            
            if(Vector3.Angle(forward, positionToDamager) > hitAngle * 0.5f)
            {
                return;
            }
            
            isInvulnerable = true;
            //currentHitPoints -= data.amount;

            if (currentHitPoints <= 0)
            {
                //schedule += OnDeath.Invoke;
            }
            else
            {
                
            }
            
            //var messageType = currentHitPoints <= 0 ? MessageType.Dead
            
            // for(var i = 0; i < onDamageMessageReceivers.Count; ++i)
            // {
            //     onDamageMessageReceivers[i].OnReceiveMessage(messageType, this, data);
            // }
        }
        
    }
}