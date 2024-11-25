using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float _playerSpeed;
    [SerializeField] private Camera _mainCam;
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
        ReadMousePosition();
        ReadMovement();
    }

    private void FixedUpdate()
    {
        LookTowardsMouse();
        DoMovement();
    }

    private void ReadMovement()
    {
        _movement = _playerInput.Player.Movement.ReadValue<Vector2>();
    }

    private void DoMovement()
    {
        transform.position += _playerSpeed * Time.fixedDeltaTime * new Vector3(_movement.x, 0, _movement.y);
    }

    private void LookTowardsMouse()
    {
        if (_playerInput.Player.Fire.ReadValue<float>() == 0)//Only look at point when we are trying to use vacuum sucker
        {
            transform.rotation = Quaternion.identity;
            return;
        }

        Ray ray = _mainCam.ScreenPointToRay(ReadMousePosition());
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        Vector3 lookAtPoint = hit.point;
        lookAtPoint.y = transform.position.y;
        transform.LookAt(lookAtPoint); 
    }

    private Vector2 ReadMousePosition()
    {
        return _playerInput.Player.Mouse.ReadValue<Vector2>();
    }
}
