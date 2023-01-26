using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class Note : MonoBehaviour
{
    private BossController _bossController;
    private NoteCreator _noteCreator;
    private KoreographyEvent _myEvent;
    private Vector3 _spawnPos;
    void Awake()
    {
         _noteCreator = FindObjectOfType<NoteCreator>();
         _spawnPos = GameObject.Find("SpawnPos").transform.localPosition;
    }

    private void OnEnable()
    {
        _myEvent = _noteCreator.CurrentEvent;
         
    }

    void Update()
    {
        MovePosition();
    }

    private void MovePosition()
    {
        float samplePerUnit = _noteCreator.SampleRate;
        // 목표 위치
        Vector3 pos = _noteCreator.transform.localPosition;
        Debug.Log(_spawnPos);
        // pos.x -= (_noteCreator.CurrentSampleTime - _myEvent.StartSample) / samplePerUnit;
        pos.x -= (_noteCreator.CurrentSampleTime - _myEvent.StartSample) / samplePerUnit * Screen.width;
        transform.localPosition = pos;
    }
}
