using UnityEngine;

namespace SonicBloom.Koreo.Demos
{
    public class ScaleNoteObjectBehaviour : NoteObjectBehaviour
    {
        [SerializeField] private int HalfSampleRange = 4800000;
        [SerializeField] private bool debug;
        protected override void OnSamplesTillPerfectChanged()
        {
            base.OnSamplesTillPerfectChanged();

            transform.localScale = Vector3.one * (1 - Mathf.Abs(noteObject.SamplesTillPerfect) / (float) HalfSampleRange);
            
            if (debug) Debug.Log(noteObject.SamplesTillPerfect);
        }
    }
}