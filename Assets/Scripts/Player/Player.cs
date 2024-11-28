using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    [SerializeField] private float _playerSpeed;
    [SerializeField] private float _damageMult;
    [SerializeField] private Camera _mainCam;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _turnAroundSpeed;
    [SerializeField] private float _aimingSpeed;
    private Vector2 _movement;
    private PlayerInput _playerInput;
    private bool _isSucking;
    private RaycastHit _slopeHit;
    private List<TestEnemy> _suckingEnemiesList = new List<TestEnemy>();

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

        if (_playerInput.Player.Fire.ReadValue<float>() > 0 && !_isSucking)
        {
            StartSucking();
        }
        else if (_playerInput.Player.Fire.ReadValue<float>() > 0 && _isSucking)
        {
            foreach (var enemy in _suckingEnemiesList)
            {
                TryToDamage(enemy);
            }
        }
        else if (_playerInput.Player.Fire.ReadValue<float>() == 0 && _isSucking)
        {
            StopSucking();
        }
    }

    private void FixedUpdate()
    {
        LookTowardsMouse();
        DoMovement();
    }

    private void StartSucking()
    {
        _isSucking = true;
    }

    private void StopSucking()
    {
        foreach (var enemy in _suckingEnemiesList)
        {
            enemy.BeingSucked = false;
        }
        _isSucking = false;
    }

    private void ReadMovement()
    {
        _movement = _playerInput.Player.Movement.ReadValue<Vector2>();
    }

    private void DoMovement()
    {
        var newDir = new Vector3(_movement.x, 0, _movement.y);
        newDir = AdjustVelocityToSlope(newDir);
        _rb.MovePosition(transform.position + _playerSpeed * Time.fixedDeltaTime * newDir.normalized);
    }

    private void LookTowardsMouse()
    {
        if (_playerInput.Player.Fire.ReadValue<float>() == 0)//Only look at point when we are trying to use vacuum sucker
        {
            LookTowardsMovement();
            return;
        }

        Vector3 lookPos = ReadMousePosition();
        lookPos.z = transform.position.z - _mainCam.transform.position.z;
        lookPos.z += .8f;//magic number honestly i have no idea
        Vector3 lookAtPoint = _mainCam.ScreenToWorldPoint(lookPos);
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
        enemy.BeingSucked = true;
        var distance = Vector3.Distance(new Vector3(_movement.x, 0, _movement.y), enemy.transform.forward);
        if (distance > 1)
        {
            enemy.HpManager.TakeDamage(Time.deltaTime * distance * _damageMult);
        }
        enemy.Rb.AddForce(-enemy.transform.forward * ((1000 + distance * 800) * Time.deltaTime), ForceMode.Force);
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
            var enemy = collider.GetComponent<TestEnemy>();
            enemy.BeingSucked = true;
            if (!_suckingEnemiesList.Contains(enemy))
            {
                _suckingEnemiesList.Add(enemy);
            }
        }
        else if (collider.CompareTag("Enemy") && !_isSucking)
        {
            collider.GetComponent<TestEnemy>().BeingSucked = false;
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        /*if (collider.CompareTag("Enemy") && collider.GetComponent<TestEnemy>().BeingSucked)
        {
            collider.GetComponent<TestEnemy>().BeingSucked = false;
            collider.transform.GetComponent<Rigidbody>().AddForce(Vector3.down * 25, ForceMode.Impulse);
        }*/
    }
}
