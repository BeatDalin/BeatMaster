// using System;
// using System.Collections;
// using System.Collections.Generic;
// using SonicBloom.Koreo;
// using UnityEngine;
// using System.Linq;
//
// public abstract class NoteCreator : MonoBehaviour
// {
//     public int SampleRate { get => playingKoreo.SampleRate; }
//     public int CurrentSampleTime { get => playingKoreo.GetLatestSampleTime(); }
//     public KoreographyEvent CurrentEvent { get => rawEvents[noteIndex]; }
//
//     [EventID][SerializeField] protected string eventID;
//
//     [SerializeField] protected int poolAmount = 10;
//     [SerializeField] protected GameObject notePrefab;
//     [SerializeField] protected Transform backgroundPanel;
//
//     protected int noteMaxCount;
//     protected int noteIndex;
//     protected Koreography playingKoreo;
//     protected Stack<GameObject> poolStack = new Stack<GameObject>();
//     protected List<GameObject> activeObjects = new List<GameObject>();
//     protected List<KoreographyEvent> rawEvents = new List<KoreographyEvent>();
//
//
//     protected void Awake()
//     {
//         playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);
//
//         KoreographyTrack rhythmTrack = SoundManager.instance.playingKoreo.GetTrackByID("BossEventCheck");
//         rawEvents = rhythmTrack.GetAllEvents();
//
//         noteMaxCount = rawEvents.Count;
//
//         Init();
//     }
//
//     protected abstract void Init();
//
//     protected GameObject GetObject()
//     {
//         if (poolStack.Count > 0)
//         {
//             GameObject go = poolStack.Pop();
//             go.SetActive(true);
//             go.transform.SetParent(backgroundPanel);
//             activeObjects.Add(go);
//             noteIndex++;
//             return go;
//         }
//         else
//         {
//             if (poolAmount >= noteMaxCount)
//             {
//                 return null;
//             }
//
//             CreateNewObject();
//             return GetObject();
//         }
//     }
//
//     protected virtual GameObject CreateNewObject()
//     {
//         GameObject go = Instantiate(notePrefab);
//         go.SetActive(false);
//         go.transform.SetParent(transform);
//         poolStack.Push(go);
//         return go;
//     }
//
//     public void ReturnObject(GameObject go)
//     {
//         go.SetActive(false);
//         activeObjects.Remove(go);
//         go.transform.SetParent(transform);
//         poolStack.Push(go);
//         if (noteIndex < noteMaxCount)
//         {
//             GetObject();
//         }
//     }
//
//     public void ReturnLastObject()
//     {
//         activeObjects.First().GetComponent<Note>().EndLine();
//     }
// }