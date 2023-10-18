using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Lullaby
{
    [RequireComponent(typeof(Collider))]
    public class Sign: MonoBehaviour
    {
        [Header("Sing Settings")] [TextArea(15, 20)]
        public string text = "La tortilla es sin cebolla";
        public float viewAngle = 90f; //Angulo de vision de la camara para que te pueda salir el cartel

        [Header("Canvas Settings")] 
        public Canvas canvas;
        public Text uiText;
        public float scaleDuration = 0.25f;

        [Space(15)] 
        public UnityEvent onShow;
        public UnityEvent onHide;

        protected Vector3 initialScale;
        protected bool showing;
        protected Collider collider;
        protected Camera camera;

        public virtual void Show()
        {
            if (!showing)
            {
                showing = true;
                onShow?.Invoke();
                Scale(Vector3.zero, initialScale);
            }
        }

        public virtual void Hide()
        {
            if (showing)
            {
                showing = false;
                onShow?.Invoke();
                Scale(canvas.transform.localScale, Vector3.zero);
            }
        }

        protected void Scale(Vector3 from, Vector3 to)
        {
            Debug.Log("Intentamos escalar");
            canvas.transform.DOScale(to, scaleDuration).SetEase(Ease.InOutSine);
        }
        
        // protected virtual IEnumerator Scale(Vector3 from, Vector3 to)
        // {
        //     var elapsedTime = 
        // }

        protected void Awake()
        {
            uiText.text = text;
            initialScale = canvas.transform.localScale;
            canvas.transform.localScale = Vector3.zero;
            //Quiza haya que inicializar la escala
            canvas.gameObject.SetActive(true);
            collider = GetComponent<Collider>();
            camera = Camera.main;
        }

        protected void OnTriggerExit(Collider other)
        {
            if (other.CompareTag(GameTags.Player))
            {
                Hide();
            }
        }

        protected virtual void OnTriggerStay(Collider other)
        {
            if (other.CompareTag(GameTags.Player))
            {
                var direction = (other.transform.position - transform.position).normalized; // Direccion hacia el jugador
                var angle = Vector3.Angle(transform.forward, direction);
                //var allowedHeight = other.transform.position.y > collider.bounds.min.y;
                // Si la cámara y el cartel miran en direcciones diferentes significa que la camara esá mirando el cartel
                var inCameraSight = Vector3.Dot(camera.transform.forward, transform.forward) < 0;
                
                if(angle < viewAngle && inCameraSight)
                {
                    Show();
                }
                else
                {
                    Hide();
                }
            }
        }
    }
}