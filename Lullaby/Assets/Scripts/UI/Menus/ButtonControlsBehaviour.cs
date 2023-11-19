using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace Lullaby.UI.Menus
{
    public class ButtonControlsBehaviour : ButtonBehaviour
    {
        public GameObject moon;
        public Vector3 moveOffset = new Vector3(0, 0, 0);
        public Vector3 moonRotationOffset = new Vector3(0, 0, 0);
        public bool movePositon = false;
        public float moveTime = 0.3f;
        public float rotateTime = 0.3f;
        public Scheme scheme;
        
        private Vector3 _originalButtonPosition;
        private Vector3 _originalMoonRotation;
        private Vector3 _originalMoonScale;
        private Vector3 _selectedButtonPosition;
        private Sequence _enterSequence;
        private Sequence _exitSequence;
        private bool isEntering = false; // Para evitar conflictos al seleccionar y tener el raton encima tambien
        protected override void Start()
        {
            base.Start();
            _originalButtonPosition = transform.position;
            _originalMoonRotation = moon.transform.eulerAngles;
            _originalMoonScale = moon.transform.localScale;
            _selectedButtonPosition = transform.position + moveOffset;
            moon.transform.localScale = Vector3.zero;
            moon.SetActive(false);
        }
        
        public override void OnPointerEnter(BaseEventData eventData)
        {
            if (!isEntering)
            {
                base.OnPointerEnter(eventData);
                isEntering = true;
                //Sequence s = DOTween.Sequence();
                _exitSequence.Complete();
                _enterSequence = DOTween.Sequence().SetUpdate(true);

                _enterSequence.AppendCallback(() => moon.SetActive(true));
                _enterSequence.AppendCallback(() => moon.GetComponent<Image>().enabled = true);
                _enterSequence.Append(moon.transform.DOScale(_originalMoonScale, 0));
                //moon.SetActive(true);
                //moon.transform.DOScale(_originalMoonScale, rotateTime);
                if (movePositon)
                    _enterSequence.Append(transform.DOMove(_originalButtonPosition + moveOffset, moveTime)
                        .SetEase(Ease.InOutExpo));

                _enterSequence.Join(moon.transform.DORotate(moonRotationOffset, rotateTime));
                if (shakePosition && isEntering)
                {
                    //isEntering = false;
                    _enterSequence.OnComplete(() => tween = transform
                        .DOShakePosition(2f, 5f, 3, 60, false, false,
                            ShakeRandomnessMode.Harmonic).SetLoops(-1, LoopType.Yoyo).SetUpdate(true));
                    //_enterSequence.AppendCallback(() => tween.Play());
                }

            }
            //_enterSequence.Play();
        }

        public override void OnPointerExit(BaseEventData eventData)
        {
            base.OnPointerExit(eventData);
            isEntering = false;
            _enterSequence.Complete();
            tween.Pause();
            //Sequence s = DOTween.Sequence();
            
            _exitSequence = DOTween.Sequence().SetUpdate(true);
            if (movePositon)
            {
                _exitSequence.Append(transform.DOMove(_originalButtonPosition, moveTime).SetEase(Ease.InOutExpo));
            }
            _exitSequence.Join(moon.transform.DORotate(_originalMoonRotation, rotateTime));
            _exitSequence.InsertCallback(rotateTime - 0.1f,() => moon.GetComponent<Image>().enabled = false);
            _exitSequence.Append(moon.transform.DOScale(Vector3.zero, rotateTime));
            _exitSequence.AppendCallback(() => moon.SetActive(false));
            _exitSequence.OnComplete(() => tween.Pause());
            //_exitSequence.Play();
        }
    }
}