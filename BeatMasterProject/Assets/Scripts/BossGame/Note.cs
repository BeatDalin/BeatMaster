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
    void Awake()
    {
         _bossController = FindObjectOfType<BossController>();
         _noteCreator = FindObjectOfType<NoteCreator>();
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
        Vector3 pos = _bossController.transform.position;
        pos.x -= (_noteCreator.CurrentSampleTime - _myEvent.StartSample) / samplePerUnit;
        transform.position = pos;
    }
}
