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

    private float _startPosX;
    private float _goalPosX;
    private int _preIndex;
    private RectTransform _canvasRectTrans;
    private List<RectTransform> _indicatorsTransforms = new List<RectTransform>();
    private Dictionary<int, Animator> _animatorDict = new Dictionary<int, Animator>();

    private void Awake()
    {
        Init();
    }

    private void Init()
    {
        _canvasRectTrans = GetComponent<RectTransform>();
        Koreographer.Instance.RegisterForEvents(_indicatorStart, StartIndicating);
        Koreographer.Instance.RegisterForEvents(_indicator, PlayNextAnim);

        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform targetRectTrans = transform.GetChild(i).GetComponent<RectTransform>();
            _indicatorsTransforms.Add(targetRectTrans);
            _goalPosX = targetRectTrans.anchoredPosition.x;
            _startPosX = _goalPosX + _canvasRectTrans.rect.width;
            targetRectTrans.anchoredPosition = new Vector2(_startPosX, targetRectTrans.anchoredPosition.y);
            
            _animatorDict.Add(i, _indicatorsTransforms[i].GetComponentInChildren<Animator>());
            _animatorDict[i].enabled = false;
        }
        
    }

    private void PlayNextAnim(KoreographyEvent evt)
    {
        int index = evt.GetIntValue();
        _animatorDict[_preIndex].Rebind();
        // _animatorDict[_preIndex].Update(0f);
        
        _animatorDict[_preIndex].enabled = false;
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
            ratio += Time.deltaTime * _moveAmount;
            float currentPosX = Mathf.Lerp(_startPosX, _goalPosX, ratio);
            targetTrans.anchoredPosition = new Vector2(currentPosX, targetTrans.anchoredPosition.y);
            
            yield return null;
        }
        
        
    }

    private IEnumerator CoMoveTrans()
    {
        yield break;
    }

    void Update()
    {
        
    }
}
