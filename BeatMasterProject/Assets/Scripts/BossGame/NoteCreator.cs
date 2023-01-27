using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using UnityEngine;

public class NoteCreator : MonoBehaviour
{
    [EventID] public string eventID;
    
    public int SampleRate { get => _playingKoreo.SampleRate; }
    public int CurrentSampleTime { get => _playingKoreo.GetLatestSampleTime(); }
    public KoreographyEvent CurrentEvent { get => _rawEvents[_noteIndex]; }
    [SerializeField] private int _poolAmount = 10;
    [SerializeField] private GameObject _notePrefab;
    [SerializeField] private Transform _backgroundPanel;

    private int _noteMaxCount;
    private int _noteIndex;
    private Koreography _playingKoreo;
    private SimpleMusicPlayer _musicPlayer;
    private AudioSource _audioSource;
    private AudioClip _audioClip;
    private Stack<GameObject> _poolStack = new Stack<GameObject>();
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
        for (int i = 0; i < _poolAmount; i++)
        {
            CreateNewObject();
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
        if (_poolStack.Count > 0)
        {
            GameObject go = _poolStack.Pop();
            go.SetActive(true);
            go.transform.SetParent(_backgroundPanel);
            _activeObjects.Add(go);
            _noteIndex++;
            return go;
        }
        else
        {
            if (_poolAmount >= _noteMaxCount)
            {
                return null;
            }
            CreateNewObject();
            return GetObject();
        }
    }

    private GameObject CreateNewObject()
    {
        GameObject go = Instantiate(_notePrefab);
        go.SetActive(false);
        go.transform.SetParent(transform);
        _poolStack.Push(go);
        return go;
    }

    public void ReturnObject(GameObject go)
    {
        _activeObjects.Remove(go);
        go.SetActive(false);
        go.transform.SetParent(transform);
        _poolStack.Push(go);
    }
    
}
