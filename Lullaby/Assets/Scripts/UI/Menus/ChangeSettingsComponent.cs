using System;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

namespace Lullaby.UI.Menus
{
    public class ChangeSettingsComponent : MonoBehaviour
    {
        // PARAMETERS TO CHANGE
        [Header("General parameters to change")]
        public TextMeshProUGUI text;
        public Color selectedColor;
        [Header("Language parameters to change")]
        public bool isForLanguage = false;
        public Image[] buttonBackgrounds = new Image[2];
        public int currentButtonSelected = 0;
        public Sprite selectedButtonBackground;
        public Sprite deselectedButtonBackground;
        
        protected Color _initialColor;

        protected void Start()
        {
            _initialColor = text.color;
            if (isForLanguage)
            {
                buttonBackgrounds[currentButtonSelected].sprite = deselectedButtonBackground;
                for (int i = 0; i < buttonBackgrounds.Length; i++)
                {
                    if (i == currentButtonSelected)
                    {
                        buttonBackgrounds[i].sprite = deselectedButtonBackground;
                    }
                    else
                    {
                        buttonBackgrounds[i].gameObject.SetActive(false);
                    }
                }
            }
        }

        public virtual void TextSelectedBehaviour()
        {
            text.color = selectedColor;
        }
        
        public virtual void TextDeselectedBehaviour()
        {
            text.color = _initialColor;
        }

        public virtual void ChangeButtonSelected(int index)
        {
            currentButtonSelected = index;
            for (int i = 0; i < buttonBackgrounds.Length; i++)
            {
                if (i == index)
                {
                    buttonBackgrounds[i].sprite = selectedButtonBackground;
                }
                else
                {
                    buttonBackgrounds[i].sprite = deselectedButtonBackground;
                }
            }
            //buttonBackground.sprite = selectedButtonBackground;
        }

        public virtual void ActivateButtonBackgrounds()
        {
            for (int i = 0; i < buttonBackgrounds.Length; i++)
            {
                buttonBackgrounds[i].gameObject.SetActive(true);
            }
            buttonBackgrounds[currentButtonSelected].sprite = selectedButtonBackground;
        }
        
        public virtual void DeactivateButtonBackgrounds()
        {
            for (int i = 0; i < buttonBackgrounds.Length; i++)
            {
                if (i != currentButtonSelected)
                {
                    buttonBackgrounds[i].gameObject.SetActive(false);
                }
            }
            buttonBackgrounds[currentButtonSelected].sprite = deselectedButtonBackground;
        }
    }
}