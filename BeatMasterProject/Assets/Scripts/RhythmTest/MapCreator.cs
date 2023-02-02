using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using UnityEngine.Tilemaps;

public class MapCreator : MonoBehaviour
{
    // eventID[0] : 맵 eventID
    // eventID[1] : 장애물 eventID
    [EventID] public string[] eventID;

    public int tileCount;
    
    [SerializeField] private Tilemap[] _tilemap;
    [SerializeField] private Tile[] _tiles;

    private int _tileX = 0;
    [SerializeField] private Koreography _koreography;

    private void Awake()
    {
        _koreography = Koreographer.Instance.GetKoreographyAtIndex(0);
        tileCount = _koreography.GetTrackByID(eventID[0]).GetAllEvents().Count;
        
        CreateMap();
    }
    
    private void CreateMap()
    {
        for (int i = 0; i <= tileCount; i++)
        {
            _tilemap[0].SetTile(new Vector3Int(_tileX, -1, 0), _tiles[0]);
            _tileX++;
        }
    }
    
}
