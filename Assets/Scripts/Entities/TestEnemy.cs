using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CalebUtils;

public class TestEnemy : MonoBehaviour
{
    [SerializeField] private HealthManager _hpManager;
    [SerializeField] private Player _player;
    [SerializeField] private float _detectionRadius;
    private bool _playerFound;
    private bool _beingSucked;

    void Start()
    {
        
    }

    void Update()
    {
        FindPlayer();

        if (_playerFound)
        {
            if (!_beingSucked)
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
        transform.position = Vector3.MoveTowards(transform.position, _player.transform.position, Time.deltaTime);
    }

    private void RunFromPlayer()
    {
        transform.position.MoveAway(_player.transform.position, Time.deltaTime);
    }
}
