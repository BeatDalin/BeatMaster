using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UIElements;

public class CreateMap : MonoBehaviour
{
    [EventID] 
    public string eventID;

    public int beatPerMeasure;
    public int tileCount;
    public int beatCount;
    public Tilemap tileMap;
    public Tilemap beattileMap;
    
    // 장애물 타일
    // [0] = 일반 타일,  [1] = 점프 타일, [2] = 적 타일 
    [SerializeField] private Tile[] _obstacleTiles;

    private Vector3Int _startPos;
    private Koreography _playingKoreo;
    
    // Start is called before the first frame update
    private void Start()
    {
        Koreographer.Instance.RegisterForEvents(eventID, CreateTile);
        Koreographer.Instance.RegisterForEvents("BeatTrack", CreateObstacleTile);

        Debug.Log(Koreographer.Instance.GetMusicBeatTime());
        _playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);
        tileCount = _playingKoreo.GetTrackByID("Level3Track").GetAllEvents().Count;
        beatCount = _playingKoreo.GetTrackByID("BeatTrack").GetAllEvents().Count;
        
        _startPos = new Vector3Int(-10, -4, 0);
    }

    private void CreateTile(KoreographyEvent evt)
    {
        
        for (int i = 0; i <= tileCount; i++)
        {
            tileMap.SetTile(new Vector3Int(_startPos.x + i, _startPos.y, _startPos.z), _obstacleTiles[0]);
        }
    }

    private void CreateObstacleTile(KoreographyEvent evt)
    {
        for (int i = 0; i <= beatCount; i++)
        {
            beattileMap.SetTile(new Vector3Int(_startPos.x + (i * beatPerMeasure), _startPos.y , _startPos.z), _obstacleTiles[1]);
        }
    }
}
