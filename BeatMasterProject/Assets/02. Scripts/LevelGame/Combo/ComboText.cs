using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class ComboText : MonoBehaviour
{
    [SerializeField] private float _minDistance = 5f;
    [SerializeField] private float _maxDistance = 10f;
    [SerializeField] private float _minShootMod = 1f;
    [SerializeField] private float _maxShootMod = 2f;
    [SerializeField] private float _fadeMod = 0.5f;
    [SerializeField] private int _minAngle = 15;
    [SerializeField] private int _maxAngle = 166;
    [SerializeField] private float _playerSpeed;
    [SerializeField] private float _defaultSpeed;
    [SerializeField] private float _changeColorMod = 2f;
    [SerializeField] private int[] _hueValues;
    
    private TextMeshPro _text;
    private Rigidbody2D _rigid;
    private WaitForEndOfFrame _waitForEndOfFrame;
    private bool _isGoingDown;
    private Color _startColor;
    private Color _goalColor;
    private int _hueIndex;

    private readonly ushort MAX_HUE = 360;
    
    private void Awake()
    {
        _text = GetComponent<TextMeshPro>();
        _rigid = GetComponent<Rigidbody2D>();
        _startColor = _text.faceColor;
        _waitForEndOfFrame = new WaitForEndOfFrame();
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

    public void SetText(int combo, int index)
    {
        _text.text = $"Combo {combo}";
        _hueIndex = index; 
        int hueIndex = Math.Clamp(index, 0, _hueValues.Length - 1);
        StartCoroutine(SetTextColor());
    }

    private IEnumerator SetTextColor()
    {
        if (_hueIndex == 0)
        {
            yield break;
        }
        
        float h, s, v;
        Color.RGBToHSV(_startColor, out h, out s, out v);
        s = 1;

        while (true)
        {
            if (_hueIndex < 3)
            {
                // hueIndex로 위에서 0 검사하므로 1부터 시작하니 -1 해줘야한다.
                h = (float)_hueValues[_hueIndex - 1] / MAX_HUE;
                _goalColor = Color.HSVToRGB(h, s, v);
                Color newColor = Color.Lerp(_startColor, _goalColor, Mathf.Cos(Mathf.PI + Time.time * _changeColorMod) / 2 + 1);
                _text.faceColor = newColor;
            }
            else
            {
                _startColor = _text.faceColor;
                h += Time.deltaTime;
                h = h > 1 ? h - 1 : h;
                Color newColor = Color.HSVToRGB(h, s, v);
                _text.faceColor = newColor;
            }
            yield return _waitForEndOfFrame;
        }

    }
    
    private void Shoot()
    {
        // x가 cos y가 sin
        int angle = Random.Range(_minAngle, _maxAngle);
        float distance = Random.Range(_minDistance, _maxDistance);
        float shootMod = Random.Range(_minShootMod, _maxShootMod);
        distance *= _playerSpeed / _defaultSpeed * shootMod;
        Vector2 direction = distance * new Vector2(MathF.Cos(angle * Mathf.Deg2Rad), MathF.Sin(angle * Mathf.Deg2Rad));
        _rigid.velocity = direction;
    }

    public void SetPlayerSpeed(float speed, float defaultSpeed)
    {
        _playerSpeed = speed;
        _defaultSpeed = defaultSpeed;
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