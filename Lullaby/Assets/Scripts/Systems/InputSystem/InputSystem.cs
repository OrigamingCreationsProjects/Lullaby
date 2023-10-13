using System;
using System.Collections.Generic;
using Movement.Commands;
using Movement.Components;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Systems
{
    public class InputSystem : MonoBehaviour
    {
        private static InputSystem _instance;
        public static InputSystem Instance => _instance;
        
        [SerializeField] private CharacterMovement _character;
        public CharacterMovement Character
        {
            get => _character;
            set
            {
                _character = value;
                SetCharacter(_character);
            }
        }

        public InputActionAsset actions;
        
        // Acciones disponibles para el personaje y diccionario de comandos
        protected InputAction Move;
        protected InputAction Jump;
        protected InputAction Look;
        // public InputAction Attack1;
        // public InputAction Attack2;
        // public InputAction TakeDamage;
        // public InputAction Dash; 
        private Dictionary<string, ICommand> _commands;

        protected Camera camera;
        
        
        protected const string mouseDeviceName = "Mouse";

        // Dirección a la que mira el jugador: derecha 1, izquierda -1
        private float lastDirection = 1f;
        private float lookDirection = 1f;


        protected virtual void CacheActions()
        {
            Move = actions["Move"];
            Jump = actions["Jump"];
            Look = actions["Look"];
        }
        
        // Asigna la instancia del inputSystem a sí misma si no existe ya una, y si hay un fighter movement asociado, llama a asociar los comandos y las acciones
        protected virtual void Awake()
        {
            CacheActions();
            if (_instance == null)
            {
                _instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            
            if (_character)
            {
                SetCharacter(_character);
            }
        }

        protected void Start()
        {
            camera = Camera.main;
            actions.Enable();
        }

        // Se le llama desde el Awake con el parámetro _character
        public void SetCharacter(CharacterMovement character)
        {
            _commands = new Dictionary<string, ICommand> {
                { "walk", new WalkCommand(character) },
                { "stop", new StopCommand(character) },
                { "jump", new JumpCommand(character) },
                { "doubleJump", new DoubleJumpCommand(character) },
                { "land", new LandingCommand(character) }
            }; // Guarda en el diccionario nuevas entradas con los comandos para cada acción
            
            // Suscrición de eventos al performed de los inputAction correspondientes y activación de estos últimos.
            // Cuando se active performed se llamará al evento suscrito onAcciónCorrespondiente
            Move.performed += OnMove;
            Move.canceled += OnMove;
            Move.Enable();
            
            Jump.performed += OnJump;
            Jump.Enable();

            // Dash.performed += OnDash;
            // Dash.Enable();   

            // Similar para los ataques, aunque lo que se suscribe es el método execute de un comando
            // Attack1.started += context =>
            // {
            //     _commands["attack1"].Execute();
            // };
            // Attack1.Enable();
            //
            // Attack2.started += context =>
            // {
            //     _commands["attack2"].Execute();
            // };
            // Attack2.Enable();
        }

        // Evento al que se llama cuando se activa el input de movimiento
        public void OnMove(InputAction.CallbackContext context)
        {
            Vector2 directionValue = context.ReadValue<Vector2>();
            //lastDirection = value; // Obtenemos el valor de movimiento
            Debug.Log($"OnMove called {context.action}");
            Debug.Log($"OnMove input value {directionValue}");
            var inputDirection = GetMovementDirection();
            
            if (inputDirection.sqrMagnitude > 0) // Si es 0 es que se ha parado
            {
                ((AMovementCommand)_commands["walk"]).SetDirection(inputDirection);
                _commands["walk"].Execute();
            }
            else // Si es 1 se mueve a la derecha
            {
                //lookDirection = value;
                ((AMovementCommand)_commands["stop"]).SetDirection(Vector2.zero);
                ((AMovementCommand)_commands["walk"]).SetDirection(Vector2.zero);
                _commands["stop"].Execute();
            }
        }

        // Evento al que se llama cuando se activa el input de dash
        public void OnDash(InputAction.CallbackContext context)
        {
            Debug.Log("Entramos en OnDash");
            if(lookDirection == 1f) // Dash hacia la derecha
            {
                _commands["dash-right"].Execute();
            }
            else if(lookDirection == -1f) // Dash hacia la izquierda
            {
                _commands["dash-left"].Execute();
            }
        }

        // Evento al que se llama cuando se activa el input de saltar
        public void OnJump(InputAction.CallbackContext context)
        {
            float value = context.ReadValue<float>();
            
            
            // Debug.Log($"OnJump called {context.ReadValue<float>()}");

            if (value == 0f) // Si es 0 es porque está cayendo
            {
                _commands["land"].Execute();
            }
            else // Si es 1 es porque está saltando
            {
                _commands["jump"].Execute();
            }
        }
        public virtual Vector3 GetLookDirection()
        {
            var value = Look.ReadValue<Vector2>();
            
            if (IsLookingWithMouse())
            {
                return new Vector3(value.x, 0, value.y);
            }

            return new Vector3(value.x, 0, value.y);
            //return GetAxisWithCrossDeadZone(value);
        }
        public virtual Vector3 GetMovementDirection()
        {
            //if (Time.time < m_movementDirectionUnlockTime) return Vector3.zero;

            var value = Move.ReadValue<Vector2>();
            return value;
            //return GetAxisWithCrossDeadZone(value);
        }
        public virtual Vector3 GetMovementCameraDirection()
        {
            var direction = GetMovementDirection();

            if (direction.sqrMagnitude > 0)
            {
                var rotation = Quaternion.FromToRotation(camera.transform.up, transform.up);
                direction = rotation * camera.transform.rotation * direction;
                direction = Vector3.ProjectOnPlane(direction, transform.up);
                direction = Quaternion.FromToRotation(transform.up, Vector3.up) * direction;
                direction = direction.normalized;
            }

            return direction;
        }
        public virtual bool IsLookingWithMouse()
        {
            if (Look.activeControl == null)
            {
                return false;
            }

            return Look.activeControl.device.name.Equals(mouseDeviceName);
        }
    }
}