using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Lullaby.Entities
{
    public abstract class EntityState<T> where T : Entity<T>
    {
        public UnityEvent onEnter;
        public UnityEvent onExit;
        
        public float timeSinceEntered { get; protected set; }

        public void Enter(T entity)
        {
            //Debug.Log($"Se entra en el estado {entity.states.current.GetType()}");
            timeSinceEntered = 0f;
            onEnter?.Invoke();
            OnEnter(entity);
        }

        public void Exit(T entity)
        {
            //Debug.Log($"Se sale del estado {entity.states.current.GetType()}");
            onExit?.Invoke();
            OnExit(entity);
        }

        public void Step(T entity)
        {
            OnStep(entity);
            timeSinceEntered += Time.deltaTime; // Sumamos el tiempo que ha pasado desde que entramos en el estado
        }
        /// <summary>
        /// Called when this state is invoked
        /// </summary>
        protected abstract void OnEnter(T entity);

        /// <summary>
        /// Called when this state change to another
        /// </summary>
        protected abstract void OnExit(T entity);
        
        /// <summary>
        /// Called EVERY FRAME when this state is active
        /// </summary>
        public abstract void OnStep(T entity);
        
        /// <summary>
        /// Called when the entity is in contact with another a collider
        /// </summary>
        public abstract void OnContact(T entity, Collider other);

        /// <summary>
        /// Returns a new instance of the Entity State with a given type name.
        /// </summary>
        /// <param name="typeName">The type name of the Entity State class.</param>
        public static EntityState<T> CreateFromString(string typeName)
        {
            return (EntityState<T>)System.Activator.CreateInstance(System.Type.GetType(typeName)); // Creamos una instancia de la clase que se pasa por parametro
        }
        
        /// <summary>
        /// Returns a new list with instances of the Entity States matching the array of type names.
        /// </summary>
        /// <param name="typeNames">The array of type names.</param>
        public static List<EntityState<T>> CreateListFromStringArray(string[] typeNames)
        {
            var list = new List<EntityState<T>>();
            foreach (string typeName in typeNames)
            {
                list.Add(CreateFromString(typeName));
            }

            return list; 
        }
    }
}
