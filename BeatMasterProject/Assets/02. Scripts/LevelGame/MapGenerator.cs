using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using SonicBloom.Koreo;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private Koreography _koreography;

    [Header("Event")]
    [SerializeField] [EventID] private string _mapEventID;
    [SerializeField] private List<KoreographyEvent> _mapEventList = new List<KoreographyEvent>();

    [Header("Ground")]
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private List<Tile> _groundTileList = new List<Tile>();

    [Header("Interaction")]
    [SerializeField] private Tilemap _interactionTilemap;
    [SerializeField] private List<Tile> _interactionTileList = new List<Tile>();

    private int _tileX = -1, _tileY;

    private void Awake()
    {
        //_koreography = Koreographer.Instance.GetKoreographyAtIndex(0);
    
        GenerateMap();
        GenerateJumpCheckEvent();
    }

    public KoreographyTrack track;
    private void GenerateJumpCheckEvent()
    {
        var _koreography = Koreographer.Instance.GetKoreographyAtIndex(0);

        for (int i = 0; i < _mapEventList.Count; i++)
        {
            int[] tileData = _mapEventList[i].GetTextValue().Split().Select(int.Parse).ToArray();

            if (tileData[2] == 1)
            {
                int startSample = _mapEventList[i].StartSample - 5000;
                int endSample = _mapEventList[i].StartSample + 5000;

                KoreographyEvent koreoEvent = new KoreographyEvent();

                CurvePayload curvePayload = new CurvePayload();
                
                koreoEvent.Payload = curvePayload;
                koreoEvent.StartSample = startSample;
                koreoEvent.EndSample = endSample;

                track.AddEvent(koreoEvent);

            }


        }
    }
    
    // private void Awake()
    // {
    //     var _koreography = Koreographer.Instance.GetKoreographyAtIndex(0);
    //
    //     GenerateMap();
    //
    //     _mapEventList = _koreography.GetTrackByID(_mapEventID).GetAllEvents();
    //     for (int i = 0; i < _mapEventList.Count; i++)
    //     {
    //         int[] tileData = _mapEventList[i].GetTextValue().Split().Select(int.Parse).ToArray();
    //         
    //         
    //
    //         if (tileData[2] == 1)
    //         {
    //             Debug.Log(tileData[2]);
    //
    //             var start = _mapEventList[i].StartSample - 5000;
    //             var end = _mapEventList[i].StartSample + 5000;
    //
    //             var trackEvent = track.GetAllEvents()[index];
    //             trackEvent.Payload = new CurvePayload();
    //             Debug.Log(trackEvent.HasCurvePayload());
    //             trackEvent.StartSample = start;
    //             trackEvent.EndSample = end;
    //
    //             index++;
    //         }
    //     }
    // }
    
    private void GenerateMap()
    {
        //_mapEventList = _koreography.GetTrackByID(_mapEventID).GetAllEvents();
        _mapEventList = SoundManager.instance.playingKoreo.GetTrackByID(_mapEventID).GetAllEvents();

        for (int i = 0; i < _mapEventList.Count; i++)
        {
            int[] tileData = _mapEventList[i].GetTextValue().Split().Select(int.Parse).ToArray();

            int groundType = tileData[0];
            int groundYDelta = tileData[1];
            int actionType = tileData[2];
            int checkPoint = tileData[3];

            _tileX += 1;
            _tileY += groundYDelta;

            if (groundType == 3)
            {
                continue;
            }

            // 땅 타일
            _groundTilemap.SetTile(new Vector3Int(_tileX, _tileY, 0), _groundTileList[groundType]);
            for (int j = _tileY - 1; j >= -10; j--)
            {
                if (j == _tileY - 1 && (groundType == 1 || groundType == 2))
                {
                    _groundTilemap.SetTile(new Vector3Int(_tileX, j, 0), _groundTileList[groundType + 3]);
                }
                else
                {
                    _groundTilemap.SetTile(new Vector3Int(_tileX, j, 0), _groundTileList[3]);
                }
            }

            // 액션 타일
            if (actionType != 0)
            {
                float yOffset = (groundType == 0) ? 0.5f : 0f;
                Matrix4x4 tileTransform = Matrix4x4.Translate(new Vector3(0f, yOffset, 0f)) * Matrix4x4.Rotate(Quaternion.identity);
                TileChangeData tileChangeData = new TileChangeData
                {
                    position = new Vector3Int(_tileX, _tileY, 0),
                    tile = _interactionTileList[0],
                    transform = tileTransform
                };

                _interactionTilemap.SetTile(tileChangeData, false);
            }

            // 체크포인트
            if (checkPoint == 1)
            {
                _interactionTilemap.SetTile(new Vector3Int(_tileX, _tileY + 1, 0), _interactionTileList[1]);
            }
        }
    }
}
