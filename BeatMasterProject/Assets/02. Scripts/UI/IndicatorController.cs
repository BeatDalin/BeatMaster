using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;
using UnityEngine.UI;

public class IndicatorController : MonoBehaviour
{
    [SerializeField] [EventID] private string _indicatorStart;
    [SerializeField] [EventID] private string _indicator;
    [SerializeField] private float _moveAmount = 0.1f;
    [SerializeField] private float _changeAmount = 0.1f;
    [SerializeField] private Image[] _indicatorImages;
    [SerializeField] private Text[] _indicatorsTexts;
    [SerializeField] private Vector2Int _targetTextureSize = new Vector2Int(64, 64);
    [SerializeField] private Vector2Int _targetTexturePos = new Vector2Int(0, 128);

    private float _goalPosX;
    private int _preIndex;
    private bool _isIndicatorMove;
    private RectTransform _canvasRectTrans;
    private Game _game;
    // StopCoroutine을 사용하기 위해 이전 코루틴을 받았다
    // -> 특이한 점은 StopCoroutine이 StartCoroutine(); 즉 Coroutine타입을 받는다는 점이다.
    private Coroutine _preCoroutine;
    private Color[] _indicatorColors;
    private List<RectTransform> _indicatorsTransforms = new List<RectTransform>();
    private Dictionary<int, Animator> _animatorDict = new Dictionary<int, Animator>();

    private void Awake()
    {
        GetComponent<Canvas>().worldCamera = Camera.main;
        _game = FindObjectOfType<Game>();
    }

    private void Start()
    {
        Koreographer.Instance.RegisterForEvents(_indicatorStart, StartIndicating);
        Koreographer.Instance.RegisterForEvents(_indicator, PlayNextAnim);
        Init();
    }

    private void Init()
    {
        _canvasRectTrans = GetComponent<RectTransform>();

        int childCount = transform.childCount;
        _indicatorColors = new Color[childCount];
        
        for (int i = 0; i < childCount; i++)
        {
            RectTransform targetRectTrans = transform.GetChild(i).GetComponent<RectTransform>();
            _indicatorsTransforms.Add(targetRectTrans);
            _goalPosX = targetRectTrans.anchoredPosition.x;
            // _startPosX = _goalPosX + _canvasRectTrans.rect.width;
            float startPosX = _goalPosX + _canvasRectTrans.rect.width;
            
            targetRectTrans.anchoredPosition = new Vector2(startPosX, targetRectTrans.anchoredPosition.y);
            
            _animatorDict.Add(i, _indicatorsTransforms[i].GetComponentInChildren<Animator>());
            _animatorDict[i].enabled = false;

            Vector4 colorVec = Vector4.zero;
            
            // 타겟 텍스처 position x: 0~64 y: 128 ~ 192 
            int count = 0;

            for (int j = _targetTexturePos.x; j < _targetTexturePos.x + _targetTextureSize.x; j++)
            {
                for (int k = _targetTexturePos.y; k < _targetTexturePos.y + _targetTextureSize.y; k++)
                {
                    Color color = _indicatorImages[i].sprite.texture.GetPixel(j, k);
                    // 1로 지정 안 하니까 극단적인 값이 나왔음 하늘색인데 파란색이 나오는 등
                    if (!color.a.Equals(1f))
                    {
                        continue;
                    }

                    colorVec.x += color.r;
                    colorVec.y += color.g;
                    colorVec.z += color.b;
                    count++;
                }
            }

            colorVec.x /= count;
            colorVec.y /= count;
            colorVec.z /= count;

            _indicatorColors[i] = new Color(colorVec.x, colorVec.y, colorVec.z, 1f);
        }
    }

    private void PlayNextAnim(KoreographyEvent evt)
    {
        int index = evt.GetIntValue();
        
        SetColorToOffset(_preIndex);
        
        if (index == _animatorDict.Count)
        {
            StopAnim();
            return;
        }

        _animatorDict[_preIndex].Rebind();
        _animatorDict[_preIndex].enabled = false;
        
        _preCoroutine = StartCoroutine(CoChangeColor(index));
        _animatorDict[index].enabled = true;
        
        _preIndex = index;
    }

    private void StartIndicating(KoreographyEvent evt)
    {
        int value = evt.GetIntValue();
        StartCoroutine(CoMoveIndicatingBox(value));
    }

    private IEnumerator CoMoveIndicatingBox(int index)
    {
        RectTransform targetTrans = _indicatorsTransforms[index];
        float ratio = 0f;
        
        while (ratio < 1)
        {
            // currentPos알기
            while (_game.curState == GameState.Pause)
            {
                yield return null;
            }
            
            ratio += Time.deltaTime * _moveAmount;
            float currentPosX = Mathf.Lerp(targetTrans.anchoredPosition.x, _goalPosX, ratio);
            targetTrans.anchoredPosition = new Vector2(currentPosX, targetTrans.anchoredPosition.y);
            
            yield return null;
        }
    }

    private void StopAnim()
    {
        _animatorDict[_preIndex].Rebind();
        _animatorDict[_preIndex].enabled = false;
    }

    private IEnumerator CoChangeColor(int index)
    {
        Color goalColor = _indicatorColors[index];
        float afterTime = 0f;
        while (true)
        {
            while (_game.curState == GameState.Pause)
            {
                yield return null;
            }
            
            afterTime += Time.deltaTime;
            float ratio = (Mathf.Cos((Mathf.PI) + afterTime * _changeAmount) + 1) / 2;
            Color color = Color.Lerp(Color.white, goalColor, ratio);
            _indicatorsTexts[index].color = color;
            yield return null;
        }
    }

    private void SetColorToOffset(int preIndex)
    {
        if (_preCoroutine != null)
        {
            StopCoroutine(_preCoroutine);
        }
        
        _indicatorsTexts[preIndex].color = Color.white;
    }
}
