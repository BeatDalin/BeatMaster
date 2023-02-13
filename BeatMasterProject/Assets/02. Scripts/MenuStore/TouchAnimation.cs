using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchAnimation : MonoBehaviour
{
    private bool _animStart = false;
    private SpriteRenderer _spriteRenderer;

    private void Start()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPos = Camera.main.ScreenToWorldPoint(touch.position);
            Vector2 objectPos = _spriteRenderer.bounds.center;

            if (Vector2.Distance(touchPos, objectPos) <= _spriteRenderer.bounds.extents.x && _animStart == false)
            {
                _animStart = true;
                int op = UnityEngine.Random.Range(1, Enum.GetValues(typeof(CharacterStatus)).Length);
                PlayerStatus.Instance.ChangeStatus((CharacterStatus)op);
                Invoke("ResetAnimation", 3f);
            }
        }
    }

    private void ResetAnimation()
    {
        _animStart = false;
        PlayerStatus.Instance.ChangeStatus(CharacterStatus.Idle);
    }
}