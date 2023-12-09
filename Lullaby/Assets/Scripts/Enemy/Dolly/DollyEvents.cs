using System;
using UnityEngine.Events;

namespace Lullaby.Entities.Enemies
{
    [Serializable]
    public class DollyEvents
    {
        public UnityEvent<Dolly> OnDamage;
        
        public UnityEvent<Dolly> OnStopMoving;
        
        public UnityEvent<Dolly> OnStartMoving;
        
        public UnityEvent<Dolly> OnRetreat;
        
        /// <summary>
        /// Called when this Enemy appear
        /// </summary>
        public UnityEvent OnAppear;
        /// <summary>
        /// Called when this Enemy dead and disappear
        /// </summary>
        public UnityEvent OnDisappear;
        
        /// <summary>
        /// Called when this Dolly starts attacking
        /// </summary>
        public UnityEvent<bool> OnAttack;
    }
}