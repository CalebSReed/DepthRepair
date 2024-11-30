using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public List<TestEnemy> SuckingEnemiesList = new List<TestEnemy>();
    [SerializeField] private float _playerSpeed;
    [SerializeField] private float _damageMult;
    [SerializeField] private Camera _mainCam;
    [SerializeField] private Rigidbody _rb;
    [SerializeField] private float _turnAroundSpeed;
    [SerializeField] private float _aimingSpeed;
    private HealthManager _hpManager;
    private Vector2 _movement;
    private PlayerInput _playerInput;
    private bool _isSucking;
    [SerializeField] private float _maxSuctionGrip;
    [SerializeField] private float _suctionGrip;
    private bool _canSuck = true;
    private RaycastHit _slopeHit;
    private bool _isPlayerTurn = true;
    private float _turnDuration;

    private void Awake()
    {
        _playerInput = new PlayerInput();

        _playerInput.Enable();

        _hpManager = GetComponent<HealthManager>();
        _hpManager.OnDeath += Die;
        _suctionGrip = _maxSuctionGrip;
        _turnDuration = 1f;
    }

    private void Start()
    {
        
    }

    private void Update()
    {
        ReadMovement();

        if (_playerInput.Player.Fire.ReadValue<float>() > 0 && !_isSucking && _canSuck)
        {
            StartSucking();
        }
        else if (_playerInput.Player.Fire.ReadValue<float>() > 0 && _isSucking)
        {
            ManageCurrentTurn();
            if (_suctionGrip > 0)
            {
                foreach (var enemy in SuckingEnemiesList)
                {
                    TryToDamage(enemy);
                }
            }
            else
            {
                StopSucking();
                StartCoroutine(LoseSuctionGripTimer());
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

    private IEnumerator LoseSuctionGripTimer()
    {
        _canSuck = false;
        yield return new WaitForSeconds(1);
        _canSuck = true;
    }

    private void StartSucking()
    {
        _isSucking = true;
    }

    private void StopSucking()
    {
        foreach (var enemy in SuckingEnemiesList)
        {
            enemy.GetUnsucked();
        }
        _suctionGrip = _maxSuctionGrip;
        _isSucking = false;
        _isPlayerTurn = true;
        _turnDuration = 1f;
    }

    private void ManageCurrentTurn()
    {
        _turnDuration -= Time.deltaTime;
        if (_turnDuration < 0)
        {
            _turnDuration += 1f;
            _isPlayerTurn = !_isPlayerTurn;
        }
    }

    private void ReadMovement()
    {
        _movement = _playerInput.Player.Movement.ReadValue<Vector2>();
    }

    private void DoMovement()
    {
        if (_canSuck)//placeholder bool - Don't move as a punishment if u lost suction grip
        {
            var newDir = new Vector3(_movement.x, 0, _movement.y);
            newDir = AdjustVelocityToSlope(newDir);
            _rb.MovePosition(transform.position + _playerSpeed * Time.fixedDeltaTime * newDir.normalized);
        }
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
        lookPos.z += .8f;//magic number honestly i have no idea EDIT: IT DOESNT EVEN MATTER!!!!
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
        enemy.GetSucked();
        var distance = Vector3.Distance(new Vector3(_movement.x, 0, _movement.y), (enemy.transform.position - transform.position).normalized);
        if (distance > 1.25f)//distance will be a max of 2 since we're working with normalized vectors. 2 is best sucking, 0 is worst. 1 is if you're not holding any buttons.
        {
            enemy.HpManager.TakeDamage(Time.deltaTime * distance * _damageMult);
            if (_suctionGrip < _maxSuctionGrip)
            {
                _suctionGrip += Time.deltaTime * 2;
            }
        }
        else if (distance > .5f)
        {
            _suctionGrip -= Time.deltaTime * 5 * (1/distance);//distance range here is 1.25 - .5f. So we get a damage per second range of 4-10
            if (_suctionGrip < _maxSuctionGrip / 2)
            {
                _hpManager.TakeDamage(Time.deltaTime * 5 * (1/distance));
            }
        }
        else if (distance != 0f)
        {
            _suctionGrip -= Time.deltaTime * 5 * Mathf.Min(1/distance, 10);//from .5f to 0 distance our damage per second range is 10-50. 2 seconds to die with worst possible sucking
            _hpManager.TakeDamage(Time.deltaTime * 5 * Mathf.Min(1/distance, 10));

        }
        else//if distance is 0. Don't get a divide by 0 error please.
        {
            Debug.Log("Must be rare to get a distance of 0!! Congrats?????");
            _suctionGrip -= Time.deltaTime * 5 * 10;
            _hpManager.TakeDamage(Time.deltaTime * 20);
        }

        if (_isPlayerTurn)
        {
            enemy.transform.position = ForceMaximumDistanceFromPoint(transform.position, enemy.transform.position, 10f);
            enemy.Rb.AddForce((transform.position - enemy.transform.position) * ((800 + distance * 800) * Time.deltaTime), ForceMode.Force);
        }
        else
        {
            transform.position = ForceMaximumDistanceFromPoint(enemy.transform.position, transform.position, 10f);
            //_rb.AddForce((enemy.transform.position - transform.position) * ((1000 + distance * 800) * Time.deltaTime), ForceMode.Force);
        }
    }

    private Vector3 ForceMaximumDistanceFromPoint(Vector3 origin, Vector3 target, float maxDistance)
    {
        Vector3 newPos;
        float distance = Vector2.Distance(new Vector2(origin.x, origin.z), new Vector2(target.x, target.z));

        newPos.x = origin.x + maxDistance / distance * (target.x - origin.x);// 0,0 0,1 -> 0 + 10 / 1 * (0 + 1) = 0, 10
        newPos.y = target.y;
        newPos.z = origin.z + maxDistance / distance * (target.z - origin.z);
        Debug.Log($"enemy pos is {target}");
        Debug.Log($"Newpos is {newPos}");
        Debug.Log($"Distance between origin and newpos is {Vector3.Distance(origin, newPos)}");
        if (Vector3.Distance(origin, target) > maxDistance)
        {
            //newPos.x *= -1f;
            //newPos.z *= -1f;
            Debug.Log($"Teleporting to : {newPos}");
            return newPos;
        }
        else
        {
            return target;
        }
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
            enemy.GetSucked();
            if (!SuckingEnemiesList.Contains(enemy))
            {
                SuckingEnemiesList.Add(enemy);
            }
        }
        else if (collider.CompareTag("Enemy") && !_isSucking)
        {
            collider.GetComponent<TestEnemy>().GetUnsucked();
        }
    }

    private void Die(object sender, System.EventArgs e)
    {
        Destroy(gameObject);
    }
}
