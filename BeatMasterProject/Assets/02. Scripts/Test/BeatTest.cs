using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;


public class BeatTest : MonoBehaviour
{
    [EventID] public string eventID;

    [SerializeField] private float _moveSpeed = 1f;
    [SerializeField] private float _jumpForce = 10f;

    private float _previousBeatTime = 0;
    [SerializeField] private bool _isGrounded;

    private Rigidbody2D _rigidbody;


    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        Jump();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        float currentBeatTime = (float)Koreographer.Instance.GetMusicBeatTime();
        float x = transform.position.x + (currentBeatTime - _previousBeatTime) * _moveSpeed;
        ;
        _previousBeatTime = currentBeatTime;

        _rigidbody.MovePosition(Vector2.right * x);
    }

    private void Jump()
    {
        
        if (Input.GetKeyDown(KeyCode.F))
        {
            Vector2 jumpVector = new Vector2(0, _jumpForce);
            _rigidbody.transform.position += (Vector3)jumpVector * Time.deltaTime;
            
            _isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.CompareTag("Ground"))
        {
            _isGrounded = true;
        }
    }
}