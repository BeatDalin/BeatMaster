using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BossController : MonoBehaviour
{
    private BossState _myState;
    public BossState MyState
    {
        get { return _myState; }
        set
        {
            _myState = value;
            
            _animator.Play(_myState.ToString(), -1, 0f);
            
            switch (_myState)
            {
                case BossState.Idle:
                    OnIdle();
                    break;
                case BossState.Attack:
                    OnAttack();
                    break;
                case BossState.Laugh:
                    OnLaugh();
                    break;
                case BossState.Destroyed:
                    OnDestroyed();
                    break;
            }
        } 
    }
    [SerializeField] private GameObject _fireBallPrefab;
    private Animator _animator;

    public enum BossState
    {
        Idle,
        Attack,
        Laugh,
        Destroyed,
    }
    
    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _animator = GetComponentInChildren<Animator>();
        MyState = BossState.Idle;
    }

    private void Update()
    {
        TestInput();
    }

    

    private void OnIdle()
    {
        
    }

    private void OnAttack()
    {
        
    }
    
    private void OnLaugh()
    {
    }
    
    private void OnDestroyed()
    {
        
    }
    
    private void TestInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            MyState = BossState.Attack;
        }
    }

    
}
