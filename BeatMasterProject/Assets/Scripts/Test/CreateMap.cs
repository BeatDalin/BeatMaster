using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class CreateMap : MonoBehaviour
{
    [EventID] public string eventID;

    public int tileCount = 0;
    
    [SerializeField] private Tilemap _map;
    [SerializeField] private Tile[] _tiles;
    
    private Koreography _playingKoreo;
    
    private int _width = -1;
    private int _height = 0;

    public int beat;
    private int note;
    private void Awake()
    {
        Koreographer.Instance.RegisterForEvents(eventID, CreateTile);

        _playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);
        tileCount = _playingKoreo.GetTrackByID(eventID).GetAllEvents().Count;

    }

    private void CreateTile(KoreographyEvent evt)
    {
        Debug.Log(Koreographer.Instance.GetMusicBeatTime());
        if (evt.GetTextValue() == "Start")
        {
            for (int i = 0; i <= tileCount; i++)
            {
                _width++;
                _map.SetTile(new Vector3Int(_width, _height, 0), _tiles[0]);
            }
        }
    }
    
}