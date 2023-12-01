using System;
using System.Collections.Generic;
using UnityEngine;

namespace Lullaby.Entities
{
    public class EntityStateManager : MonoBehaviour
    {
        public EntityStateManagerEvents events;
    }

    public abstract class EntityStateManager<T> : EntityStateManager where T : Entity<T>
    {
        //La lista general de todos los estados para pasar de uno a otro por indice
        protected List<EntityState<T>> list = new List<EntityState<T>>(); //Lista de todos los estados

        //Diccionario de estados por tipo que tiene la entidad (para acceder a ellos por tipo)
        protected Dictionary<Type, EntityState<T>> entityStates = new Dictionary<Type, EntityState<T>>(); // Diccionario de estados por tipo
        
        /// <summary>
        /// Returns the instance of the current Entity State.
        /// </summary>
        public EntityState<T> current { get; protected set; }
        
        /// <summary>
        /// Returns the instance of the last Entity State.
        /// </summary>
        public EntityState<T> last { get; protected set; }
        
        /// <summary>
        /// Return the index of the current Entity State.
        /// </summary>
        public int index => list.IndexOf(current);
        
        /// <summary>
        /// Return the index of the last Entity State.
        /// </summary>
        public int lastIndex => list.IndexOf(last);
        
        /// <summary>
        /// Return the instance of the Entity associated with this Entity State Manager.
        /// </summary>
        public T entity { get; protected set; } 
        
        protected abstract List<EntityState<T>> GetStateList(); //Cada clase que herede de esta clase debe implementar
                                                                //este metodo y devolver su lista de estados

        protected virtual void InitializeEntity() => entity = GetComponent<T>(); //Inicializamos la entidad
        
        protected virtual void InitializeStates()
        {
            list = GetStateList(); //Obtenemos la lista de estados
            
            foreach (var state in list)
            {
                var type = state.GetType(); //Obtenemos el tipo de estado
                
                if (!entityStates.ContainsKey(type)) //Si no contiene el tipo de estado
                {
                    entityStates.Add(type, state); //Añadimos el tipo y estado
                }
            }

            if (list.Count > 0)
            {
                current = list[0];
            }
        }
        
        /// <summary>
        /// Change to a given Entity State based on its index on the States list.
        /// </summary>
        /// <param name="to">The index of the State you want to change to.</param>
        public virtual void Change(int to)
        {
            if(to >= 0 && to < list.Count)
            {
                Change(list[to]);
            }
        }
        /// <summary>
        /// Change to a given Entity State based on its class type.
        /// </summary>
        /// <typeparam name="TState">The class of the state you want to change to.</typeparam>
        public virtual void Change<TState>() where TState : EntityState<T>
        {
            var type = typeof(TState);
            
            if(entityStates.ContainsKey(type))
            {
                Change(entityStates[type]);
            }
        }
        
        /// <summary>
        /// Changes to a given Entity State based on its instance.
        /// </summary>
        /// <param name="to">The instance of the Entity State you want to change to.</param>
        public virtual void Change(EntityState<T> to)
        {
            if (to != null && Time.timeScale > 0)
            {
                if (current != null)
                {
                    current.Exit(entity);
                    events.onExit.Invoke(current.GetType());
                    last = current;
                }

                current = to;
                current.Enter(entity);
                events.onEnter.Invoke(current.GetType());
                events.onChange?.Invoke();
            }
        }
        /// <summary>
        /// Returns true if the type of the current State matches a given one.
        /// </summary>
        /// <param name="type">The type you want to compare to.</param>
        public virtual bool IsCurrentOfType(params Type[] types) // params permite pasar un numero variable de argumentos
        {
            if(current == null)
                return false;
            
            foreach (var type in types)
            {
                if (current.GetType() == type)
                {
                    return true;
                }
            }
            return false;
        }
        
        /// <summary>
        /// Returns true if the manager has a State of a given type.
        /// </summary>
        /// <param name="type">The Type of the State you want to find.</param>
        public virtual bool ContainsStateOfType(Type type) => entityStates.ContainsKey(type);

        public virtual void Step()
        {
            if (current != null && Time.timeScale > 0)
                current.Step(entity);
        }

        public virtual void OnContact(Collider other)
        {
            if(current != null && Time.timeScale > 0)
                current.OnContact(entity, other);
        }

        protected virtual void Start()
        {
            InitializeEntity();
            InitializeStates();
        }
    }
}