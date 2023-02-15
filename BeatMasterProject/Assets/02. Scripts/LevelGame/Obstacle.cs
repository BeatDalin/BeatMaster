using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    private Animator _animator;
    private static readonly int IsObstacle = Animator.StringToHash("isObstaclePlay");
    private readonly string _defaultState = "Empty";
    private readonly string _playerTag = "Player";
    private void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    
    private void PlayAnim()
    {
        _animator.SetTrigger(IsObstacle);
    }
    
    public void ResetAnim()
    {
        _animator.ResetTrigger(IsObstacle);
        _animator.Play(_defaultState);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag(_playerTag))
        {
            PlayAnim();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag(_playerTag))
        {
            
        }
    }
}
