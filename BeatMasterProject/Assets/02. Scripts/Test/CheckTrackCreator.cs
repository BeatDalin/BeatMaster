using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class CheckTrackCreator : MonoBehaviour
{
    [Header("Map Track")]
    public KoreographyTrack mapTrack;
    
    [Header("Long Check Track")]
    public KoreographyTrack longTrack; // long note track

    // Track for check long notes to be created 
    public KoreographyTrack longCheckStart;
    public KoreographyTrack longCheckEnd;
    public KoreographyTrack longCheckMiddle;

    [Header("Short Check Track")] public KoreographyTrack shortTrack; // short note track
    [SerializeField] KoreographyTrack _jumpCheckTrack; // Track for check to be created
    [SerializeField] KoreographyTrack _attackCheckTrack; // Track for check to be created
    [SerializeField] private List<KoreographyEvent> _shortEventList = new List<KoreographyEvent>();

    [Header("Check Point Track")] public KoreographyTrack spdTrack;
    public KoreographyTrack checkPointTrack;
    
    [Header("Sync Attack and Long Note")]
    public List<KoreographyEvent> shortEvents;
    public List<KoreographyEvent> longEvents;

    private void Start()
    {
        // If you want to create check events, call functions here
        // ConvertMapToText();
        GenerateJumpCheckEvent();
        GenerateAttackCheckEvent();
        // GenerateLongCheckEvent();
        // GenerateCheckPointEvent();
        // SyncAttackLong();
    }

    private void ConvertMapToText()
    {
        // Convert Map
        var mapEvents = mapTrack.GetAllEvents();
        for (int i = 0; i < mapEvents.Count; i++)
        {
            if (mapEvents[i].HasIntPayload())
            {
                var value = mapEvents[i].GetIntValue();
                TextPayload textPayload = new TextPayload();
                textPayload.TextVal = value.ToString();
                mapEvents[i].Payload = textPayload;
            }
        }
    }

    private void GenerateLongCheckEvent()
    {
        // Important Note : You have to convert all event payloads as Curve after creating CheckEvents
        var allEvents = longTrack.GetAllEvents();
        for (int i = 0; i < allEvents.Count; i++)
        {
            var start = allEvents[i].StartSample;
            var end = allEvents[i].EndSample;
            // Long Check Start
            var koreoEventStart = new KoreographyEvent();
            koreoEventStart.StartSample = start - 8000;
            koreoEventStart.EndSample = start + 8000;
            longCheckStart.AddEvent(koreoEventStart);

            // Long Check End
            var koreoEventEnd = new KoreographyEvent();
            koreoEventEnd.StartSample = end - 8000;
            koreoEventEnd.EndSample = end + 8000;
            longCheckEnd.AddEvent(koreoEventEnd);

            // Long Check Middle
            var koreoEventMiddle = new KoreographyEvent();
            koreoEventMiddle.StartSample = start + 8001;
            koreoEventMiddle.EndSample = end - 8001;
            longCheckMiddle.AddEvent(koreoEventMiddle);
        }
    }


    private void GenerateJumpCheckEvent()
    {
        _shortEventList = shortTrack.GetAllEvents();
        for (int i = 0; i < _shortEventList.Count; i++)
        {
            int shortType = _shortEventList[i].GetIntValue();

            if (shortType == 0)
            {
                KoreographyEvent koreoEvent = new KoreographyEvent();
                koreoEvent.Payload = new CurvePayload();
                koreoEvent.StartSample = _shortEventList[i].StartSample - 5000;
                koreoEvent.EndSample = _shortEventList[i].StartSample + 5000;

                _jumpCheckTrack.AddEvent(koreoEvent);
            }
        }
    }

    private void GenerateAttackCheckEvent()
    {
        _shortEventList = shortTrack.GetAllEvents();
        for (int i = 0; i < _shortEventList.Count; i++)
        {
            int shortType = _shortEventList[i].GetIntValue();

            if (shortType == 1)
            {
                KoreographyEvent koreoEvent = new KoreographyEvent();
                koreoEvent.Payload = new CurvePayload();
                koreoEvent.StartSample = _shortEventList[i].StartSample - 5000;
                koreoEvent.EndSample = _shortEventList[i].StartSample + 5000;

                _attackCheckTrack.AddEvent(koreoEvent);
            }
        }
    }

    private void GenerateCheckPointEvent()
    {
        var spdEvents = spdTrack.GetAllEvents();
        for (int i = 0; i < spdEvents.Count - 1; i++)
        {
            KoreographyEvent koreoEvent = new KoreographyEvent();
            koreoEvent.Payload = new CurvePayload();
            koreoEvent.StartSample = spdEvents[i].StartSample - 3000;
            koreoEvent.EndSample = spdEvents[i].StartSample + 3000;
            checkPointTrack.AddEvent(koreoEvent);
        }
    }

    private void VerifyShortNote()
    {
        shortEvents = shortTrack.GetAllEvents();
        for (int i = shortEvents.Count-1; i >= 1 ; i++)
        {
            if (shortEvents[i - 1].StartSample == shortEvents[i].StartSample)
            {
                shortEvents.Remove(shortEvents[i]);
            }
        }
    }

    private void SyncAttackLong()
    {
        longEvents = longTrack.GetAllEvents();
        shortEvents = shortTrack.GetAllEvents();
        int longIdx = 0;
        int longEndSample = 0;
        for (int i = 0; i < shortEvents.Count; i++)
        {
            // Long의 start sample은 반드시 0으로
            if (shortEvents[i].StartSample == longEvents[longIdx].StartSample)
            {
                var pay = new IntPayload();
                pay.IntVal = 0;
                shortEvents[i].Payload = pay;
                longEndSample = longEvents[longIdx].EndSample;
                longIdx++;
            }
            // long 노트 중간의 short note
            else if (shortEvents[i].StartSample <= longEndSample)
            {
                var pay = new IntPayload();
                pay.IntVal = 1;
                shortEvents[i].Payload = pay;
            }
        }
    }
}