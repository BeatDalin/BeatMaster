using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    [SerializeField] private float _pixelMultiplier = 0.75f;
    

    public bool IsFeverTime { get; private set; }

    private Canvas _myCanvas;
    private Animator[] _corgiAnims;
    private Image[] _playerImages;
    private Slider _feverSlider;
    private Image _fillImage;
    private Coroutine _changeAlphaCoroutine;
    private Coroutine _feverTimeCoroutine;
    private Coroutine _feverIncreasingCoroutine;
    private RectTransform _sliderRectTrans;
    private RectTransform _leftCorgiRectTrans;
    
    private ParticleSystem _sliderBarParticle;
    private FallingCorgiController _fallingCorgiController;
    private int _currentPlayerIndex;
    private Vector3 _offsetPos;
    private float _feverRatio;

    enum AnimatorName
    {
        FeverIdle,
        FeverSniff,
        FeverJump,
        FeverSit,
    }

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
        _sliderRectTrans = _feverSlider.GetComponent<RectTransform>();
        _offsetPos = _sliderRectTrans.anchoredPosition;
        _sliderBarParticle = _feverSlider.GetComponentInChildren<ParticleSystem>();

        Animator[] anims = GetComponentsInChildren<Animator>();
        _corgiAnims = new Animator[anims.Length];
        _playerImages = new Image[_corgiAnims.Length];
        _corgiAnims = anims;

        // 좌측 CorgiImage RectTransform
        _leftCorgiRectTrans = _corgiAnims[1].GetComponent<RectTransform>();

        _fillImage = _feverSlider.fillRect.GetComponent<Image>();

        for (int i = 0; i < _playerImages.Length; i++)
        {
            _playerImages[i] = _corgiAnims[i].GetComponent<Image>();
        }

        SetCorgiImageDefault();

    }

    public void IncreaseFeverGage()
    {
        if (_feverIncreasingCoroutine != null)
        {
            StopCoroutine(_feverIncreasingCoroutine);
        }
        
        _decreasingAmount++;

        if (IsFeverTime)
        {
            return;
        }

        _feverCount++;
        _feverRatio = (float)_feverCount / _feverStandard;
        
        if (_feverRatio == 1f)
        {
            ChangeColor(_feverRatio, 1f);
            _feverSlider.value = _feverRatio;
            
            IsFeverTime = true;
            _feverCount = 0;
            _feverTimeCoroutine = StartCoroutine(CoPlayFeverTime());
        }

        _feverIncreasingCoroutine = StartCoroutine(IncreaseSlider(_feverRatio, 1f));
        //ChangeColor(ratio, 1f);
    }

    private void ChangeColor(float hue, float sat)
    {
        float h, s, v;
        Color.RGBToHSV(Color.white, out h, out s, out v);

        h = hue;
        s = sat;
        Color color = Color.HSVToRGB(h, s, v);

        _fillImage.color = color;
    }

    private IEnumerator IncreaseSlider(float feverRatio, float sat)
    {
        float h, s, v;
        Color.RGBToHSV(Color.white, out h, out s, out v);

        h = feverRatio;
        s = sat;
        Color color = Color.HSVToRGB(h, s, v);
        Color startColor = _fillImage.color;
        float startValue = _feverSlider.value;

        float ratio = 0f;
        while (ratio < 1f)
        {
            // color
            ratio += Time.deltaTime * _timeMod;
            _fillImage.color = Color.Lerp(startColor, color, ratio);
            _feverSlider.value = Mathf.Lerp(startValue, feverRatio, ratio);
            yield return null;
        }
    }
    
    private IEnumerator CoPlayFeverTime()
    {
        _changeAlphaCoroutine = StartCoroutine(CoChangeCorgiAlpha());
        
        _sliderBarParticle.Play();

        StartCoroutine(CoSliderMove());
        
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
        // 초기화

        if (IsFeverTime)
        {
            IsFeverTime = false;

            _feverCount = 0;
            _feverSlider.value = _feverCount;
            ChangeColor(0f, 0f);
            SetCorgiImageDefault();
            _fallingCorgiController.StopFallingCorgi();

            StartCoroutine(CoSliderMove());

            if (_feverIncreasingCoroutine != null)
            {
                StopCoroutine(_feverIncreasingCoroutine);
            }

            if (_changeAlphaCoroutine != null)
            {
                StopCoroutine(_changeAlphaCoroutine);
            }
            
            if (_feverTimeCoroutine != null)
            {
                StopCoroutine(_feverTimeCoroutine);
            }
            
            _sliderBarParticle.Stop();
        }
        else
        {
            DecreaseFeverTime();
            ResetDecreasingAmount();
            float ratio = (float)_feverCount / _feverStandard;
            ChangeColor(ratio, 1f);
        }
    }


    public void ResetDecreasingAmount()
    {
        _decreasingAmount = 0;
    }

    public void SetPlayerIndex(int index)
    {
        _currentPlayerIndex = index;
        int length = _fallingCorgiController.GetSpritesLength() - 1;
        float pixelPerUnit = _currentPlayerIndex < length ? 1f : _pixelMultiplier;
        Debug.Log($"length : {length} _currentPlayerIndex : {_currentPlayerIndex} pixelPerUnit : {pixelPerUnit}");
        
        
        foreach (var image in _playerImages)
        {
            image.pixelsPerUnitMultiplier = pixelPerUnit;
        }
        
        SetAnimatorDefault();
    }

    private void DecreaseFeverTime()
    {
        _feverCount = DecreaseCount(_feverCount);
        _feverSlider.value = (float)_feverCount / _feverStandard;
    }

    private int DecreaseCount(int amount)
    {
        amount -= _decreasingAmount;
        return Mathf.Clamp(amount, 0, _feverStandard);
    }

    private float GetCosValue(float time)
    {
        float changingRatio = (Mathf.Cos(Mathf.PI + time) + 1) / 2f;
        return changingRatio;
    }

    private IEnumerator CoChangeCorgiAlpha()
    {
        float ratio = 0f;
        bool isDone = false;
        while (!isDone)
        {
            ratio += Time.deltaTime * _timeMod;
            foreach (var image in _playerImages)
            {
                Color color = image.color;
                color.a = GetCosValue(ratio);
                image.color = color;
            }

            if (ratio <= 0.1f)
            {
                isDone = true;
            }

            yield return null;
        }
    }

    private void SetCorgiImageDefault()
    {
        foreach (var corgi in _playerImages)
        {
            Color color = corgi.color;
            color.a = 0f;
            corgi.color = color;
        }
    }

    private void SetAnimatorDefault()
    {
        string[] names = Enum.GetNames(typeof(AnimatorName));
        foreach (var anim in _corgiAnims)
        {
            foreach (var animName in names)
            {
                // 초기 값이 애니메이터이름0 임
                if (anim.GetCurrentAnimatorStateInfo(0).IsName($"{animName}0"))
                {
                    // 애니메이터의 다른 애니메이션은 1씩 더하는 식으로
                    anim.Play($"{animName}{_currentPlayerIndex}");
                }
            }
        }
    }

    private IEnumerator CoSliderMove()
    {
        float ratio = 0f;
        float increasingPosX = _leftCorgiRectTrans.rect.width / 2f;
        Debug.Log(increasingPosX);
        Vector3 goalPos = _offsetPos;
        bool isRight = _sliderRectTrans.anchoredPosition.x > _offsetPos.x;
        // 오른쪽이면 왼쪽 왼쪽이면 오른쪽으로
        goalPos.x += isRight ? 0 : increasingPosX;
        
        while (ratio < 1f)
        {
            ratio += Time.deltaTime;
            float currentPosX = Mathf.Lerp(_sliderRectTrans.anchoredPosition.x, goalPos.x, ratio);
            Vector3 tempVec = _sliderRectTrans.anchoredPosition;
            tempVec.x = currentPosX;
            _sliderRectTrans.anchoredPosition = tempVec;
            
            yield return null;
        }
    }

}
