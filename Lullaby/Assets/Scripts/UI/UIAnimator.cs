using System;
using UnityEngine;
using UnityEngine.Events;

namespace Lullaby.UI
{
    public class UIAnimator : MonoBehaviour
    {
        /// <summary>
        /// Called when the Show action is invoked.
        /// </summary>
        public UnityEvent OnShow;

        /// <summary>
        /// Called when the Hide action is invoked.
        /// </summary>
        public UnityEvent OnHide;
        
        public bool hidenOnAwake;
        // Declaramos los triggers del animator en variables porque el acceso a strings directamente es más lento
        // y ademas evitamos errores al escribir mal el nombre del trigger 
        public string normalTrigger = "Normal";
        public string showTrigger = "Show";
        public string hideTrigger = "Hide";

        protected Animator _animator;
        protected Canvas _canvas;

        /// <summary>
        /// Show the UI element
        /// </summary>
        public virtual void Show()
        {
            //Aqui lanzariamos el trigger del animator
            OnShow?.Invoke();
        }

        /// <summary>
        /// Hide the UI element
        /// </summary>
        public virtual void Hide()
        {
            //Aqui lanzariamos el trigger del animator
            OnHide?.Invoke();
        }
        
        /// <summary>
        /// Change de active state passing a given value.
        /// </summary>
        /// <param name="value">The value you want to pass</param>
        public virtual void SetActive(bool value) => gameObject.SetActive(value);


        protected virtual void Awake()
        {
            //_animator = GetComponent<Animator>();
            _canvas = GetComponent<Canvas>();
            if (hidenOnAwake)
            {
                //Aqui lanzariamos el trigger del animator
                _canvas.enabled = false;
            }
        }
    }
}