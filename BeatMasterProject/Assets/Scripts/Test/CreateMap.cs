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

    public int beatPerMeasure;

    public int tileCount;
    public int beatCount;
    public Tilemap tileMap;
    public Tilemap beatTileMap;

    // 장애물 타일
    // [0] = 일반 타일,  [1] = 점프 타일, [2] = 적 타일 
    [SerializeField] private Tile[] _tiles;

    private Vector3Int _startPos;
    private Koreography _playingKoreo;

    public int tileX = 0;
    public int tileY = 0;
    
    private void Awake()
    {
        Koreographer.Instance.RegisterForEvents("Level3_Map", CreateTile);
        //Koreographer.Instance.RegisterForEvents("BeatTrack", CreateBeat);

        _playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);
        tileCount = _playingKoreo.GetTrackByID("Level3_Map").GetAllEvents().Count;
        //beatCount = _playingKoreo.GetTrackByID("BeatTrack").GetAllEvents().Count;

        _startPos = Vector3Int.zero;
    }

    private void CreateTile(KoreographyEvent evt)
    {
        if (evt.GetIntValue() == 0)
        {
            for (int i = 0; i <= tileCount; i++)
            {
                tileX++;
                tileMap.SetTile(new Vector3Int(tileX, tileY, _startPos.z), _tiles[0]);
            }
        }
    }

    private void CreateBeat(KoreographyEvent evt)
    {
        if (evt.GetTextValue() == "Start")
        {
            for (int i = 0; i <= beatCount; i++)
            {
                beatTileMap.SetTile(new Vector3Int(_startPos.x + (i * beatPerMeasure), _startPos.y, _startPos.z),
                    _tiles[1]);
            }
        }
    }
}