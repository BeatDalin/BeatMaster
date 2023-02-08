using System;
using UnityEngine;

namespace SonicBloom.Koreo.Demos
{
    public class YPosNoteObjectBehaviour : NoteObjectBehaviour
    {
	    protected override void OnSamplesTillPerfectChanged()
	    {
		    base.OnSamplesTillPerfectChanged();
			// Get the number of samples we traverse given the current speed in Units-Per-Second.
			float samplesPerUnit = GameController.SampleRate / GameController.noteSpeed;
			Vector3 pos = LaneController.TargetPosition;
			pos.y -= noteObject.SamplesTillPerfect / samplesPerUnit;
			transform.position = pos;
        }
    }
}