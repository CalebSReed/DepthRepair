using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CalebUtils;

public class TestEnemy : MonoBehaviour
{
    public bool BeingSucked { get; private set; }
    public HealthManager HpManager;
    public Rigidbody Rb;
    [SerializeField] private Player _player;
    [SerializeField] private float _detectionRadius;
    [SerializeField] private float _speed;
    private Quaternion _targetRot;
    private bool _playerFound;
    private float _timeToFlipRotation = .5f;
    private bool _mirrorSquirmRotation;

    void Start()
    {
        HpManager.OnDeath += Die;
    }

    void Update()
    {
        FindPlayer();

        if (BeingSucked)
        {
            Squirm();
            RunFromPlayer();
        }
        if (_playerFound && !BeingSucked)
        {
            ChasePlayer();
        }
    }

    public void GetSucked()
    {
        BeingSucked = true;
        _targetRot = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * transform.rotation;
    }

    public void GetUnsucked()
    {
        BeingSucked = false;
    }

    private void FindPlayer()
    {
        if (Vector3.Distance(transform.position, _player.transform.position) < _detectionRadius)
        {
            _playerFound = true;
        }
        else
        {
            _playerFound = false;
        }
    }

    private void ChasePlayer()
    {
        var targetPos = Vector3.MoveTowards(transform.position, _player.transform.position, _speed * Time.deltaTime);
        transform.LookAt(targetPos);
        transform.position = targetPos;
    }

    private void RunFromPlayer()
    {
        var targetPos = transform.position + (transform.forward * _speed * 4 * Time.deltaTime);
        transform.position = targetPos;
        //Debug.Log(targetPos);
    }

    private void Squirm()
    {
        if (_mirrorSquirmRotation)
        {
            transform.Rotate(Vector3.up, 1f);
        }
        else
        {
            transform.Rotate(Vector3.up, -1f);
        }

        _timeToFlipRotation -= Time.deltaTime;

        if (_timeToFlipRotation < 0f)
        {
            _timeToFlipRotation = Random.Range(.5f, 1f);
            _mirrorSquirmRotation = !_mirrorSquirmRotation;
        }
    }


    private void Die(object sender, System.EventArgs e)
    {
        _player.SuckingEnemiesList.Remove(this);
        Destroy(gameObject);
    }
}
