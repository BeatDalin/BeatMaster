using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public abstract class NoteCreator : MonoBehaviour
{
    public int SampleRate { get => playingKoreo.SampleRate; }
    public int CurrentSampleTime { get => playingKoreo.GetLatestSampleTime(); }
    public KoreographyEvent CurrentEvent { get => rawEvents[noteIndex]; }

    [EventID] [SerializeField] protected string eventID;

    [SerializeField] protected int _poolAmount = 10;
    [SerializeField] protected GameObject _notePrefab;
    [SerializeField] protected Transform _backgroundPanel;

    protected int noteMaxCount;
    protected int noteIndex;
    protected Koreography playingKoreo;
    protected Stack<GameObject> poolStack = new Stack<GameObject>();
    protected List<GameObject> activeObjects = new List<GameObject>();
    protected List<KoreographyEvent> rawEvents = new List<KoreographyEvent>();


    protected void Awake()
    {
        playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);

        KoreographyTrack rhythmTrack = playingKoreo.GetTrackByID(eventID);
        rawEvents = rhythmTrack.GetAllEvents();
        
        noteMaxCount = rawEvents.Count;

        Debug.Log($"{name} : {noteMaxCount}");

        Init();
    }

    protected abstract void Init();

    public virtual GameObject GetObject()
    {
        if (poolStack.Count > 0)
        {
            GameObject go = poolStack.Pop();
            go.SetActive(true);
            go.transform.SetParent(_backgroundPanel);
            activeObjects.Add(go);
            noteIndex++;
            return go;
        }
        else
        {
            if (_poolAmount >= noteMaxCount)
            {
                return null;
            }

            CreateNewObject();
            return GetObject();
        }
    }

    protected virtual GameObject CreateNewObject()
    {
        GameObject go = Instantiate(_notePrefab);
        go.SetActive(false);
        go.transform.SetParent(transform);
        poolStack.Push(go);
        return go;
    }

    public virtual void ReturnObject(GameObject go)
    {
        activeObjects.Remove(go);
        go.SetActive(false);
        go.transform.SetParent(transform);
        poolStack.Push(go);
        if (noteIndex < noteMaxCount)
        {
            GetObject();
        }
    }
}