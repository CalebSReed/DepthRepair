using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBehavior : MonoBehaviour
{
    [SerializeField] private Vector3 _cameraOffset;
    [SerializeField] private Transform _playerTransform;
    [SerializeField] private float _smoothTime;
    private Vector3 _velocity = Vector3.zero;

    void FixedUpdate()
    {
        Vector3 targetPosition = _playerTransform.position + _cameraOffset;
        transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref _velocity, _smoothTime);
    }
}
