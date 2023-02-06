using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using SonicBloom.Koreo;

public class MapTemp : MonoBehaviour
{
    [System.Serializable]
    public class TileSet
    {
        public List<Tile> topTileList = new List<Tile>();
        public List<Tile> underTileList = new List<Tile>();
    }

    public enum Theme
    {
        City, Forest, Desert, Glacier
    }

    public Theme theme;
    [SerializeField] private List<TileSet> _tileSet;

    [Header("Check Track")]
    [SerializeField] KoreographyTrack _jumpCheckTrack;

    [Space][Header("Event")]
    [SerializeField] [EventID] private string _mapEventID;
    [SerializeField] [EventID] private string _actEventID;
    [SerializeField] [EventID] private string _spdEventID;
    [SerializeField] private List<KoreographyEvent> _mapEventList = new List<KoreographyEvent>();
    [SerializeField] private List<KoreographyEvent> _shortEventList = new List<KoreographyEvent>();
    [SerializeField] private List<KoreographyEvent> _spdEventList = new List<KoreographyEvent>();
    [SerializeField] private int _actEventIndex;
    [SerializeField] private int _spdEventIndex;

    [Space][Header("Tilemap")]
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _interactionTilemap;

    [Space][Header("Interaction")]
    [SerializeField] private List<Tile> _interactionTileList = new List<Tile>();

    private int _tileX = -1, _tileY;

    private void Awake()
    {
        LoadAllEvents();
        GenerateMap();
        FillMapSide();

        // 액션 트랙을 확인해 판정용 트랙들을 초기화
        // 판정용 트랙들의 이벤트가 비어있을 때에만 실행
        if (_jumpCheckTrack.GetAllEvents().Count == 0)
        {
            GenerateJumpCheckEvent();
        }
    }

    private void LoadAllEvents()
    {
        _mapEventList = SoundManager.instance.playingKoreo.GetTrackByID(_mapEventID).GetAllEvents();
        _shortEventList = SoundManager.instance.playingKoreo.GetTrackByID(_actEventID).GetAllEvents();
        _spdEventList = SoundManager.instance.playingKoreo.GetTrackByID(_spdEventID).GetAllEvents();
    }

    private void FillMapSide()
    {   
        // 맵 오른쪽 끝 채우기
        for (int i = 0; i < 30; i++)
        {
            _groundTilemap.SetTile(new Vector3Int(++_tileX, _tileY, 0), _tileSet[(int)theme].topTileList[0]);

            for (int j = _tileY - 1; j >= -10; j--)
            {
                _groundTilemap.SetTile(new Vector3Int(_tileX, j, 0), _tileSet[(int)theme].underTileList[0]);
            }
        }

        // 맵 왼쪽 끝 채우기
        for (int i = -1; i >= -15; i--)
        {
            _groundTilemap.SetTile(new Vector3Int(i, 0, 0), _tileSet[(int)theme].topTileList[0]);

            for (int j = -1; j >= -10; j--)
            {
                _groundTilemap.SetTile(new Vector3Int(i, j, 0), _tileSet[(int)theme].underTileList[0]);
            }
        }
    }

