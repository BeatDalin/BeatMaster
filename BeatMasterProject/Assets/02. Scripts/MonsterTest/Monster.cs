using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class Monster : MonoBehaviour
{
    [Header("Variable")]
    public bool isGainCoin;

    [Header("Transform")]
    [SerializeField] private Vector3 _originalPos;

    [Header("Physics")]
    [SerializeField] private Rigidbody2D _rigidbody2D;
    
    [Header("Animation")]
    [SerializeField] private Animator _animator;
    [SerializeField] private DOTweenAnimation _doTweenAnimation;

    private SpriteRenderer _spriteRenderer;
    private GameState _curState;
    private Coin _coin;

    private void Start()
    {
        _curState = GameState.Idle;
        _coin = transform.GetChild(0).GetComponent<Coin>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.collider.name.Equals("Ground Tilemap"))
        {
            _rigidbody2D.isKinematic = true;
            _originalPos = transform.localPosition;
        }
    }

    public void ShowAnim(Vector2 movePos, GameObject coinParent)
    {
        if (isGainCoin)
        {
            isGainCoin = false;
            
            _doTweenAnimation.transform.DOLocalMove(new Vector3(transform.localPosition.x + 3f,transform.localPosition.y + 3f,0), 0.1f).onPlay += () =>
            {
                _doTweenAnimation.transform.DOLocalRotate(new Vector3(180f, 0f, 0f), 0.3f).onPlay += () =>
                {
                    _spriteRenderer.DOFade(0f, 0.3f).onComplete += () =>
                    {
                        _doTweenAnimation.DORewind();
                        _coin.MoveCoin(movePos, coinParent);
                        transform.position = _originalPos;
                        transform.rotation = Quaternion.identity;
                    };
                };
            };
        }
        else
        {
            _doTweenAnimation.transform.DOLocalMove(new Vector3(transform.localPosition.x + 3f,transform.localPosition.y + 3f,0), 0.1f).onPlay += () =>
            {
                _doTweenAnimation.transform.DOLocalRotate(new Vector3(180f, 0f, 0f), 0.3f).onPlay += () =>
                {
                    _spriteRenderer.DOFade(0f, 0.3f).onComplete += () =>
                    {
                        _doTweenAnimation.DORewind();
                        transform.position = _originalPos;
                        transform.rotation = Quaternion.identity;
                    };
                };
            };
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
}
