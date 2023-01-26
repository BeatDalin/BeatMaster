using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using UnityEngine;
using SimpleMusicPlayer = SonicBloom.Koreo.Players.SimpleMusicPlayer;

public class NoteCreator : MonoBehaviour
{
    [EventID] public string eventID;
    
    public int SampleRate { get => _playingKoreo.SampleRate; }
    public int CurrentSampleTime { get => _playingKoreo.GetLatestSampleTime(); }
    public KoreographyEvent CurrentEvent { get => _rawEvents[_noteIndex]; }
    [SerializeField] private int _amount = 10;
    [SerializeField] private GameObject _notePrefab;

    private int _noteMaxCount;
    private int _noteIndex;
    private Koreography _playingKoreo;
    private SimpleMusicPlayer _musicPlayer;
    private AudioSource _audioSource;
    private AudioClip _audioClip;
    private Stack<GameObject> _stack = new Stack<GameObject>();
    private List<GameObject> _activeObjects = new List<GameObject>();
    private List<KoreographyEvent> _rawEvents = new List<KoreographyEvent>();
    
    
    
    private void Awake()
    {
        _musicPlayer = GetComponent<SimpleMusicPlayer>();
        _audioSource = GetComponent<AudioSource>();
        _playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);
        _audioClip = _playingKoreo.SourceClip;

        KoreographyTrack rhythmTrack = _playingKoreo.GetTrackByID(eventID);
        _rawEvents = rhythmTrack.GetAllEvents();
        _noteMaxCount = _rawEvents.Count; 
        
        Init();
        
        /*for (int i = 0; i < rawEvents.Count; i++)
        {
            KoreographyEvent evt = rawEvents[i];
            int payload = evt.GetIntValue();
        }*/
    }

    private void Init()
    {
        for (int i = 0; i < _amount; i++)
        {
            _stack.Push(CreateNewObject());
        }

    }

    private void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            GetObject();
        }
    }

    public GameObject GetObject()
    {
        if (_stack.Count > 0)
        {
            _amount++;
            GameObject go = _stack.Pop();
            go.SetActive(true);
            go.transform.SetParent(null);
            _activeObjects.Add(go);
            _noteIndex++;
            return go;
        }
        else
        {
            if (_amount >= _noteMaxCount)
                return null;
            CreateNewObject();
            return GetObject();
        }
    }

    private GameObject CreateNewObject()
    {
        GameObject go = Instantiate(_notePrefab);
        go.SetActive(false);
        go.transform.SetParent(transform);
        _stack.Push(go);
        return go;
    }

    public void ReturnObject(GameObject go)
    {
        _activeObjects.Remove(go);
        go.SetActive(false);
        go.transform.SetParent(transform);
        _stack.Push(go);
    }
    
}
