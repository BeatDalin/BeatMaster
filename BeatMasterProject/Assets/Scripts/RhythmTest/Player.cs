using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using SonicBloom.Koreo;
public class Player : MonoBehaviour
{
    [EventID] public string eventID;

    [SerializeField]
    private float moveSpeed;

    private bool _isJumping;

    private Animator _animator;
    private Rigidbody2D _rigidbody;


    // Start is called before the first frame update
    private void Start()
    {
        _animator = GetComponent<Animator>();
        _rigidbody = GetComponent<Rigidbody2D>();

        moveSpeed = (float)Koreographer.Instance.GetMusicBPM() / 60;

    }

    // Update is called once per frame
    private void Update()
    {
    }

    private void FixedUpdate()
    {
        if (AudioTest.instance.isPlaying)
        {
            Move();
        }
    }

    private void Move()
    {
        transform.Translate(new Vector3(moveSpeed * Time.deltaTime,0,0));
    }
    private void Attack()
    {
        
    }

    private void Jump()
    {
        
    }
}