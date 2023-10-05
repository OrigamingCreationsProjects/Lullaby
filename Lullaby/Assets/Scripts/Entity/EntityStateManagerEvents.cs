using System;
using UnityEngine.Events;

namespace Lullaby.Entities
{
    [Serializable]
    public class EntityStateManagerEvents
    {
        /// <summary>
        /// Called when there's a state change.
        /// </summary>
        public UnityEvent onChange;

        /// <summary>
        /// Called when entering a state.
        /// </summary>
        public UnityEvent<Type> onEnter; // Type es un tipo de dato que nos permite almacenar el tipo de dato de una clase

        /// <summary>
        /// Called when exiting a state.
        /// </summary>
        public UnityEvent<Type> onExit;    
    }
}