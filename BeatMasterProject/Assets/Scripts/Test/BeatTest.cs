using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;


public class BeatTest : MonoBehaviour
{
    [EventID] 
    public string eventID;

    public float moveSpeed = 4f;
    public float jumpSpeed = 1f;

    private Rigidbody2D _rigidbody;
    
    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody2D>();
        Koreographer.Instance.RegisterForEvents(eventID, Beat);
    }
    
    private void Beat(KoreographyEvent evt)
    {
        Vector2 vel = _rigidbody.velocity;
        vel.y = jumpSpeed;

        _rigidbody.velocity = vel;
    }

    private void FixedUpdate()
    {
        _rigidbody.velocity = new Vector2(moveSpeed, _rigidbody.velocity.y);
    }
}
