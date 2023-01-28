using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public abstract class Note : MonoBehaviour
{
    protected NoteCreator noteCreator;
    protected KoreographyEvent myEvent;
    protected Vector3 destroyPos;
    protected float samplePerUnit;
    
    private void Awake()
    {
         Init();
    }

    protected virtual void OnEnable()
    {
        myEvent = noteCreator.CurrentEvent;
    }

    void Update()
    {
        MovePosition();
        CrossEndLine();
    }

    protected abstract void Init();

    protected virtual void CrossEndLine()
    {
        if (destroyPos.x < transform.localPosition.x)
        {
            return;
        }
        noteCreator.ReturnObject(gameObject);
    }

    protected virtual void MovePosition()
    {
        samplePerUnit = noteCreator.SampleRate;
        // 목표 위치
        Vector3 pos = noteCreator.transform.localPosition;
        // pos.x -= (_noteCreator.CurrentSampleTime - _myEvent.StartSample) / samplePerUnit;
        pos.x -= (noteCreator.CurrentSampleTime - myEvent.StartSample) / samplePerUnit * Screen.width;
        transform.localPosition = pos;
        
    }
}
