using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace Lullaby
{
    [AddComponentMenu("Lullaby/Misc/Toggle")]
    public class Toggle : MonoBehaviour
    {
        public bool state = true; //Estado inicial del toggle
        public float delay;
        public Toggle[] multiTrigger; // Lo hacemos por si hay que lanzar varios toogles a la vez agrupados

        /// <summary>
        /// Called when the Toggle is activated.
        /// </summary>
        public UnityEvent onActivate;
        
        /// <summary>
        /// Called when the Toggle is deactivated.
        /// </summary>
        public UnityEvent onDeactivate;


        public virtual void Set(bool value)
        {
            StopAllCoroutines();
            StartCoroutine(SetRoutine(value));
        }

        protected virtual IEnumerator SetRoutine(bool value)
        {
            yield return new WaitForSeconds(delay);

            if (value)
            {
                if (!state)
                {
                    state = true;

                    foreach (var toggle in multiTrigger)
                    {
                        toggle.Set(state);
                    }
                    
                    onActivate?.Invoke();
                }
            }
            else if(state)
            {
                state = false;

                foreach (var toggle in multiTrigger)
                {
                    toggle.Set(state);
                }
                
                onDeactivate?.Invoke();
            }
        }
    }
}