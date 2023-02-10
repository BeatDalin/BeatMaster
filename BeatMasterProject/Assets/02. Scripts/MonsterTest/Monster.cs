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
    [SerializeField] private Animation _anim;

    private SpriteRenderer _spriteRenderer;
    private GameState _curState;

    private void Start()
    {
        _curState = GameState.Idle;
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

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

            //StartCoroutine(CoWaitAnimation());
        }
        else
        {
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0);
            //gameObject.SetActive(false);
        }
    }

    public void DisableMonster()
    {
        _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0);
    }

    public void ChangeAlpha(bool up)
    {
        if (up)
        {
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 1);
        }
        else
        {
            _spriteRenderer.color = new Color(_spriteRenderer.color.r, _spriteRenderer.color.g, _spriteRenderer.color.b, 0);
        }
    }

    IEnumerator CoWaitAnimation()
    {
        ChangeAlpha(false);
        _animator.SetBool("Hit", false);
        while (!_animator.GetCurrentAnimatorStateInfo(0).IsName("Coin"))
        {
            Debug.Log("기다리는중");
            yield return null;
        }
        
        Destroy(transform.GetChild(0).gameObject);
    }
}
