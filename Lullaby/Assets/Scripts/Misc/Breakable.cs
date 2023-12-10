using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Serialization;

namespace Lullaby
{
    [RequireComponent(typeof(Collider), typeof(AudioSource))]
    [AddComponentMenu("Lullaby/Misc/Breakable")]
    public class Breakable : MonoBehaviour
    {
        public GameObject display;
        public AudioClip brokenClip;
        public AudioClip hitClip;
        public int currentHitsToBreak = 2;
        public float invulnerabilityTime = 0.5f;
        /// <summary>
        /// Called when this object breaks.
        /// </summary>
        public UnityEvent OnBreak;

        protected Collider _collider;
        protected AudioSource _audio;
        protected Rigidbody _rigidBody;
        protected bool _isInvulnerable = false;
        public bool broken { get; protected set; }

        public virtual void Break()
        {
            if(_isInvulnerable) return;
            StartCoroutine(InvulnerabilityRoutine());
            currentHitsToBreak--;
            if (!broken && currentHitsToBreak <= 0)
            {
                if (_rigidBody)
                {
                    _rigidBody.isKinematic = true;
                }
                
                broken = true;
                display.SetActive(false);
                _collider.enabled = false;
                _audio.PlayOneShot(brokenClip);
                OnBreak?.Invoke();
            }
            else
            {
                _audio.PlayOneShot(hitClip);
            }
        }

        protected virtual void Start()
        {
            _audio = GetComponent<AudioSource>();
            _collider = GetComponent<Collider>();
            TryGetComponent(out _rigidBody);
        }

        protected IEnumerator InvulnerabilityRoutine()
        {
            _isInvulnerable = true;
            yield return new WaitForSeconds(invulnerabilityTime);
            _isInvulnerable = false;
        }
    }
}