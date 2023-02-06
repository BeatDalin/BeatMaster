using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class LongCheckCreator : MonoBehaviour
{
    public KoreographyTrack longTrack;
    public KoreographyTrack longCheckStart;
    public KoreographyTrack longCheckEnd;
    public KoreographyTrack longCheckMiddle;

    private void Start()
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
}
