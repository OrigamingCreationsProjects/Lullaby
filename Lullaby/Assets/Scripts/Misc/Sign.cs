using System;
using System.Collections;
using DG.Tweening;
using TMPro;
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
        public Vector3 backPosition = new Vector3(0, 0.82f, 0);
        public  Ease scaleAndMoveEase = Ease.OutQuart;
        [Space(15)] 
        public UnityEvent onShow;
        public UnityEvent onHide;
        
        protected Vector3 initialScale;
        protected Vector3 finalPosition;
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
                Move(backPosition, finalPosition);
            }
        }

        public virtual void Hide()
        {
            if (showing)
            {
                showing = false;
                onShow?.Invoke();
                Scale(canvas.transform.localScale, Vector3.zero);
                Move(canvas.transform.localPosition, backPosition);
            }
        }

        protected void Scale(Vector3 from, Vector3 to)
        {
            Debug.Log("Intentamos escalar");
            //Sequence showSequence = DOTween.Sequence();
            // showSequence.Append(canvas.transform.DOScaleY(to.y, scaleDuration).SetEase(Ease.InOutSine));
            // showSequence.Insert(0, canvas.transform.DOMoveY(finalPosition.y, scaleDuration).SetEase(Ease.InOutSine));
            // showSequence.Insert(0, canvas.transform.DOScaleX(to.x, scaleDuration / 2).SetEase(Ease.InOutSine));
            // showSequence.Insert(2, canvas.transform.DOScaleZ(to.z, scaleDuration / 2).SetEase(Ease.InOutSine));
            // canvas.transform.DOScaleY(to.y, scaleDuration).SetEase(Ease.InOutSine);
            // canvas.transform.DOScaleX(to.x, scaleDuration).SetEase(Ease.InOutSine);
            // canvas.transform.DOScaleZ(to.z, scaleDuration).SetEase(Ease.InOutSine);
            canvas.transform.DOScale(to, scaleDuration).SetEase(scaleAndMoveEase);
          

        }
        protected void Move(Vector3 from, Vector3 to)
        {
            Debug.Log("Intentamos escalar");
            
            // Sequence showSequence = DOTween.Sequence();
            // showSequence.Append(canvas.transform.DOScaleY(to.y, scaleDuration).SetEase(Ease.InOutSine));
            // showSequence.Append(canvas.transform.DOScaleX(to.x, scaleDuration / 2).SetEase(Ease.InOutSine));
            // showSequence.Insert(2, canvas.transform.DOScaleZ(to.z, scaleDuration / 2).SetEase(Ease.InOutSine));
            canvas.transform.DOMoveY(to.y, scaleDuration).SetEase(scaleAndMoveEase);
            

        }
        
        // protected virtual IEnumerator Scale(Vector3 from, Vector3 to)
        // {
        //     var elapsedTime = 
        // }

        protected void Awake()
        {
            uiText.text = text;
            finalPosition = canvas.transform.position;
            canvas.transform.localPosition = backPosition;
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