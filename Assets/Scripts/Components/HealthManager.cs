using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthManager : MonoBehaviour
{
    [SerializeField] private float _maxHealth;
    private float _currentHealth;

    private void Awake()
    {
        _currentHealth = _maxHealth;
    }

    private void TakeDamage(float damage)
    {
        _currentHealth -= damage;
    }
}
