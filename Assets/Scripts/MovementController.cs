using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public enum MovingState
{
    Idle,
    Walking,
    Running
}

[RequireComponent(typeof(CharacterController), typeof(Animator))]
public class MovementController : MonoBehaviour
{
    private CharacterController _characterController;
    private Animator _animator;
    
    [SerializeField]
    private float walkSpeed;
    [SerializeField]
    private float runSpeed;
    [SerializeField] 
    private float turnSmoothTime;
    [SerializeField] 
    private float turnSmoothVelocity;

    [SerializeField]
    private float _currentMoveSpeed;
    private bool _isMovementPressed;
    private bool _isRunPressed;
    private Vector2 _currentMovementInput;
    private Vector3 _currentMovement;
    private MovingState _movingState;

    private delegate void OnMovement();

    private event OnMovement OnMove;
    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _animator = GetComponent<Animator>();
        OnMove += SetSpeed;
        OnMove += SmoothLook;
    }


    public void OnWalk(Vector2 inputDirection)
    {
        _currentMovementInput = inputDirection;
        _currentMovement = new Vector3(_currentMovementInput.x, 0, _currentMovementInput.y); 
        _isMovementPressed = _currentMovementInput.magnitude > 0.01f;
        OnMove?.Invoke();
    }

    public void OnRun(bool isRunPressed)
    {
        _isRunPressed = isRunPressed;
        OnMove?.Invoke();
    }

    private void SmoothLook()
    {
        var targetAngle = Mathf.Atan2(_currentMovement.x, _currentMovement.z) * Mathf.Rad2Deg;
        var angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity,
            turnSmoothTime);
        //transform.rotation = Quaternion.Euler(0, targetAngle, 0);
        gameObject.transform.LookAt(_currentMovement + gameObject.transform.position);
    }

    private void SetSpeed()
    {
        if (_isMovementPressed)
        {
            _currentMoveSpeed = _isRunPressed ? runSpeed : walkSpeed;
        }
        else
        {
            _currentMoveSpeed = 0;
        }

        _animator.SetFloat("Speed", _currentMoveSpeed);
    }

    private void MoveToGround()
    {
        if (!_characterController.isGrounded)
        {
            RaycastHit hit;
            if (Physics.Raycast(transform.localPosition, Vector3.down, out hit))
            {
                _characterController.Move(hit.point - transform.position);
            }
        }
    }

    private void FixedUpdate()
    {
        MoveToGround();
    }

    private void Update()
    {

        _characterController.Move(_currentMoveSpeed * Time.deltaTime * _currentMovement);
    }
}
