using System;
using UnityEngine.Events;

namespace Lullaby.Entities.Events
{
    [Serializable]
    public class EnemyEvents
    {
        /// <summary>
        /// Called when the Player enters this Enemy sight.
        /// </summary>
        public UnityEvent OnPlayerDetected;

        /// <summary>
        /// Called when the Player leaves this Enemy sight.
        /// </summary>
        public UnityEvent OnPlayerEscaped;

        /// <summary>
        /// Called when this Enemy touches a Player.
        /// </summary>
        public UnityEvent OnPlayerContact;

        /// <summary>
        /// Called whe this Enemy takes damage
        /// </summary>
        public UnityEvent OnDamage;

        /// <summary>
        /// Called when this Enemy loses all health
        /// </summary>
        public UnityEvent OnDie;

        /// <summary>
        /// Called when this Enemy dead and disappear
        /// </summary>
        public UnityEvent OnDisappear;
        /// <summary>
        /// Called when this Enemy appear
        /// </summary>
        public UnityEvent OnAppear;

        /// <summary>
        /// Called when this Enemy was revived
        /// </summary>
        public UnityEvent OnRevive;
    }
}