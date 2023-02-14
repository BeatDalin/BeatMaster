using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using SonicBloom.Koreo;

public class MapGenerator : MonoBehaviour
{
    private enum _TileType
    {
        Top, Under, Interaction
    }
    public enum Theme
    {
        City, Forest, Desert, Glacier
    }
    public Theme theme;

    [Space]
    [Header("Event")]
    [SerializeField] [EventID] private string _mapEventID;
    [SerializeField] [EventID] private string _shortEventID;
    [SerializeField] [EventID] private string _spdEventID;
    [SerializeField] private List<KoreographyEvent> _mapEventList = new List<KoreographyEvent>();
    [SerializeField] private List<KoreographyEvent> _shortEventList = new List<KoreographyEvent>();
    [SerializeField] private List<KoreographyEvent> _spdEventList = new List<KoreographyEvent>();
    [SerializeField] private int _shortEventIndex;
    [SerializeField] private int _spdEventIndex;

    [Space]
    [Header("Tile")]
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _interactionTilemap;
    private List<List<Tile>> _tileLists = new List<List<Tile>>();
    private List<Tile> _topTiles = new List<Tile>();
    private List<Tile> _underTiles = new List<Tile>();
    [SerializeField] private List<Tile> _interactionTiles = new List<Tile>();
    private const int _tileCount = 9;

    private MonsterPooling _monsterPooling;
    private int _tileX = -1, _tileY;
    private float _groundYOffset = 0f;
    
    [Space]
    [Header("Object Generator")]
    [SerializeField] private ObjectGenerator _objectGenerator;
    [SerializeField] [EventID] private string _longEventID;
    [SerializeField] private int _longTrackIdx = 0;
    [SerializeField] private List<KoreographyEvent> _longEventTrack;

    private void Awake()
    {
        _objectGenerator = GetComponent<ObjectGenerator>();
        _longEventTrack = SoundManager.instance.playingKoreo.GetTrackByID(_longEventID).GetAllEvents();

        Init(theme);
        GenerateMap();
        _objectGenerator.PositLongNotify();
    }

    private void Init(Theme theme)
    {
        // 타일 세팅
        string tilePath = "Tiles/" + theme.ToString() + "/" + theme.ToString();

        for (int i = 0; i < _tileCount; i++)
        {
            _topTiles.Add(Resources.Load<Tile>(tilePath + "_Top_" + i.ToString()));
            _underTiles.Add(Resources.Load<Tile>(tilePath + "_Under_" + i.ToString()));
        }

        _tileLists.Add(_topTiles);
        _tileLists.Add(_underTiles);
        _tileLists.Add(_interactionTiles);

        // 이벤트 세팅
        _mapEventList = SoundManager.instance.playingKoreo.GetTrackByID(_mapEventID).GetAllEvents();
        _shortEventList = SoundManager.instance.playingKoreo.GetTrackByID(_shortEventID).GetAllEvents();
        _spdEventList = SoundManager.instance.playingKoreo.GetTrackByID(_spdEventID).GetAllEvents();

        // 기타 세팅
        _monsterPooling = FindObjectOfType<MonsterPooling>();
    }