    private void GenerateJumpCheckEvent()
    {
        for (int i = 0; i < _shortEventList.Count; i++)
        {
            int actType = _shortEventList[i].GetIntValue();

            if (actType == 0)
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
        int prevGroundType = 0;
        float groundYOffset = 0f;
        int gradualTileCount = 0;

        for (int i = 0; i < _mapEventList.Count; i++)
        {
            float[] groundData = _mapEventList[i].GetTextValue().Split().Select(float.Parse).ToArray();

            int groundType = (int)groundData[0];
            groundYOffset += groundData.Length > 1 ? groundData[1] : 0;

            bool isSideTile = false;
            int groundIndex = groundType;
            int groundYDelta = 0;

            // 경사 타일일 때 타일 위치와 번호 지정
            if ((groundType == 1 || prevGroundType == 2))
            {
                gradualTileCount++;

                if (groundType == 1)
                {
                    if (gradualTileCount == 1)
                    {
                        groundYDelta = 1;
                    }
                    else // gradualTileCount == 2
                    {
                        groundIndex = groundType + 4;
                    }
                }
                else // prevGroundType == 2
                {
                    if (gradualTileCount == 2)
                    {
                        groundYDelta = -1;
                    }
                    else // gradualTileCount == 1
                    {
                        groundIndex = groundType + 4;
                    }
                }

                gradualTileCount %= 2;
            }

            if (groundType == 3)
            {
                groundYDelta = 1;
            }
            else if (prevGroundType == 4)
            {
                groundYDelta = -1;
            }

            _tileX += 1;
            _tileY += groundYDelta;

            // 빈 타일
            if (groundType == 5)
            {
                prevGroundType = groundType;

                continue;
            }

            // 사이드 타일 체크
            // 사이드 타일은 평평한 타일(0)만 올 수 있기로 했기 때문에 현재 타일 타입이 0인지 같이 체크해 준다.
            if (groundType != 5 && i != 0 && i != _mapEventList.Count - 1)
            {
                // 이전 타일이 빈 타일(5)이고 현재 타일 타입이 평평한 타일(0)이면 왼쪽 사이드
                if (prevGroundType == 5 && groundType == 0)
                {
                    isSideTile = true;
                    groundIndex = 7;
                }
                // 다음 타일이 빈 타일(5)이고 현재 타일 타입이 평평한 타일(0)이면 오른쪽 사이드
                else if ((int)(_mapEventList[i + 1].GetTextValue().Split().Select(float.Parse).ToArray()[0]) == 5 && groundType == 0)
                {
                    isSideTile = true;
                    groundIndex = 8;
                }
            }

            // 최종 결정된 타일 위치와 번호로 타일을 배치한다.
            // 최상단 타일 배치
            Matrix4x4 tileTransform = Matrix4x4.Translate(new Vector3(0f, groundYOffset, 0f)) * Matrix4x4.Rotate(Quaternion.identity);
            TileChangeData tileChangeData = new TileChangeData
            {
                position = new Vector3Int(_tileX, _tileY, 0),
                tile = _tileSet[(int)theme].topTileList[groundIndex],
                transform = tileTransform
            };
            
            _groundTilemap.SetTile(tileChangeData, false);

            // 밑 영역 타일들 배치
            for (int j = _tileY - 1; j >= -10; j--)
            {
                if (!isSideTile && j != _tileY - 1)
                {
                    groundIndex = 0;
                }

                tileChangeData = new TileChangeData
                {
                    position = new Vector3Int(_tileX, j, 0),
                    tile = _tileSet[(int)theme].underTileList[groundIndex],
                    transform = tileTransform
                };

                _groundTilemap.SetTile(tileChangeData, false);
            }

            // 이전 타일 타입을 현재 타일 타입으로 갱신
            prevGroundType = groundType;
        }
    }

    //private void GenerateMap()
    //{
    //    _mapEventList = SoundManager.instance.playingKoreo.GetTrackByID(_mapEventID).GetAllEvents();

    //    for (int i = 0; i < _mapEventList.Count; i++)
    //    {
    //        int[] tileData = _mapEventList[i].GetTextValue().Split().Select(int.Parse).ToArray();

    //        int groundType = tileData[0];
    //        int groundYDelta = tileData[1];
    //        int actionType = tileData[2];
    //        int checkPoint = tileData[3];


    //        _tileX += 1;
    //        _tileY += groundYDelta;

    //        if (groundType == 3)
    //        {
    //            continue;
    //        }

    //        // 땅 타일
    //        Tile tile = _topTileList[groundType];
    //        Tile underTile = _topTileList[3];

    //        bool isLeftSide = false;
    //        bool isRightSide = false;
            
    //        if (groundType != 3 && i != 0 && i != _mapEventList.Count - 1)
    //        {
    //            if (_mapEventList[i - 1].GetTextValue().Split().Select(int.Parse).ToArray()[0] == 3)
    //            {
    //                isLeftSide = true;
    //                tile = (groundType == 0) ? _topTileList[6] : tile;
    //                underTile = _topTileList[8];
    //            }
    //            else if (_mapEventList[i + 1].GetTextValue().Split().Select(int.Parse).ToArray()[0] == 3)
    //            {
    //                isRightSide = true;
    //                tile = (groundType == 0) ? _topTileList[7] : tile;
    //                underTile = _topTileList[9];
    //            }
    //        }

    //        _groundTilemap.SetTile(new Vector3Int(_tileX, _tileY, 0), tile);
            
    //        for (int j = _tileY - 1; j >= -10; j--)
    //        {
    //            if (!isLeftSide && !isRightSide && j == _tileY - 1 && (groundType == 1 || groundType == 2))
    //            {
    //                _groundTilemap.SetTile(new Vector3Int(_tileX, j, 0), _topTileList[groundType + 3]);
    //            }
    //            else
    //            {
    //                _groundTilemap.SetTile(new Vector3Int(_tileX, j, 0), underTile);
    //            }
    //        }

    //        // 액션 타일
    //        if (actionType != 0)
    //        {
    //            float yOffset = (groundType == 0) ? 0.5f : 0f;
    //            Matrix4x4 tileTransform = Matrix4x4.Translate(new Vector3(0f, yOffset, 0f)) * Matrix4x4.Rotate(Quaternion.identity);
    //            TileChangeData tileChangeData = new TileChangeData
    //            {
    //                position = new Vector3Int(_tileX, _tileY, 0),
    //                tile = _interactionTileList[0],
    //                transform = tileTransform
    //            };

    //            _interactionTilemap.SetTile(tileChangeData, false);
    //        }

    //        // 체크포인트
    //        if (checkPoint == 1 || checkPoint == 2)
    //        {
    //            _interactionTilemap.SetTile(new Vector3Int(_tileX, _tileY + 1, 0), _interactionTileList[1]);
    //        }
    //    }
    //}
}
