using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CalebUtils;

public class TestEnemy : MonoBehaviour
{
    public bool BeingSucked;
    public HealthManager HpManager;
    public Rigidbody Rb;
    [SerializeField] private Player _player;
    [SerializeField] private float _detectionRadius;
    [SerializeField] private float _speed;
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
                RunFromPlayer();
            }
        }
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
        var targetPos = CalebUtility.MoveAway(transform.position, _player.transform.position, _speed * Time.deltaTime);
        transform.LookAt(targetPos);
        transform.position = targetPos;
    }

    private void Die(object sender, System.EventArgs e)
    {
        _player.SuckingEnemiesList.Remove(this);
        Destroy(gameObject);
    }
}
