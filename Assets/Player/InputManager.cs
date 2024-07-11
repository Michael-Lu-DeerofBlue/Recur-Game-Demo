using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerInput;
    private PlayerInput.OnFootActions onFoot;

    private PlayerController playerController;

    void Awake()
    {
        playerInput = new PlayerInput();
        onFoot = playerInput.OnFoot;
        playerController = GetComponent<PlayerController>();

        //onFoot.Jump.performed += ctx => playerController.Jump();
        //onFoot.Crouch.performed += ctx => playerController.Crouch(); // Update your PlayerController script to handle Crouch if needed
    }

    void Update()
    {
        //playerController.ProcessLook(onFoot.Look.ReadValue<Vector2>());
        //playerController.JudgeSprint();
    }
    void FixedUpdate()
    {
        //playerController.ProcessMove(onFoot.Movement.ReadValue<Vector2>());//Move();
    }

    void LateUpdate()
    {

    }

    private void OnEnable()
    {
        onFoot.Enable();
    }

    private void OnDisable()
    {
        onFoot.Disable();
    }

    public PlayerInput.OnFootActions OnFoot => onFoot; // Expose OnFoot actions
}
