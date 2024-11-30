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

    void Start()
    {
        HpManager.OnDeath += Die;
    }

    void Update()
    {
        FindPlayer();

        if (_playerFound)
        {
            if (!BeingSucked)
            {
                ChasePlayer();
            }
            else
            {
                Squirm();
                RunFromPlayer();
            }
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
        var targetPos = transform.position + (transform.forward * _speed * Time.deltaTime);
        transform.position = targetPos;
    }

    private void Squirm()
    {
        Debug.Log(transform.rotation);
        if (transform.rotation == _targetRot)
        {
            _targetRot = Quaternion.AngleAxis(Random.Range(0, 360), Vector3.up) * transform.rotation;
        }
        else
        {
            transform.rotation = Quaternion.Lerp(transform.rotation, _targetRot, Time.deltaTime);
        }
    }


    private void Die(object sender, System.EventArgs e)
    {
        _player.SuckingEnemiesList.Remove(this);
        Destroy(gameObject);
    }
}