    private void GenerateMap()
    {
        int prevGroundType = 0;
        int gentleSlopeCount = 0;

        for (int i = 0; i < _mapEventList.Count; i++)
        {
            float[] groundData = _mapEventList[i].GetTextValue().Split().Select(float.Parse).ToArray();

            int groundType = (int)groundData[0];
            _groundYOffset += groundData.Length > 1 ? groundData[1] : 0;

            bool isSideTile = false;
            int groundIndex = groundType;
            int groundYDelta = 0;

            // 빈 타일(5)일 때는 특정 사항들만 처리 후 다음 타일로 넘어간다.
            if (groundType == 5)
            {
                _tileX += 1;

                _monsterPooling.AddTilePos(_tileX, _tileY);

                prevGroundType = groundType;
                _objectGenerator.PositItems(_tileX, _tileY + 2);

                continue;
            }

            // 경사 타일(1, 2, 3, 4)일 때 타일 위치와 번호 지정
            // 완경사(1, 2)일 때
            if (groundType == 1 || prevGroundType == 2)
            {
                gentleSlopeCount++;

                if (groundType == 1)
                {
                    if (gentleSlopeCount == 1)
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
                    if (gentleSlopeCount == 2)
                    {
                        groundYDelta = -1;
                    }
                    else // gradualTileCount == 1
                    {
                        groundIndex = groundType + 4;
                    }
                }

                gentleSlopeCount %= 2;
            }

            // 급경사(3, 4)일 때
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

            // 사이드 타일 체크
            // 사이드 타일은 평평한 타일(0)만 올 수 있기 때문에 현재 타일 타입이 0인지 체크
            if (groundType == 0 && i != 0 && i != _mapEventList.Count - 1)
            {
                // 이전 타일이 빈 타일(5)이면 왼쪽 사이드
                if (prevGroundType == 5)
                {
                    isSideTile = true;
                    groundIndex = 7;
                }
                // 다음 타일이 빈 타일(5)이면 오른쪽 사이드
                else if ((int)(_mapEventList[i + 1].GetTextValue().Split().Select(float.Parse).ToArray()[0]) == 5)
                {
                    isSideTile = true;
                    groundIndex = 8;
                }
            }

            // 최종 결정된 타일 위치와 번호로 타일을 배치하고 적 위치를 저장한다.
            // 적 위치 저장
            _monsterPooling.AddTilePos(_tileX, _tileY + _groundYOffset);

            // 최상단 타일 배치
            _groundTilemap.SetTile(GetTileChangeData(_TileType.Top, groundIndex, new Vector3Int(_tileX, _tileY, 0), _groundYOffset), false);

            // 밑 영역 타일들 배치
            for (int j = _tileY - 1; j >= -10; j--)
            {
                if (!isSideTile && j != _tileY - 1)
                {
                    groundIndex = 0;
                }

                _groundTilemap.SetTile(GetTileChangeData(_TileType.Under, groundIndex, new Vector3Int(_tileX, j, 0), _groundYOffset), false);
            }

            // (임시) 숏 노트 타일 배치
            for (int j = 0; j < _shortEventList.Count; j++)
            {
                if (_shortEventList[j].StartSample - 5 < _mapEventList[i].StartSample && _mapEventList[i].StartSample < _shortEventList[j].StartSample + 5)
                {
                    float shortYOffset = 1f;

                    switch (groundType)
                    {
                        case 1:
                        case 2:
                            shortYOffset = 0.5f;
                            break;
                        case 3:
                        case 4:
                            shortYOffset = 0.25f;
                            break;
                    }

                    _interactionTilemap.SetTile(GetTileChangeData(_TileType.Interaction, 0, new Vector3Int(_tileX, _tileY, 0), _groundYOffset + shortYOffset), false);

                    break;
                }
            }

            // 체크포인트 배치
            for (int j = 0; j < _spdEventList.Count; j++)
            {
                if (_spdEventList[j].StartSample - 5 < _mapEventList[i].StartSample && _mapEventList[i].StartSample < _spdEventList[j].StartSample + 5)
                {
                    if (_spdEventList[j].HasFloatPayload() || (_spdEventList[j].GetTextValue() == "End"))
                    {
                        _interactionTilemap.SetTile(GetTileChangeData(_TileType.Interaction, 1, new Vector3Int(_tileX, _tileY + 1, 0), _groundYOffset), false);
                        // Locate CheckPoint Animation
                        _objectGenerator.RecordCheckPoint(_tileX, _tileY+1);
                    }
                }
            }

            // 이전 타일 타입을 현재 타일 타입으로 갱신
            prevGroundType = groundType;
            
            // Record Long Note's Start, End position
            if (_longTrackIdx < _longEventTrack.Count)
            {
                RecordLongPos(_mapEventList[i].StartSample, _longEventTrack[_longTrackIdx], _tileX, _tileY+1);
            }
        }

        // 맵 오른쪽 끝 채우기
        for (int i = 0; i < 30; i++)
        {
            _groundTilemap.SetTile(GetTileChangeData(_TileType.Top, 0, new Vector3Int(++_tileX, _tileY, 0), _groundYOffset), false);

            for (int j = _tileY - 1; j >= -10; j--)
            {
                _groundTilemap.SetTile(GetTileChangeData(_TileType.Under, 0, new Vector3Int(_tileX, j, 0), _groundYOffset), false);
            }
        }

        // 맵 왼쪽 끝 채우기
        for (int i = -1; i >= -15; i--)
        {
            _groundTilemap.SetTile(GetTileChangeData(_TileType.Top, 0, new Vector3Int(i, 0, 0), 0), false);

            for (int j = -1; j >= -10; j--)
            {
                _groundTilemap.SetTile(GetTileChangeData(_TileType.Under, 0, new Vector3Int(i, j, 0), 0), false);
            }
        }
    }

    private TileChangeData GetTileChangeData(_TileType type, int index, Vector3Int position, float yOffset)
    {
        Matrix4x4 tileTransform = Matrix4x4.Translate(new Vector3(0f, yOffset, 0f)) * Matrix4x4.Rotate(Quaternion.identity);
        TileChangeData tileChangeData = new TileChangeData
        {
            position = position,
            tile = _tileLists[(int)type][index],
            transform = tileTransform
        };

        return tileChangeData;
    }

    private int tempCount = 1;
    private void RecordLongPos(int mapSample, KoreographyEvent longEvent, int xPos, int yPos)
    {
        // Debug.Log($"LongIdx {_longTrackIdx}-{tempCount}, mapSample {mapSample} // longEvent start:{longEvent.StartSample}, end:{longEvent.EndSample}");
        if (mapSample > longEvent.StartSample - 5 && mapSample < longEvent.StartSample + 5)
        {
            _objectGenerator.RecordLongPos(new Vector3Int(xPos, yPos, 0));
            tempCount++;
        }
        else if (mapSample > longEvent.EndSample - 5 && mapSample < longEvent.EndSample + 5)
        {
            _objectGenerator.RecordLongPos(new Vector3(xPos, yPos, 0));
            _longTrackIdx++;
            tempCount--;
        }
    }
}
