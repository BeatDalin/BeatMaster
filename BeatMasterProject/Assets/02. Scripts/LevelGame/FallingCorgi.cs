using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FallingCorgi : MonoBehaviour
{
    [SerializeField] private float _minSpeed = 3f;
    [SerializeField] private float _maxSpeed = 5f;

    private Camera _defaultCam;
    private float _speed;

    private void Awake()
    {
        _defaultCam = GameObject.FindWithTag("Main").GetComponent<Camera>();
        _speed = Random.Range(_minSpeed, _maxSpeed);
    }

    void Update()
    {
        Vector3 pos = _defaultCam.WorldToViewportPoint(transform.position);
        pos.z = 0f;
        if (pos.x < -1f || pos.y < -1f)
        {
            Destroy(gameObject);
        }
        
        transform.Translate(transform.right * (-1f * (Time.deltaTime * _speed)));        
    }
}
