using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using SonicBloom.Koreo;

public class MapTemp : MonoBehaviour
{
    // 주석 필
    //[SerializeField] private Koreography _koreography;
    [SerializeField] KoreographyTrack _jumpCheckTrack;

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
        // 주석 필
        //_koreography = Koreographer.Instance.GetKoreographyAtIndex(0);

        GenerateMap();

        TempGenerateMap();

        if (_jumpCheckTrack.GetAllEvents().Count == 0)
        {
            GenerateJumpCheckEvent();
        }
    }
    
    private void TempGenerateMap()
    {   
        // Right End
        for (int i = 0; i < 30; i++)
        {
            _groundTilemap.SetTile(new Vector3Int(++_tileX, _tileY, 0), _groundTileList[0]);

            for (int j = _tileY - 1; j >= -10; j--)
            {
                _groundTilemap.SetTile(new Vector3Int(_tileX, j, 0), _groundTileList[3]);
            }
        }

        // Left End
        for (int i = -1; i >= -15; i--)
        {
            _groundTilemap.SetTile(new Vector3Int(i, 0, 0), _groundTileList[0]);

            for (int j = -1; j >= -10; j--)
            {
                _groundTilemap.SetTile(new Vector3Int(i, j, 0), _groundTileList[3]);
            }
        }
    }

    private void GenerateJumpCheckEvent()
    {
        //var _koreography = Koreographer.Instance.GetKoreographyAtIndex(0);

        for (int i = 0; i < _mapEventList.Count; i++)
        {
            int[] tileData = _mapEventList[i].GetTextValue().Split().Select(int.Parse).ToArray();

            if (tileData[2] == 1)
            {
                KoreographyEvent koreoEvent = new KoreographyEvent();
                koreoEvent.Payload = new CurvePayload();
                koreoEvent.StartSample = _mapEventList[i].StartSample - 5000;
                koreoEvent.EndSample = _mapEventList[i].StartSample + 5000;

                _jumpCheckTrack.AddEvent(koreoEvent);
            }
        }
    }

    private void GenerateMap()
    {
        // 주석 필
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
            Tile tile = _groundTileList[groundType];
            Tile underTile = _groundTileList[3];

            bool isLeftSide = false;
            bool isRightSide = false;
            
            if (groundType != 3 && i != 0 && i != _mapEventList.Count - 1)
            {
                if (_mapEventList[i - 1].GetTextValue().Split().Select(int.Parse).ToArray()[0] == 3)
                {
                    isLeftSide = true;
                    tile = (groundType == 0) ? _groundTileList[6] : tile;
                    underTile = _groundTileList[8];
                }
                else if (_mapEventList[i + 1].GetTextValue().Split().Select(int.Parse).ToArray()[0] == 3)
                {
                    isRightSide = true;
                    tile = (groundType == 0) ? _groundTileList[7] : tile;
                    underTile = _groundTileList[9];
                }
            }

            _groundTilemap.SetTile(new Vector3Int(_tileX, _tileY, 0), tile);
            
            for (int j = _tileY - 1; j >= -10; j--)
            {
                if (!isLeftSide && !isRightSide && j == _tileY - 1 && (groundType == 1 || groundType == 2))
                {
                    _groundTilemap.SetTile(new Vector3Int(_tileX, j, 0), _groundTileList[groundType + 3]);
                }
                else
                {
                    _groundTilemap.SetTile(new Vector3Int(_tileX, j, 0), underTile);
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
            if (checkPoint == 1 || checkPoint == 2)
            {
                _interactionTilemap.SetTile(new Vector3Int(_tileX, _tileY + 1, 0), _interactionTileList[1]);
            }
        }
    }
}
