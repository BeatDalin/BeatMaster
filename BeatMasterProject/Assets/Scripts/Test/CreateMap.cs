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
    public int tileCount;
    
    public Tilemap tileMap;
    
    // 장애물 타일
    // [0] = 일반 타일,  [1] = 점프 타일, [2] 적 타일 
    [SerializeField] private Tile[] _obstacleTiles;

    private Vector3Int _startPos;
    private Koreography _playingKoreo;
    
    // Start is called before the first frame update
    private void Start()
    {
        Koreographer.Instance.RegisterForEvents(eventID, CreateTile);
        
        _playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);
        tileCount = _playingKoreo.GetTrackByID("Level3Track").GetAllEvents().Count;
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
        
    }
}
