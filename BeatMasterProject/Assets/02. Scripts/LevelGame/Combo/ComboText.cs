using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ComboText : MonoBehaviour
{
    // public bool vertexBufferAutoSizeReduction;
    
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
    
    private Text _text;
    private Rigidbody2D _rigid;
    private WaitForEndOfFrame _waitForEndOfFrame;
    private StringBuilder _comboStr;
    private ComboSystem _comboSystem;
    private Color _startColor;
    private Color _goalColor;
    private bool _isGoingDown;
    private int _hueIndex;
    private float _alpha = 1f;
    private bool _wasCreated;

    private readonly ushort MAX_HUE = 360;
    private readonly string COMBO_STR = "Combo ";
    
    private void Awake()
    {
        _text = GetComponent<Text>();
        _rigid = GetComponent<Rigidbody2D>();
        _waitForEndOfFrame = new WaitForEndOfFrame();
        _comboStr = new StringBuilder(10);
    }

    private void OnEnable()
    {
        if (_wasCreated)
        {
            Debug.Log("Shoot");
            Shoot();
        }
    }
    
    private void Start()
    {
        Debug.Log("Start");      
    }

    private void OnDisable()
    {
        // 초기화
        _wasCreated = true;
        
        _alpha = 1f;
        _text.color = Color.white;
        _startColor = _text.color;
        _startColor.a = _alpha;
        _goalColor.a = _alpha;
    }

    private void Update()
    {
        FadeIn();
    }

    public void SetText(int combo, int index, ComboSystem comboSystem)
    {
        _comboSystem = comboSystem;
        _comboStr.Append(COMBO_STR);
        _comboStr.Append(combo);
        _text.text = _comboStr.ToString();
        gameObject.SetActive(true);
        int hueIndex = Math.Clamp(index, 0, _hueValues.Length - 1);
        _hueIndex = hueIndex;
        StartCoroutine(SetTextColor());
        _comboStr.Clear();
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
            if (_hueIndex < _hueValues.Length - 1)
            {
                // hueIndex로 위에서 0 검사하므로 1부터 시작하니 -1 해줘야한다.
                h = (float)_hueValues[_hueIndex - 1] / MAX_HUE;
                _goalColor = Color.HSVToRGB(h, s, v);
                Color newColor = Color.Lerp(_startColor, _goalColor, Mathf.Cos(Mathf.PI + Time.time * _changeColorMod) / 2 + 1);
                _text.color = newColor;
            }
            else
            {
                h += Time.deltaTime;
                h = h > 1 ? h - 1 : h;
                Color tempColor = Color.HSVToRGB(h, s, v);
                _goalColor = new Color(tempColor.r, tempColor.g, tempColor.b, _goalColor.a);
                _text.color = _goalColor;
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
        _alpha -= Time.deltaTime * _fadeMod;

        if (_hueIndex == 0)
        {
            Color tempColor = _text.color;
            tempColor.a = _alpha;
            _text.color = tempColor;
        }
        else
        {
            _startColor.a = _alpha;
            _goalColor.a = _alpha;
        }
        
        if (_text.color.a <= 0)
        {
            _comboSystem.ReturnObject(gameObject);
        }
    }

}