using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using Unity.VisualScripting;
using UnityEngine;

public class CalibarateNote : MonoBehaviour
{
    [SerializeField] private KoreographyTrack _originTrack;
    [SerializeField] private KoreographyTrack _spectrumTrack;
    [SerializeField] private KoreographyTrack _newTrack;

    private void Start()
    {
        for (int i = 0; i < _originTrack.GetAllEvents().Count; i++) 
            // 스펙트럼 분석한 트랙과 사람이
            // 찍은 트랙을 비교해서 사람이 찍은 것과 비슷한 곳에 
            // 스펙트럼이 있으면 새로운 트랙에 이벤트 생성
        {
            for (int j = 0; j < _spectrumTrack.GetAllEvents().Count; j++)
            {
                if (_originTrack.GetAllEvents()[i].StartSample - 10000 <= _spectrumTrack.GetAllEvents()[j].StartSample
                    && _originTrack.GetAllEvents()[i].EndSample + 10000 >= _spectrumTrack.GetAllEvents()[j].EndSample)
                {
                    KoreographyEvent koreographyEvent = new KoreographyEvent();
                    koreographyEvent.Payload = _originTrack.GetAllEvents()[i].Payload;
                    koreographyEvent.StartSample = _spectrumTrack.GetAllEvents()[j].StartSample;
                    koreographyEvent.EndSample = _spectrumTrack.GetAllEvents()[j].EndSample;
                    _newTrack.AddEvent(koreographyEvent);
                }
            }
        }
    }
}