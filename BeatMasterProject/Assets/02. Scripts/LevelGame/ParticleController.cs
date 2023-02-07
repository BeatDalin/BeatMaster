using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class ParticleController : MonoBehaviour
{
    private CharacterMovement _characterMovement; 
    [SerializeField] private ParticleSystem _movementParticle;
    [SerializeField] private ParticleSystem _fallParticle;
    [SerializeField] private ParticleSystem _touchParticle;
    [SerializeField] private ParticleSystem _jumpParticle;
    [SerializeField] private ParticleSystem _attackParticle;
    
    [SerializeField] private Animator _animator;

    [Range(0, 10)] 
    [SerializeField] private int _occurAfterVelocity;
    [Range(0, 0.2f)] 
    [SerializeField] private float _dustFormationPeriod;
    [SerializeField] private Rigidbody2D playerRb;

    private float counter;

    public bool isOnGround = false;
    private static readonly int IsJump = Animator.StringToHash("isJump");

    private void Awake()
    {
        _characterMovement = GetComponent<CharacterMovement>();
        
    }

    // Update is called once per frame
    void Update()
    {
        counter += Time.deltaTime;

        if (isOnGround && Mathf.Abs(playerRb.velocity.x) > _occurAfterVelocity)
        {
            if (counter > _dustFormationPeriod)
            {
                _movementParticle.Play();
                counter = 0;
            }
        }

        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            _jumpParticle.Play();
            if (isOnGround)
            {
                // _animator.transform.position = transform.position - Vector3.up * (transform.localPosition.y / 2);
                _animator.SetTrigger(IsJump);
            }
        }

        if (Input.GetKeyDown(KeyCode.F))
        {
            _attackParticle.Play();
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
            isOnGround = true;
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Ground"))
        {
            isOnGround = false;
        }
    }
}