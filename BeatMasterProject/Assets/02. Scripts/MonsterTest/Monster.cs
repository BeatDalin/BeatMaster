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
    [SerializeField] private CapsuleCollider2D _capsuleCollider2D;

    private void Update()
    {
        if (_rigidbody2D.velocity.y == 0)
        {
            _capsuleCollider2D.isTrigger = true;
        }
    }
}
