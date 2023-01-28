using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LongNote : Note
{
    private int _startSample;
    private int _endSample;
    private RectTransform _myRect;
    private Vector3 initPos;
    

    protected override void OnEnable()
    {
        base.OnEnable();
        
        float samplePerUnit = noteCreator.SampleRate;
        
        _startSample = myEvent.StartSample;
        _endSample = myEvent.EndSample;

        Vector2 tempRectVec = _myRect.sizeDelta;
        float sizeX = (_endSample - _startSample) / samplePerUnit * Screen.width;
        tempRectVec.x += sizeX;
        _myRect.sizeDelta = tempRectVec;

        Debug.Log($"transform.localPosition : {transform.localPosition}");
        
        initPos = transform.localPosition;
        initPos.x += sizeX / 2f;
        Debug.Log($"after => transform.localPosition : {transform.localPosition}");
    }

    protected override void Init()
    {
        _myRect = GetComponent<RectTransform>();
        noteCreator = FindObjectOfType<LongNoteCreator>();
        destroyPos = GameObject.Find("DestroyPos").transform.localPosition;
    }

    protected override void CrossEndLine()
    {
        if (destroyPos.x < transform.localPosition.x + initPos.x)
        {
            return;
        }
        noteCreator.ReturnObject(gameObject);
    }

    protected override void MovePosition()
    {
        base.MovePosition();
        // 목표 위치
        
        Vector3 pos = noteCreator.transform.localPosition + initPos;
        // pos.x -= (_noteCreator.CurrentSampleTime - _myEvent.StartSample) / samplePerUnit;
        pos.x -= (noteCreator.CurrentSampleTime - myEvent.StartSample) / samplePerUnit * Screen.width;
        transform.localPosition = pos;
    }
    
    
}
