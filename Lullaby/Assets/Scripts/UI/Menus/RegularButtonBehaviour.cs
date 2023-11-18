using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lullaby.UI.Menus
{
    [RequireComponent(typeof(EventTrigger))]
    public class RegularButtonBehaviour : MonoBehaviour
    {
        public float scaleOffset = 0.35f;
        public bool shakePosition = true;

        protected Button _button;
        private string _onButtonSound = "OnButtonSound";
        private string _pressButtonSound = "PressButtonSound";
        private EventTrigger _eventTrigger;
        private Vector3 _originalScale;
        protected Tweener tween;

        protected virtual void Awake()
        {
            _originalScale = transform.localScale;
            _button = GetComponent<Button>();
            if (!_button)
            {
                _button = GetComponentInChildren<Button>();
            }

            _button.onClick.AddListener(ButtonClicked);

            #region -- EVENTOS DE POINTER DEL OBJETO --

            _eventTrigger = GetComponent<EventTrigger>();

            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;

            // Agrega una función de delegado para manejar el evento PointerClick
            entry.callback.AddListener((data) => { OnPointerEnter((BaseEventData)data); });

            // Agrega la entrada del evento al EventTrigger
            _eventTrigger.triggers.Add(entry);
            EventTrigger.Entry entry2 = new EventTrigger.Entry();
            entry2.eventID = EventTriggerType.PointerExit;

            // Agrega una función de delegado para manejar el evento PointerClick
            entry2.callback.AddListener((data) => { OnPointerExit((BaseEventData)data); });

            // Agrega la entrada del evento al EventTrigger
            _eventTrigger.triggers.Add(entry2);

            EventTrigger.Entry entry3 = new EventTrigger.Entry();
            entry3.eventID = EventTriggerType.PointerClick;

            // Agrega una función de delegado para manejar el evento PointerClick
            entry3.callback.AddListener((data) => { OnPointerExit((BaseEventData)data); });

            // Agrega la entrada del evento al EventTrigger
            _eventTrigger.triggers.Add(entry3);

            #endregion

            #region -- EVENTOS DE POINTER EN EL BUTTON --

            EventTrigger buttonTrigger = _button.gameObject.GetComponent<EventTrigger>();
            if (buttonTrigger == null)
            {
                buttonTrigger = _button.gameObject.AddComponent<EventTrigger>();
            }
            EventTrigger.Entry entry4 = new EventTrigger.Entry();
            entry4.eventID = EventTriggerType.Select;
            
            // Agrega una función de delegado para manejar el evento Select
            entry4.callback.AddListener((data) => OnPointerEnter(data));
            
            buttonTrigger.triggers.Add(entry4);
            
            EventTrigger.Entry entry5 = new EventTrigger.Entry();
            entry5.eventID = EventTriggerType.Deselect;
            
            // Agrega una función de delegado para manejar el evento Deselect
            entry5.callback.AddListener((data)=> OnPointerExit(data));
            
            // Agrega la entrada del evento al EventTrigger
            buttonTrigger.triggers.Add(entry5);

            #endregion

            tween = transform.DOShakePosition(2f, 5f, 3, 60, false, false,
                ShakeRandomnessMode.Harmonic).SetLoops(-1, LoopType.Yoyo).SetUpdate(true);
            tween.Pause();

        }

        public void OnPointerEnter(BaseEventData eventData)
        {
            
            //SoundManager.Instance.PlayOneShot("OnButtonSound");
            transform.DOScale(new Vector3(
                transform.localScale.x + scaleOffset,
                transform.localScale.y + scaleOffset,
                transform.localScale.z + scaleOffset), 0.1f).SetEase(Ease.InOutExpo).SetUpdate(true);
            if (shakePosition)
            {
                tween.Play();
            }
            
        }

        public void OnPointerExit(BaseEventData eventData)
        {
            // El ratón ha salido del botón
            Debug.Log("El ratón ha salido del botón.");
            transform.DOScale(_originalScale, 0.1f).SetEase(Ease.InOutExpo).SetUpdate(true);

            if (shakePosition) tween.Pause();
        }

        public virtual void ButtonClicked()
        {
            //SoundManager.Instance.Play("PressButtonSound");
        }
    }
}
