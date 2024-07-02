using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace Kamgam.SettingsGenerator.Examples
{
    public class SimplePlayer : MonoBehaviour
    {
        public Button OpenMenuButton;

        public float JumpForce = 30f;
        public float MoveForce = 150f;

#if ENABLE_INPUT_SYSTEM
        public InputActionAsset InputActionAsset;
#else
        public ScriptableObject InputActionAsset;
#endif

#if ENABLE_INPUT_SYSTEM
        protected Rigidbody _rigidbody;
        public Rigidbody Rigidbody
        {
            get
            {
                if (_rigidbody == null)
                {
                    _rigidbody = this.GetComponent<Rigidbody>();
                }
                return _rigidbody;
            }
        }

        protected InputAction _moveAction;
        public InputAction MoveAction
        {
            get
            {
                if (_moveAction == null)
                {
                    _moveAction = InputActionAsset.FindAction("Move");
                }
                return _moveAction;
            }
        }

        protected PlayerInput _playerInput;
        public PlayerInput PlayerInput
        {
            get
            {
                if (_playerInput == null)
                {
                    _playerInput = this.GetComponent<PlayerInput>();
                }
                return _playerInput;
            }
        }


        protected bool _isNearGround = false;
        protected bool _jumpRequested = false;
        protected bool _movementRequested = false;
        protected Vector2 _movementDirection = Vector2.zero;

        public void Start()
        {
            SetControlsIdle(false);
        }

        public void Update()
        {
            _isNearGround = transform.position.y > -0.01f && transform.position.y < 0.05f;

            _movementDirection = MoveAction.ReadValue<Vector2>();
            _movementRequested = _movementDirection.sqrMagnitude > 0.0001f;
        }

        public void OnJump()
        {
            _jumpRequested = true;
        }

        public void OnSpecialMove()
        {
            Debug.Log("Special Move: Though it does nothing in this demo.");
        }

        public void OnOpenMenu()
        {
            OpenMenuButton.OnPointerClick(new PointerEventData(EventSystem.current));
        }

        public void FixedUpdate()
        {
            if (_isNearGround && _jumpRequested)
            {
                Rigidbody.AddForce(Vector3.up * JumpForce, ForceMode.Impulse);
            }
            _jumpRequested = false;

            if (_isNearGround && _movementRequested && Rigidbody.velocity.sqrMagnitude < 49f)
            {
                Rigidbody.AddForce(Vector3.right * _movementDirection.x * MoveForce, ForceMode.Force);
                Rigidbody.AddForce(Vector3.forward * _movementDirection.y * MoveForce, ForceMode.Force);
            }
        }

        public void SetControlsIdle(bool idle)
        {
            if (idle)
                PlayerInput.SwitchCurrentActionMap("idle");
            else
                PlayerInput.SwitchCurrentActionMap("gameplay");
        }

#if UNITY_EDITOR
        public void OnValidate()
        {
            if (InputActionAsset == null)
            {
                string[] guids = UnityEditor.AssetDatabase.FindAssets("t:InputActionAsset");
                if (guids.Length > 0)
                {
                    foreach (var guid in guids)
                    {
                        string path = UnityEditor.AssetDatabase.GUIDToAssetPath(guid);
                        if (path.Contains("InputSystemBinding"))
                        {
                            InputActionAsset = UnityEditor.AssetDatabase.LoadAssetAtPath<InputActionAsset>(path);
                            UnityEditor.EditorUtility.SetDirty(this.gameObject);
                            break;
                        }
                    }
                }
            }
        }
#endif
#endif
    }
}
