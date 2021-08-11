using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(MovementController))]
public class PlayerInputController : MonoBehaviour
{
    private MovementController _movementController;
    private Action<Vector2> _onWalkAction;
    private Action<bool> _onRunAction;
    
    public void OnWalk(InputAction.CallbackContext context)
    {
        var currentMovementInput = context.ReadValue<Vector2>();
        _onWalkAction.Invoke(currentMovementInput);
    }

    public void OnRun(InputAction.CallbackContext context)
    {
        var isRunPressed = context.ReadValueAsButton();
        _onRunAction.Invoke(isRunPressed);
    }

    private void Awake()
    {
        _movementController = GetComponent<MovementController>();
        _onWalkAction = _movementController.OnWalk;
        _onRunAction = _movementController.OnRun;
    }
}
