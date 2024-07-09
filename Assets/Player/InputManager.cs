using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    // private PlayerInput.OnFootActions onFoot;

    // private PlayerController playerController;

    // void Awake()
    // {
    //     playerInput = new PlayerInput();
    //     onFoot = playerInput.OnFoot;
    //     playerController = GetComponent<PlayerController>();

    //     onFoot.Jump.performed += ctx => playerController.Jump();
    //     //onFoot.Crouch.performed += ctx => playerController.Crouch(); // Update your PlayerController script to handle Crouch if needed
    //     onFoot.Sprint.performed += ctx => playerController.StartSprint();
    //     onFoot.Sprint.canceled += ctx => playerController.StopSprint();
    // }

    // void FixedUpdate()
    // {
    //     playerController.ProcessMove(onFoot.Movement.ReadValue<Vector2>());
    // }

    // void LateUpdate()
    // {
    //     playerController.ProcessLook(onFoot.Look.ReadValue<Vector2>());
    // }

    // private void OnEnable()
    // {
    //     onFoot.Enable();
    // }

    // private void OnDisable()
    // {
    //     onFoot.Disable();
    // }

    // public PlayerInput.OnFootActions OnFoot => onFoot; // Expose OnFoot actions
}
