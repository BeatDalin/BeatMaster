using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class CheckTrackCreator : MonoBehaviour
{
    [Header("Long Check Track")]
    public KoreographyTrack longTrack; // long note track
    // Track for check long notes to be created 
    public KoreographyTrack longCheckStart;
    public KoreographyTrack longCheckEnd;
    public KoreographyTrack longCheckMiddle;

    [Header("Short Check Track")]
    public KoreographyTrack shortTrack; // short note track
    [SerializeField] KoreographyTrack _jumpCheckTrack; // Track for check to be created
    [SerializeField] private List<KoreographyEvent> _shortEventList = new List<KoreographyEvent>();

    private void Start()
    {
        // If you want to create check events, call functions here
        // GenerateJumpCheckEvent();
        // GenerateLongCheckEvent();
    }


    private void GenerateLongCheckEvent()
    {
        var allEvents = longTrack.GetAllEvents();
        for (int i = 0; i < allEvents.Count; i++)
        {
            var start = allEvents[i].StartSample;
            var end = allEvents[i].EndSample;
            // Long Check Start
            var koreoEventStart = new KoreographyEvent();
            koreoEventStart.StartSample = start-5000;
            koreoEventStart.EndSample = start+5000;
            longCheckStart.AddEvent(koreoEventStart);
            
            // Long Check End
            var koreoEventEnd = new KoreographyEvent();
            koreoEventEnd.StartSample = end-5000;
            koreoEventEnd.EndSample = end+5000;
            longCheckEnd.AddEvent(koreoEventEnd);
            
            // Long Check Middle
            var koreoEventMiddle = new KoreographyEvent();
            koreoEventMiddle.StartSample = start+5001;
            koreoEventMiddle.EndSample = end-5001;
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
}
