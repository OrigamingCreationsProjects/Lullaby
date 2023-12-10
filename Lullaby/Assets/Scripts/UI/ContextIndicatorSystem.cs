using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Lullaby.UI
{
    public class ContextIndicatorSystem : Singleton<ContextIndicatorSystem>
    {
        [SerializeField] private Image contextImage;
        
        private PlayerInput playerInput;
        private string currentScheme;
        private Dictionary<string, int> dictionary= new Dictionary<string,int> {
                {"Keyboard",0},
                {"PlayStation",1},
                {"Switch", 2},
                {"Xbox",3},
                {"Mobile", 4}
        };
        public Sprite buttonEast {get{return _buttonEast[dictionary[currentScheme]];}}
        public Sprite buttonWest {get{return _buttonWest[dictionary[currentScheme]];}}
        public Sprite buttonNorth{get{return _buttonNorth[dictionary[currentScheme]];}}
        public Sprite buttonSouth{get{return _buttonSouth[dictionary[currentScheme]];}}
        public Sprite dpad{get{return _dpad[dictionary[currentScheme]];}}
        public Sprite dpadDown{get{return _dpadDown[dictionary[currentScheme]];}}
        public Sprite dpadUp{get{return _dpadUp[dictionary[currentScheme]];}}
        public Sprite dpadLeft{get{return _dpadLeft[dictionary[currentScheme]];}}
        public Sprite dpadRight{get{return _dpadRight[dictionary[currentScheme]];}}
        public Sprite leftShoulder{get{return _leftShoulder[dictionary[currentScheme]];}}
        public Sprite leftStick{get{return _leftStick[dictionary[currentScheme]];}}
        public Sprite leftStickPress{get{return _leftStickPress[dictionary[currentScheme]];}}
        public Sprite leftTrigger{get{return _leftTrigger[dictionary[currentScheme]];}}
        public Sprite rightShoulder{get{return _rightShoulder[dictionary[currentScheme]];}}
        public Sprite rightStick{get{return _rightStick[dictionary[currentScheme]];}}
        public Sprite rightStickPress{get{return _rightStickPress[dictionary[currentScheme]];}}
        public Sprite rightTrigger{get{return _rightTrigger[dictionary[currentScheme]];}}
        public Sprite selectButton{get{return _selectButton[dictionary[currentScheme]];}}
        public Sprite startButton{get{return _startButton[dictionary[currentScheme]];}}
        public Sprite backButton{get{return _backButton[dictionary[currentScheme]];}}
                
        [SerializeField]private Sprite[] _buttonEast;
        [SerializeField]private Sprite [] _buttonWest ;
        [SerializeField]private Sprite []_buttonNorth;
        [SerializeField]private Sprite []_buttonSouth;
        [SerializeField]private Sprite []_dpad;
        [SerializeField]private Sprite []_dpadDown;
        [SerializeField]private Sprite []_dpadUp;
        [SerializeField]private Sprite []_dpadLeft;
        [SerializeField]private Sprite []_dpadRight;
        [SerializeField]private Sprite []_leftShoulder;
        [SerializeField]private Sprite []_leftStick;
        [SerializeField]private Sprite []_leftStickPress;
        [SerializeField]private Sprite []_leftTrigger;
        [SerializeField]private Sprite []_rightShoulder;
        [SerializeField]private Sprite []_rightStick;
        [SerializeField]private Sprite []_rightStickPress;
        [SerializeField]private Sprite []_rightTrigger;
        [SerializeField]private Sprite []_selectButton;
        [SerializeField]private Sprite []_startButton;
        [SerializeField]private Sprite []_backButton;
        
        private void Awake()   
        {
            playerInput = FindObjectOfType<PlayerInput>();
            SetControlScheme();
        }

        private void Update()
        { 
            SetControlScheme();
        }

        private void SetControlScheme() 
        {
            if(currentScheme != playerInput.currentControlScheme)
            {
                currentScheme = playerInput.currentControlScheme;
                //contextImage.sprite = buttonSouth; Im
            }
        }
    }
}