using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Lullaby
{
    [AddComponentMenu("Lullaby/Misc/Health")]
    public class Health : MonoBehaviour
    {
        public int initial = 100;

        public int max = 100;
        
        public float coolDown = 1f;
        
        /// <summary>
        /// Called when the health count changed.
        /// </summary>
        public UnityEvent onChange;

        /// <summary>
        /// Called when it receives damage.
        /// </summary>
        public UnityEvent onDamage;

        //MEMBER VARIABLES
        protected int currentHealth;
        protected float lastDamageTime;

        public int current
        {
            get { return currentHealth; }

            protected set
            {
                var last = currentHealth;

                if (value != last)
                {
                    Debug.Log("Detectamos que se ha actualizado esto");
                    currentHealth = Mathf.Clamp(value, 0, max);
                    onChange?.Invoke();
                }
            }
        }
        
        /// <summary>
        /// Returns true if the Health is empty.
        /// </summary>
        public virtual bool isEmpty => current == 0;

        /// <summary>
        /// Returns true if it's still recovering from the last damage.
        /// </summary>
        public virtual bool recovering => Time.time < lastDamageTime + coolDown;
        
        /// <summary>
        /// Set the current health to a given amount.
        /// </summary>
        /// <param name="amount">The total health you want to set</param>
        public virtual void Set(int amount) => current = amount;

        /// <summary>
        /// Increases the amount of health.
        /// </summary>
        /// <param name="amount">The amount you want to increase.</param>
        public virtual void Increase(int amount) => current += amount;

        /// <summary>
        /// Decreases the amount of health.
        /// </summary>
        /// <param name="amount">The amount you want to decrease.</param>
        public virtual void Damage(int amount)
        {
            if (!recovering)
            {
                current -= Math.Abs(amount);
                lastDamageTime = Time.time;
                Debug.Log($"Se daña al jugador quitando {amount} puntos de vida. La vida actual es {current}");
                onDamage?.Invoke();
            }
        }

        /// <summary>
        /// Set the current health back to its initial value.
        /// </summary>
        public virtual void ResetHealth() => current = initial;

        public virtual void Awake() => current = initial;
    }
}
