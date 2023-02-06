using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour
{
    private Animator _animStar;
    private Animator _animTwinkle0;
    private Animator _animTwinkle1;
    private static readonly int IsHide = Animator.StringToHash("isHide");
    private static readonly int IsPlay = Animator.StringToHash("isPlay");

    private void Awake()
    {
        _animStar = GetComponent<Animator>();
        _animTwinkle0 = transform.GetChild(0).GetComponent<Animator>();
        _animTwinkle1 = transform.GetChild(1).GetComponent<Animator>();
    }

    /// <summary>
    /// Start playing animation again after recycling this star object.
    /// </summary>
    private void PlayAnim()
    {
        _animStar.SetTrigger(IsPlay);
        _animTwinkle0.SetTrigger(IsPlay);
        _animTwinkle1.SetTrigger(IsPlay);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("Player"))
        {
            _animStar.SetTrigger(IsHide);
            _animTwinkle0.SetTrigger(IsHide);
            _animTwinkle1.SetTrigger(IsHide);
        }
    }
}
