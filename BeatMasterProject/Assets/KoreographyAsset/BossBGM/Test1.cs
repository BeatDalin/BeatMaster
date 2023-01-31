using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class Test1 : MonoBehaviour
{
    public KoreographyTrack track;
    void Start()
    {
        var evt = track.GetAllEvents();
        for (int i = 0; i < track.GetAllEvents().Count; i++)
        {

                var start = evt[i].StartSample - 5000;
                var end = evt[i].StartSample + 5000;

                var trackEvent = evt[i];
                trackEvent.Payload = new CurvePayload();
                Debug.Log(trackEvent.HasCurvePayload());
                trackEvent.StartSample = start;
                trackEvent.EndSample = end;
        }
    }

    void Update()
    {
        
    }
}
