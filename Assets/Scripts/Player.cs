using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float _playerSpeed;
    private Vector2 _movement;
    private PlayerInput _playerInput;

    private void Awake()
    {
        _playerInput = new PlayerInput();

        _playerInput.Enable();
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        ReadMovement();
    }

    private void FixedUpdate()
    {
        DoMovement();
    }

    private void ReadMovement()
    {
        _movement = _playerInput.Player.Movement.ReadValue<Vector2>();
    }

    private void DoMovement()
    {
        transform.position += new Vector3(_movement.x, 0, _movement.y) * _playerSpeed * Time.fixedDeltaTime;
    }
}
