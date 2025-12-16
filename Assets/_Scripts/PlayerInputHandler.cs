using System;
using Minis;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

namespace Input
{
    public class PlayerInputHandler : MonoBehaviour
    {
        [SerializeField] private float regularInputSpeed;
        [SerializeField] private float midiInputSpeed;
        [SerializeField] private float uiNavigationInterval = 0.3f;

        private PlayerInput _playerInput;
        private Vector2 _movementValue;
        private bool _isMidiDeviceSet;

        private MidiDevice _leftDeck;
        private MidiDevice _rightDeck;

        private float _uiLeftNavigationElapsed;
        private float _uiRightNavigationElapsed;

        public event Action<Vector2> Movement;
        public event Action Interaction;
        public event Action<Vector2> HeadMovement;
        
        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
        }

        private void OnEnable()
        {
            InputSystem.onDeviceChange += OnDeviceChanged;
            _playerInput.actions["Move"].performed += OnMovePerformed;
            _playerInput.actions["Interact"].performed += OnInteractPerformed;
        }

        private void OnDisable()
        {
            InputSystem.onDeviceChange -= OnDeviceChanged;
            _playerInput.actions["Move"].performed -= OnMovePerformed;
            _playerInput.actions["Interact"].performed -= OnInteractPerformed;

            if (_leftDeck != null)
            {
                _leftDeck.onWillControlChange -= OnLeftDeckValue;
                _leftDeck.onWillNoteOn -= OnLeftDeckKeyDown;
            }
            
            if (_rightDeck != null)
            {
                _rightDeck.onWillControlChange -= OnRightDeckValue;
                _rightDeck.onWillNoteOn -= OnRightDeckKeyDown;
            }
        }

        private void Update()
        {
            _uiLeftNavigationElapsed += Time.deltaTime;
            _uiRightNavigationElapsed += Time.deltaTime;
            
            Vector2 movementThisFrame = _movementValue * (regularInputSpeed * Time.deltaTime);
            Movement?.Invoke(movementThisFrame);
            _movementValue = Vector2.zero;
        }
        
        private void OnHeadRotation(InputAction.CallbackContext obj)
        {
            HeadMovement?.Invoke(obj.ReadValue<Vector2>());
        }

        private void OnDeviceChanged(InputDevice arg1, InputDeviceChange arg2)
        {
            if (arg1 is not MidiDevice midiDevice)
            {
                return;
            }
            
            switch (midiDevice.channel)
            {
                case 1:
                    _leftDeck = midiDevice;
                    _leftDeck.onWillControlChange += OnLeftDeckValue;
                    _leftDeck.onWillNoteOn += OnLeftDeckKeyDown;
                    break;
                case 2:
                    _rightDeck = midiDevice;
                    _rightDeck.onWillControlChange += OnRightDeckValue;
                    _rightDeck.onWillNoteOn += OnRightDeckKeyDown;
                    break;
            }
        }

        private void OnLeftDeckKeyDown(MidiNoteControl midiNoteControl, float f)
        {
            if (midiNoteControl.noteNumber == 7)
            {
                Interaction?.Invoke();
                BaseEventData eventData = new BaseEventData(EventSystem.current);
                eventData.Reset();
                ExecuteEvents.Execute(EventSystem.current?.currentSelectedGameObject, eventData, ExecuteEvents.submitHandler);
            }
        }
        
        private void OnRightDeckKeyDown(MidiNoteControl midiNoteControl, float f)
        {
            if (midiNoteControl.noteNumber == 7)
            {
               
                Interaction?.Invoke();
                BaseEventData eventData = new BaseEventData(EventSystem.current);
                eventData.Reset();
                ExecuteEvents.Execute(EventSystem.current?.currentSelectedGameObject, eventData, ExecuteEvents.submitHandler);
            }
        }
        
        private void OnLeftDeckValue(MidiValueControl control, float value)
        {
            if (control.controlNumber != 10 && control.controlNumber != 9)
            {
                return;
            }
            
                //value = -value * 2.0f - 1.0f;
            value = value > 0.5f ? 1.0f : -1.0f;
            _movementValue = value * midiInputSpeed * Vector2.up;

            if (_uiLeftNavigationElapsed < uiNavigationInterval)
            {
                return;
            }

            _uiLeftNavigationElapsed = 0.0f;
            
            if (-value > 0.0f)
            {
                if (!EventSystem.current?.currentSelectedGameObject)
                {
                    return;
                }

                AxisEventData eventData = new AxisEventData(EventSystem.current);
                eventData.Reset();
                eventData.moveDir = MoveDirection.Up;
                eventData.moveVector = Vector2.up;
                ExecuteEvents.Execute(EventSystem.current?.currentSelectedGameObject, eventData, ExecuteEvents.moveHandler);
            }
            else
            {
                if (!EventSystem.current?.currentSelectedGameObject)
                {
                    return;
                }

                AxisEventData eventData = new AxisEventData(EventSystem.current);
                eventData.Reset();
                eventData.moveDir = MoveDirection.Down;
                eventData.moveVector = Vector2.down;
                ExecuteEvents.Execute(EventSystem.current?.currentSelectedGameObject, eventData, ExecuteEvents.moveHandler);
            }
        }
        
        private void OnRightDeckValue(MidiValueControl control, float value)
        {
            if (control.controlNumber != 10 && control.controlNumber != 9)
            {
                return;
            }
            
           
            //value = value * 2.0f - 1.0f;
            value = value > 0.5f ? 1.0f : -1.0f;
            _movementValue = value * midiInputSpeed * Vector2.right;
            
            if (_uiRightNavigationElapsed < uiNavigationInterval)
            {
                return;
            }

            _uiRightNavigationElapsed = 0.0f;
            
            if (value < 0.0f)
            {
                if (!EventSystem.current?.currentSelectedGameObject)
                {
                    return;
                }
                
                AxisEventData eventData = new AxisEventData(EventSystem.current);
                eventData.Reset();
                eventData.moveDir = MoveDirection.Right;
                eventData.moveVector = Vector2.right;
                ExecuteEvents.Execute(EventSystem.current?.currentSelectedGameObject, eventData, ExecuteEvents.moveHandler);
            }
            else
            {
                if (!EventSystem.current?.currentSelectedGameObject)
                {
                    return;
                }

                AxisEventData eventData = new AxisEventData(EventSystem.current);
                eventData.Reset();
                eventData.moveDir = MoveDirection.Left;
                eventData.moveVector = Vector2.left;
                ExecuteEvents.Execute(EventSystem.current?.currentSelectedGameObject, eventData, ExecuteEvents.moveHandler);
            }
        }

        public void OnMovePerformed(InputAction.CallbackContext obj)
        {
            _movementValue = obj.ReadValue<Vector2>();
        }

        private void OnInteractPerformed(InputAction.CallbackContext obj)
        {
            Interaction?.Invoke();
        }
    }
}
