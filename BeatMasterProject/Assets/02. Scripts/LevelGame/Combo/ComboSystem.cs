using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class ComboSystem : ObjectPooling
{
    [SerializeField] private int _increasingAmount = 10000;
    [SerializeField] private ushort _combo;
    private Transform _characterTransform;
    [Header("OutLine Effect")]
    [SerializeField] private Material _outLineMat;

    private Material _defaultMat;
    private SpriteRenderer _characterSprite;
    private CharacterMovement _characterMovement;
    private ResourcesChanger _resourcesChanger;
    private Transform _canvasTrans;
    private Dictionary<GameObject, ComboText> _comboDict = new Dictionary<GameObject, ComboText>(); 
    private int _currentAmount;
    private int _colorIndex;
    private ushort _showEffectNum = 10;
    [Header("OutLine Color")]
    private bool _isColorChanging;
    [ColorUsage(true, true)]
    [SerializeField] private Color[] _colorsHDR; // Colors will be set in Inspector
    private static readonly int OutLineColor = Shader.PropertyToID("_Color");
    

    private void Awake()
    {
        _characterMovement = FindObjectOfType<CharacterMovement>();
        _characterSprite = _characterMovement.transform.GetChild(0).GetComponent<SpriteRenderer>();
        _defaultMat = _characterSprite.material;
        _resourcesChanger = FindObjectOfType<ResourcesChanger>();
        _canvasTrans = transform.GetChild(0).transform;
        Init();
        // Set Texture based on current character
        // _outLineMat.SetTexture("_MainTex", texture to push....);
    }

    

    void Start()
    {
        _currentAmount = _increasingAmount;
        // _colorChangeCoroutine = StartCoroutine(CoChangeOutLineColor());
    }
    
    protected override void Init()
    {
        for (int i = 0; i < initCount; i++)
        {
            GameObject go = CreateNewObject();
            _comboDict.Add(go, go.GetComponent<ComboText>());
        }
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

    public override GameObject GetObject(Vector3 touchPos)
    {
        var obj = poolingObjectQueue.Dequeue();
        obj.transform.position = Camera.main.WorldToScreenPoint(touchPos); 
        obj.transform.SetParent(_canvasTrans);
        return obj;
    }

    private void ShowCombo()
    {
        // 콤보 UI를 보여줌
        GameObject comboGo = GetObject(_characterMovement.transform.position);

        _comboDict[comboGo].SetPlayerSpeed(_characterMovement.MoveSpeed, _resourcesChanger.DefaultSpeed);
        _comboDict[comboGo].SetText(_combo, _colorIndex, this);
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
