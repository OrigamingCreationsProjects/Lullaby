using System;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Lullaby.UI.Menus
{
    public class BackgroundChanger : MonoBehaviour
    {
        public Image dayBackground;
        public Image nightBackground;
        public Image titleImage;
        public float fadeTime = 0.25f;
        [SerializeField] private Vector3 mousePos;        
        [SerializeField] private Color nightTitleColor;
        private Color _defaultTitleColor;
        
        public void ChangeBackground(bool isDay)
        {
            if (isDay)
            {
                titleImage.color = _defaultTitleColor;
                FadeBackgrounds(dayBackground, nightBackground, _defaultTitleColor);
            }
            else
            {
                titleImage.color = nightTitleColor; 
                FadeBackgrounds(nightBackground, dayBackground, nightTitleColor);
            }
        }

        private void FadeBackgrounds(Image backgroundNew, Image backgroundOld, Color inTextColor)
        {
            backgroundNew.DOFade(1.0f, fadeTime);//.SetEase(Ease.OutCubic);
            backgroundOld.DOFade(0.0f, fadeTime);//.SetEase(Ease.OutCubic);
            //titleImage.DOColor(inTextColor, fadeTime);//.SetEase(Ease.OutCubic);
        }

        private void Start()
        {
            _defaultTitleColor = titleImage.color;
        }

        private void LateUpdate()
        {
            mousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
            if (mousePos.x > 0.5f)
            {
                ChangeBackground(false);
            }
            else if (mousePos.x < 0.5f)
            {
                ChangeBackground(true);
            }
        }
    }
}