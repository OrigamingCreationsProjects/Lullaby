using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(EventTrigger))]
public class ButtonBehaviour : MonoBehaviour
{
    public float scaleOffset = 0.35f;
    public Vector3 moveOffset = new Vector3(0, 0, 0);
    public float moveTime = 0.3f;
    public bool shakePosition = true;
    public bool movePositon = false;
    
    private Button _button;
    private string _onButtonSound = "OnButtonSound";
    private string _pressButtonSound = "PressButtonSound";
    private EventTrigger _eventTrigger;
    private Vector3 _originalPosition;
    private Vector3 _originalScale;
    private Tweener tween;
    
    private void Start()
    {
        _originalPosition = transform.position;
        _originalScale = transform.localScale;
        _button = GetComponent<Button>();
        _button.onClick.AddListener(ButtonClicked);
        _eventTrigger = GetComponent<EventTrigger>();
        //_eventTrigger.OnPointerClick.AddListener(ClickSuspect);
        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = EventTriggerType.PointerEnter;

        // Agrega una función de delegado para manejar el evento PointerClick
        entry.callback.AddListener((data) => {OnPointerEnter((PointerEventData)data); });

        // Agrega la entrada del evento al EventTrigger
        _eventTrigger.triggers.Add(entry);
        EventTrigger.Entry entry2 = new EventTrigger.Entry();
        entry2.eventID = EventTriggerType.PointerExit;

        // Agrega una función de delegado para manejar el evento PointerClick
        entry2.callback.AddListener((data) => {OnPointerExit((PointerEventData)data); });

        // Agrega la entrada del evento al EventTrigger
        _eventTrigger.triggers.Add(entry2);
        
        EventTrigger.Entry entry3 = new EventTrigger.Entry();
        entry3.eventID = EventTriggerType.PointerClick;

        // Agrega una función de delegado para manejar el evento PointerClick
        entry3.callback.AddListener((data) => {OnPointerExit((PointerEventData)data); });

        // Agrega la entrada del evento al EventTrigger
        _eventTrigger.triggers.Add(entry3);
        
        //DOTWEEN
        tween = transform.DOShakePosition(2f, 5f, 3, 60, false, false, 
                ShakeRandomnessMode.Harmonic).SetLoops(-1, LoopType.Yoyo);
        tween.Pause();

    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // El ratón está sobre el botón
        Debug.Log("El ratón está sobre el botón.");
        //SoundManager.Instance.PlayOneShot("OnButtonSound");
        transform.DOScale(new Vector3(
                transform.localScale.x + scaleOffset, 
                transform.localScale.y + scaleOffset, 
                transform.localScale.z + scaleOffset), 0.1f).SetEase(Ease.InOutExpo);

        transform.DOMove(_originalPosition + moveOffset, moveTime).SetEase(Ease.InOutExpo);

        if (shakePosition)
        {
            tween.Play();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // El ratón ha salido del botón
        Debug.Log("El ratón ha salido del botón.");
        transform.DOScale(_originalScale, 0.1f).SetEase(Ease.InOutExpo);
        transform.DOMove(_originalPosition, moveTime).SetEase(Ease.InOutExpo);
        if(shakePosition) tween.Pause();
    }

    public void ButtonClicked()
    {
        //SoundManager.Instance.Play("PressButtonSound");
    }
    
}
