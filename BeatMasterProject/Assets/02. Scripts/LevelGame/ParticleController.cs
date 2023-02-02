using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleController : MonoBehaviour
{
    [SerializeField] private ParticleSystem _movementParticle;
    [SerializeField] private ParticleSystem _fallParticle;
    [SerializeField] private ParticleSystem _touchParticle;
    [SerializeField] private ParticleSystem _jumpParticle;
    [SerializeField] private ParticleSystem _hammerParticle;
    
    [SerializeField] private Animator _animator;

    [Range(0, 10)] 
    [SerializeField] private int _occurAfterVelocity;
    [Range(0, 0.2f)] 
    [SerializeField] private float _dustFormationPeriod;
    [SerializeField] private Rigidbody2D playerRb;

    private float counter;

    private bool _isOnGround = false;

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;

        if (_isOnGround && Mathf.Abs(playerRb.velocity.x) > _occurAfterVelocity)
        {
            if (counter > _dustFormationPeriod)
            {
                _movementParticle.Play();
                counter = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpParticle.Play();
            if (_isOnGround)
            {
                _animator.transform.position = transform.position - Vector3.up * (transform.localPosition.y / 2);
                _animator.SetTrigger("isJump");
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            _hammerParticle.Play();
        }
    }

    public void PlayTouchPartcle()
    {
        _touchParticle.Play();
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag == "Ground")
        {
            _fallParticle.Play();
            _isOnGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            _isOnGround = false;
            
        }
    }
}