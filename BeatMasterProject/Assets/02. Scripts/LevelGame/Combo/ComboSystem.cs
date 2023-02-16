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
    private Transform _characterTransform;
    [Header("OutLine Effect")]
    [SerializeField] private Material _outLineMat;

    private Material _defaultMat;
    private SpriteRenderer _characterSprite;
    private CharacterMovement _characterMovement;
    private ResourcesChanger _resourcesChanger;
    private int _currentAmount;
    private int _colorIndex;
    private ushort _showEffectNum = 10;
    [Header("OutLine Color")]
    private bool _isColorChanging;
    [ColorUsage(true,true)]
    [SerializeField] private Color[] _colorsHDR; // Colors will be set in Inspector
    private static readonly int OutLineColor = Shader.PropertyToID("_Color");
    

    private void Awake()
    {
        _characterMovement = FindObjectOfType<CharacterMovement>();
        _characterSprite = _characterMovement.transform.GetChild(0).GetComponent<SpriteRenderer>();
        _defaultMat = _characterSprite.material;
        _resourcesChanger = FindObjectOfType<ResourcesChanger>();
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
        ComboAction();
        ShowCombo();
    }

    private void ComboAction()
    {
        _colorIndex = _combo / _showEffectNum;
        if (_colorIndex == 1)
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
        GameObject comboGo = Instantiate(_comboTextPrefab, _characterMovement.transform.position, Quaternion.Euler(Vector3.zero),
            transform);
        ComboText comboText = comboGo.GetComponent<ComboText>();

        comboText.SetText(_combo, _colorIndex);
        comboText.SetPlayerSpeed(_characterMovement.MoveSpeed, _resourcesChanger.DefaultSpeed);
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
