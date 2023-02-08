using UnityEngine;

namespace SonicBloom.Koreo.Demos
{
    public class NoteObjectBehaviour : MonoBehaviour
    {
        [SerializeField] protected NoteObject noteObject;

        protected LaneController LaneController => noteObject.laneController;
        protected RhythmGameController GameController => noteObject.gameController;

        private void OnEnable()
        {
            noteObject.samplesTillPerfectChanged.AddListener(OnSamplesTillPerfectChanged);
        }
        
        private void OnDisable()
        {
            noteObject.samplesTillPerfectChanged.RemoveListener(OnSamplesTillPerfectChanged);
        }

        protected virtual void OnSamplesTillPerfectChanged()
        {
        }
    }
}