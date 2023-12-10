using System;
using UnityEngine;
using UnityEngine.UI;

namespace Lullaby.UI
{
    public class ContextIndicator : MonoBehaviour
    {
        public ButtonType buttonType;
        [SerializeField] private GameObject contextIndicatorContainer;
        [SerializeField] private Image icon;

        private Sprite buttonReference;
        private void Start()
        {
            RefreshUI();
            contextIndicatorContainer.SetActive(false);
        }

        private void UpdateButtonReference()
        {
            switch (buttonType)
            {
                case ButtonType.buttonEast:
                    buttonReference = ContextIndicatorSystem.instance.buttonEast;
                    break;
                case ButtonType.buttonNorth:
                    buttonReference = ContextIndicatorSystem.instance.buttonNorth;
                    break;
                case ButtonType.buttonWest:
                    buttonReference = ContextIndicatorSystem.instance.buttonWest;
                    break;
                case ButtonType.buttonSouth:
                    buttonReference = ContextIndicatorSystem.instance.buttonSouth;
                    break;
                case ButtonType.leftStick:
                    buttonReference = ContextIndicatorSystem.instance.leftStick;
                    break;
                case ButtonType.leftStickPress:
                    buttonReference = ContextIndicatorSystem.instance.leftStickPress;
                    break;
                case ButtonType.backButton:
                    buttonReference = ContextIndicatorSystem.instance.backButton;
                    break;
            }
        }
        private void RefreshUI()
        {
            UpdateButtonReference();
            icon.sprite = buttonReference;
            //icon.SetNativeSize();
        }

        public void ShowContextIndicator()
        {
            contextIndicatorContainer.SetActive(true);
            RefreshUI();
        }
        
        public void HideContextIndicator()
        {
            contextIndicatorContainer.SetActive(false);
        }
    }

    public enum ButtonType
    {
        buttonNorth,
        buttonSouth,
        buttonEast,
        buttonWest,
        leftStick,
        leftStickPress,
        rightStick,
        rightStickPress,
        backButton,
    }
}