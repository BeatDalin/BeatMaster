using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class ComboSystem : MonoBehaviour
{
    [SerializeField] private int _increasingAmount = 10000;
    [SerializeField] private GameObject _comboTextPrefab; 
    [SerializeField] private ushort _combo;
    private int _currentAmount;
    private Transform _characterTransform;
    [Header("OutLine Effect")]
    private SpriteRenderer _characterSprite;
    private Material _defaultMat;
    [SerializeField] private Material _outLineMat;
    private ushort _showEffectNum = 10;
    [Header("OutLine Color")]
    private bool _isColorChanging;
    [ColorUsage(true,true)]
    [SerializeField] private Color[] _colorsHDR; // Colors will be set in Inspector
    private static readonly int OutLineColor = Shader.PropertyToID("_Color");
    private Coroutine _colorChangeCoroutine;
    
    // 확인용 SerializeField
    private float _timer;

    private void Awake()
    {
        _characterTransform = FindObjectOfType<CharacterMovement>().transform;
        _characterSprite = _characterTransform.GetChild(0).GetComponent<SpriteRenderer>();
        _defaultMat = _characterSprite.material;
        // Set Texture based on current character
        // _outLineMat.SetTexture("_MainTex", texture to push....);
    }

    void Start()
    {
        _currentAmount = _increasingAmount;
        // _colorChangeCoroutine = StartCoroutine(CoChangeOutLineColor());
    }

    public void IncreaseCombo()
    {
        _combo++;
        ShowCombo();
        if (_combo >= _showEffectNum)
        {
            _characterSprite.material = _outLineMat;
            if (!_isColorChanging)
            {
                StartCoroutine(CoChangeOutLineColor()); // Start coroutine only once
            }
        }
    }

    public void IncreaseComboInProcess(int startSample)
    {
        int currentSampleTime = SoundManager.instance.playingKoreo.GetLatestSampleTime();
        if (_currentAmount + startSample < currentSampleTime)
        {
            IncreaseCombo();
            _currentAmount += _increasingAmount;
        }
    }
    
    public void ResetCombo()
    {
        _combo = 0;
        ResetCurrentAmount();
        // 콤보가 끊기는 연출
        _characterSprite.material = _defaultMat;
    }

    public void ResetCurrentAmount()
    {
        _currentAmount = _increasingAmount;
    }

    private void ShowCombo()
    {
        // 콤보 UI를 보여줌
        GameObject comboGo = Instantiate(_comboTextPrefab, _characterTransform.position, Quaternion.Euler(Vector3.zero), transform);
        ComboText comboText = comboGo.GetComponent<ComboText>(); 
        comboText.SetText(_combo);
    }

    private IEnumerator CoChangeOutLineColor()
    {
        while (_combo >= _showEffectNum)
        {
            _outLineMat.SetColor(OutLineColor, _colorsHDR[Random.Range(0, _colorsHDR.Length)]);
            yield return new WaitForSeconds(1f);
        }
        _isColorChanging = false;
    }

    
    
}
