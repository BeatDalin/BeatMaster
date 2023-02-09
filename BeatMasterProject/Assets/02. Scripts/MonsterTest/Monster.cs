using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Monster : MonoBehaviour
{
    public bool isGainCoin;

    public Transform position;

    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private Animator _animator;
    
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.name.Equals("Ground Tilemap"))
        {
            _rigidbody2D.isKinematic = true;
        }
    }

    public void ShowAnim()
    {
        if (isGainCoin)
        {
            isGainCoin = false;
            _animator.SetBool("Hit", true);
            
            
        }
    }
}
