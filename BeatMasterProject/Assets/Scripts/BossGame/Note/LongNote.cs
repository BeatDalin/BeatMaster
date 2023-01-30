using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNote : Note
{
    private int _startSample;
    private int _endSample;
    private RectTransform _myRect;
    private Vector3 _initPos;
    private float _offsetX;
    

    protected override void OnEnable()
    {
        base.OnEnable();
        
        float samplePerUnit = noteCreator.SampleRate;
        
        _startSample = myEvent.StartSample;
        _endSample = myEvent.EndSample;

        Vector2 tempRectVec = _myRect.sizeDelta;
        float sizeX = (_endSample - _startSample) / samplePerUnit * Screen.width;
        tempRectVec.x = sizeX;
        _myRect.sizeDelta = tempRectVec;

        _offsetX = transform.localPosition.x;

        Vector2 pivotVec = new Vector2(0f, 0.5f);
        _myRect.pivot = pivotVec;

        
        _initPos = transform.localPosition;
        _initPos.y = 0f;
        _initPos.x = _myRect.sizeDelta.x;
    }

    // 기준위치를 정하기 위함
    private void OnDisable()
    {
        SetOffsetPos();
    }

    private void SetOffsetPos()
    {
        Vector3 tempPos = transform.localPosition;
        tempPos.x = _offsetX;
        transform.localPosition = tempPos;
    }

    protected override void Init()
    {
        _myRect = GetComponent<RectTransform>();
        noteCreator = FindObjectOfType<LongNoteCreator>();
        destroyPos = GameObject.Find("DestroyPos").transform.localPosition;
    }

    protected override void CrossEndLine()
    {
        if (destroyPos.x < transform.localPosition.x + _initPos.x)
        {
            return;
        }
        noteCreator.ReturnObject(gameObject);
    }

    protected override void MovePosition()
    {
        samplePerUnit = noteCreator.SampleRate;
        // 목표 위치
        
        Vector3 pos = noteCreator.transform.localPosition;
        pos.x -= (noteCreator.CurrentSampleTime - myEvent.StartSample) / samplePerUnit * Screen.width;
        transform.localPosition = pos;
    }
    
    
}
