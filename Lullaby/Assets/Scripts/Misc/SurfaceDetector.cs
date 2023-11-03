using System;
using Lullaby.Entities;
using UnityEngine;
using UnityEngine.Events;

namespace Lullaby
{
    [RequireComponent(typeof(Collider))]
    [AddComponentMenu("Lullaby/Misc/Surface Detector")]
    public class SurfaceDetector : MonoBehaviour, IEntityContact
    {
        public bool autoToggle;
        public bool requirePlayer;
        public bool isButton;
        //Audioclips que querriamos reproducir al activar o desactivar el detector
        
        /// <summary>
        /// Called when the Platter is activated.
        /// </summary>
        public UnityEvent OnActivate;
        
        /// <summary>
        /// Called when the Platter is deactivated.
        /// </summary>
        public UnityEvent OnDeactivate;

        protected Collider collider;
        protected Collider entityActivator;
        protected Collider otherActivator;
        
        //Audio Source
        
        /// <summary>
        /// Return true if the Platter is activated.
        /// </summary>
        public bool activated { get; protected set; }

        /// <summary>
        /// Activate this Platter.
        /// </summary>
        public virtual void Activate()
        {
            if (!activated)
            {
                activated = true;
                OnActivate?.Invoke();
            }
        }

        /// <summary>
        /// Deactivates this Platter.
        /// </summary>
        public virtual void Deactivate()
        {
            if (activated)
            {
                activated = false;
                OnDeactivate?.Invoke();
            }
        }
        
        // Start is called before the first frame update
        protected virtual void Start()
        {
            gameObject.tag = GameTags.Platter;
            collider = GetComponent<Collider>();
            if (isButton)
            {
                Mover buttonMover = GetComponentInChildren<Mover>();
                OnActivate.AddListener(buttonMover.ApplyOffset);
                OnDeactivate.AddListener(buttonMover.ResetMover);
            }

        }

        // Update is called once per frame
        protected virtual void Update()
        {
            if (entityActivator || otherActivator)
            {
                var center = collider.bounds.center;
                var contactOffset = Physics.defaultContactOffset + 0.1f; // We add a small offset to avoid floating point errors
                var size = collider.bounds.size + Vector3.up * contactOffset;
                var bounds = new Bounds(center, size);

                var intersectsEntity = entityActivator && bounds.Intersects(entityActivator.bounds);
                var intersectsOther = otherActivator && bounds.Intersects(otherActivator.bounds);

                if (intersectsEntity || intersectsOther)
                {
                    Activate();
                }
                else
                {
                    entityActivator = intersectsEntity ? entityActivator : null; // Si no hay interseccion con el jugador, lo ponemos a null
                    otherActivator = intersectsOther ? otherActivator : null;

                    if (autoToggle)
                    {
                        Deactivate();
                    }
                }    
            }
        }

        public void OnEntityContact(Entity entity)
        {
            if (entity.verticalVelocity.y <= 0 &&
                BoundsHelper.IsBellowPoint(collider, entity.stepPosition))
            {
                if (!requirePlayer || entity is Player)
                {
                    entityActivator = entity.controller;
                }
            }
        }

        protected void OnCollisionStay(Collision collision)
        {
            if (!requirePlayer && !collision.collider.CompareTag(GameTags.Player))
            {
                otherActivator = collision.collider;
            }
        }
    }
}
