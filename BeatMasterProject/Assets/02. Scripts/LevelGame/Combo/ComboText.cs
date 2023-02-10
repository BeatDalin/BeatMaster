using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ComboText : MonoBehaviour
{
    [SerializeField] private float _distance = 5f;
    [SerializeField] private float _fadeMod = 0.5f;
    [SerializeField] private int _minAngle = 60;
    [SerializeField] private int _maxAngle = 131;
    
    private TextMeshPro _text;
    private bool _isGoingDown;
    private Rigidbody2D _rigid;

    private void Awake()
    {
        _text = GetComponent<TextMeshPro>();
        _rigid = GetComponent<Rigidbody2D>();
    }

    private void Start()
    {
        Shoot();
    }

    private void Update()
    {
        _isGoingDown = _rigid.velocity.y < 0;
        if (!_isGoingDown)
        {
            return;
        }

        FadeIn();
    }
    
    public void SetText(int combo)
    {
        _text.text = $"Combo {combo}";
    }
    
    private void Shoot()
    {
        // x가 cos y가 sin
        int angle = Random.Range(_minAngle, _maxAngle);
        Vector2 direction = _distance * new Vector2(MathF.Cos(angle * Mathf.Deg2Rad), MathF.Sin(angle * Mathf.Deg2Rad));
        _rigid.velocity = direction;
    }


    private void FadeIn()
    {
        _text.alpha -= Time.deltaTime * _fadeMod;
        if (_text.alpha <= 0)
        {
            Destroy(gameObject);
        }
    }
}
