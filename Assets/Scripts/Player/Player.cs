using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float _playerSpeed;
    [SerializeField] private Camera _mainCam;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _turnAroundSpeed;
    [SerializeField] private float _aimingSpeed;
    private Vector2 _movement;
    private PlayerInput _playerInput;
    private bool _isSucking;
    private RaycastHit _slopeHit;

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
        LookTowardsMouse();
        DoMovement();
        if (_playerInput.Player.Fire.ReadValue<float>() > 0)
        {
            Suck();
        }
    }

    private void Suck()
    {
        _isSucking = true;
    }

    private void ReadMovement()
    {
        _movement = _playerInput.Player.Movement.ReadValue<Vector2>();
    }

    private void DoMovement()
    {
        var newDir = new Vector3(_movement.x, 0, _movement.y);
        newDir = AdjustVelocityToSlope(newDir);
        _rb.MovePosition(transform.position + _playerSpeed * Time.fixedDeltaTime * newDir);
    }

    private void LookTowardsMouse()
    {
        if (_playerInput.Player.Fire.ReadValue<float>() == 0)//Only look at point when we are trying to use vacuum sucker
        {
            LookTowardsMovement();
            return;
        }

        Ray ray = _mainCam.ScreenPointToRay(ReadMousePosition());
        RaycastHit hit;
        Physics.Raycast(ray, out hit);
        Vector3 lookAtPoint = hit.point;
        lookAtPoint.y = transform.position.y;
        lookAtPoint -= transform.position;
        lookAtPoint.Normalize();
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(lookAtPoint), _aimingSpeed);
    }

    private Vector2 ReadMousePosition()
    {
        return _playerInput.Player.Mouse.ReadValue<Vector2>();
    }

    private void LookTowardsMovement()
    {
        if (_movement.x != 0 || _movement.y != 0)
        {
            Vector3 lookAtRotation = new Vector3(_movement.x, 0, _movement.y);
            Quaternion newRot = Quaternion.LookRotation(lookAtRotation, Vector3.up);
            transform.rotation = Quaternion.Lerp(transform.rotation, newRot, _turnAroundSpeed);
        }
    }

    private void TryToDamage(TestEnemy enemy)
    {

    }

    private Vector3 AdjustVelocityToSlope(Vector3 velocity)
    {
        var ray = new Ray(transform.position, Vector3.down);

        if (Physics.Raycast(ray, out _slopeHit, 2f))
        {
            var slopeRotation = Quaternion.FromToRotation(Vector3.up, _slopeHit.normal);
            var adjustedVelocity = slopeRotation * velocity;

            if (adjustedVelocity.y < 0)
            {
                return adjustedVelocity;
            }
        }

        return velocity;
    }

    private void OnTriggerStay(Collider collider)
    {
        if (collider.CompareTag("Enemy") && _isSucking)
        {
            TryToDamage(collider.GetComponent<TestEnemy>());
        }
    }
}
