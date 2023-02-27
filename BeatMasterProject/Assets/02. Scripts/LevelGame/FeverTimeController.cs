using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class FeverTimeController : MonoBehaviour
{
    // 레벨마다 달라야됨
    [SerializeField] private ushort _feverStandard;
    [SerializeField] private int _feverCount;
    [SerializeField] private float _timeMod = 10f;
    [SerializeField] private float _feverTotalTime = 10f;
    [SerializeField] private int _decreasingAmount = 0;

    public bool IsFeverTime { get; private set; }

    private Canvas _myCanvas;
    private Animator[] _corgiAnims;
    private Image[] _corgiObjects;
    private Slider _feverSlider;
    private Image _fillImage;
    private Coroutine _changeAlphaCoroutine;
    private Coroutine _feverTimeCoroutine;
    
    private ParticleSystem _sliderBarParticle;
    private FallingCorgiController _fallingCorgiController;
    private int _increasingMod = 1;

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _myCanvas = GetComponent<Canvas>();
        _myCanvas.worldCamera = Camera.main;

        _fallingCorgiController = FindObjectOfType<FallingCorgiController>();
        
        _feverSlider = GetComponentInChildren<Slider>();
        _sliderBarParticle = _feverSlider.GetComponentInChildren<ParticleSystem>();

        Animator[] anims = GetComponentsInChildren<Animator>();
        _corgiAnims = new Animator[anims.Length];
        _corgiObjects = new Image[_corgiAnims.Length];
        _corgiAnims = anims;
        
        _fillImage = _feverSlider.fillRect.GetComponent<Image>();
        
        for (int i = 0; i < _corgiObjects.Length; i++)
        {
            _corgiObjects[i] = _corgiAnims[i].GetComponent<Image>();
        }
        SetAlphaDefualt();
    }

    public void IncreaseFeverGage()
    {
        if (IsFeverTime)
        {
            return;
        }

        _feverCount++;
        _decreasingAmount++;
        _feverSlider.value = (float)_feverCount / _feverStandard;
        float ratio = (float)_increasingMod / _feverStandard;

        if (_feverSlider.value == 1f)
        {
            IsFeverTime = true;
            _feverCount = 0;
            _feverTimeCoroutine = StartCoroutine(CoPlayFeverTime());
        }

        ChangeColor(ratio, 1f);
    }

    private void ChangeColor(float hue, float sat)
    {
        float h, s, v;
        Color.RGBToHSV(Color.white, out h, out s, out v);

        h = hue;
        s = sat;
        Color color = Color.HSVToRGB(h, s, v);

        Debug.Log(color);
        
        _fillImage.color = color;

        _increasingMod++;
    }

    private IEnumerator CoPlayFeverTime()
    {
        _changeAlphaCoroutine = StartCoroutine(ChangeCorgiAlpha());
        _sliderBarParticle.Play();
        
        _fallingCorgiController.InstantiateFallingCorgi();
        
        float time = 0f;
        float startValue = _feverSlider.value;
        
        while (_feverTotalTime >= time)
        {
            time += Time.deltaTime;
            float valueRatio = time / _feverTotalTime;
            _feverSlider.value = Mathf.Lerp(startValue, 0f, valueRatio);
            
            var changingRatio = GetCosValue(time);
            ChangeColor(changingRatio, 1f);
            
            yield return null;
        }

        Reset();
    }

    public void Reset()
    {
        Debug.Log("Reset");
        // 초기화
        if (_changeAlphaCoroutine != null)
        {
            StopCoroutine(_changeAlphaCoroutine);
        }

        if (_feverTimeCoroutine != null)
        {
            StopCoroutine(_feverTimeCoroutine);
        }

        SetAlphaDefualt();
        ChangeColor(0f, 0f);
        _sliderBarParticle.Stop();
        _fallingCorgiController.StopFallingCorgi();
        DecreaseFeverTime();
        
        if (IsFeverTime)
        {
            IsFeverTime = false;
            _feverCount = 0;
            _increasingMod = 1;
            _feverSlider.value = _feverCount;
        }
    }


    public void ResetDecreasingAmount()
    {
        _decreasingAmount = 0;
    }

    private void DecreaseFeverTime()
    {
        _increasingMod = DecreaseCount(_increasingMod);
        _feverCount = DecreaseCount(_feverCount);
        _feverSlider.value = (float)_feverCount / _feverStandard;
    }

    private int DecreaseCount(int amount)
    {
        amount -= _decreasingAmount;
        return Mathf.Clamp(amount, 0, amount);
    }

    private float GetCosValue(float time)
    {
        float changingRatio = (Mathf.Cos(Mathf.PI + time) + 1) / 2f;
        return changingRatio;
    }

    private IEnumerator ChangeCorgiAlpha()
    {
        float ratio = 0f;
        bool isDone = false;
        while (!isDone)
        {
            ratio += Time.deltaTime * _timeMod;
            foreach (var corgi in _corgiObjects)
            {
                Color color = corgi.color;
                color.a = GetCosValue(ratio);
                corgi.color = color;
            }

            if (ratio <= 0.1f)
            {
                isDone = true;
            }

            yield return null;
        }
    }

    private void SetAlphaDefualt()
    {
        foreach (var corgi in _corgiObjects)
        {
            Color color = corgi.color;
            color.a = 0f;
            corgi.color = color;
        }
    }
    
}
