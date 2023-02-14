using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using SonicBloom.Koreo;

public class MapTemp : MonoBehaviour
{
    private enum _TileType
    {
        GroundTop, GroundUnder, Interaction
    }
    private enum _GroundType
    {
        Flat, GentleUp, GentleDown, SteepUp, SteepDown, Empty
    }
    public enum Theme
    {
        City, Forest, Desert, Glacier
    }

    [Space]
    [Header("Base")]
    public Theme theme;
    [SerializeField] private bool _isEditMode;

    [Space]
    [Header("Koreographer")]
    [SerializeField] [EventID] private string _mapEventID;
    [SerializeField] [EventID] private string _shortEventID;
    [SerializeField] [EventID] private string _longEventID;
    [SerializeField] [EventID] private string _spdEventID;
    private KoreographyTrackBase _shortTrack;
    private KoreographyTrackBase _longTrack;
    private List<KoreographyEvent> _mapEventList = new List<KoreographyEvent>();
    private List<KoreographyEvent> _shortEventList = new List<KoreographyEvent>();
    private List<KoreographyEvent> _longEventList = new List<KoreographyEvent>();
    private List<KoreographyEvent> _spdEventList = new List<KoreographyEvent>();
    private KoreographyEvent _longEvent;
    private CurvePayload _longPayload;

    [Space]
    [Header("Tile")]
    [SerializeField] private Tilemap _groundTilemap;
    [SerializeField] private Tilemap _interactionTilemap;
    private List<List<Tile>> _tileLists = new List<List<Tile>>();
    private List<Tile> _topTiles = new List<Tile>();
    private List<Tile> _underTiles = new List<Tile>();
    [SerializeField] private List<Tile> _interactionTiles = new List<Tile>();
    private const int _tileCount = 9;
    private int _tileX = -1, _tileY;
    private float _groundYOffset = 0f;
    private LayerMask _tileLayer;

    private MonsterPooling _monsterPooling;
    private ObjectGenerator _objectGenerator;

    private void Awake()
    {
        Init(theme);
        GenerateMap();

        Invoke("GenerateShortNoteTile", 0.1f);
        Invoke("GenerateLongNoteTile", 0.1f);
    }

    // 빌드 시 Edit 부분 지울 것
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _isEditMode = !_isEditMode;
        }

        if (_isEditMode)
        {
            Edit();
        }
    }

    private void Edit()
    {
        // Short Note (Q - Jump, W - Attack)
        if (Input.GetKeyDown(KeyCode.Q) || Input.GetKeyDown(KeyCode.W))
        {
            IntPayload shortPayload = new IntPayload();
            shortPayload.IntVal = Input.GetKeyDown(KeyCode.Q) ? 0 : 1;
            KoreographyEvent shortEvent = new KoreographyEvent();
            shortEvent.Payload = shortPayload;
            shortEvent.StartSample = SoundManager.instance.playingKoreo.GetLatestSampleTime(); ;

            _shortTrack.AddEvent(shortEvent);
        }
        // Long Note Start (E down)
        else if (Input.GetKeyDown(KeyCode.E))
        {
            _longPayload = new CurvePayload();
            _longEvent = new KoreographyEvent();
            _longEvent.Payload = _longPayload;
            _longEvent.StartSample = SoundManager.instance.playingKoreo.GetLatestSampleTime();
        }
        // Long Note End (E up)
        else if (Input.GetKeyUp(KeyCode.E))
        {
            _longEvent.EndSample = SoundManager.instance.playingKoreo.GetLatestSampleTime();

            _longTrack.AddEvent(_longEvent);
        }
    }

    private void Init(Theme theme)
    {
        // 타일 세팅
        string tilePath = $"Tiles/{theme}/{theme}";

        for (int i = 0; i < _tileCount; i++)
        {
            _topTiles.Add(Resources.Load<Tile>($"{tilePath}_Top_{i}"));
            _underTiles.Add(Resources.Load<Tile>($"{tilePath}_Under_{i}"));
        }

        _tileLists.Add(_topTiles);
        _tileLists.Add(_underTiles);
        _tileLists.Add(_interactionTiles);

        // 이벤트 세팅
        _shortTrack = SoundManager.instance.playingKoreo.GetTrackByID(_shortEventID);
        _longTrack = SoundManager.instance.playingKoreo.GetTrackByID(_longEventID);
        
        _shortEventList = _shortTrack.GetAllEvents();
        _longEventList = _longTrack.GetAllEvents();
        _mapEventList = SoundManager.instance.playingKoreo.GetTrackByID(_mapEventID).GetAllEvents();
        _spdEventList = SoundManager.instance.playingKoreo.GetTrackByID(_spdEventID).GetAllEvents();
        
        // 기타 세팅
        _tileLayer = LayerMask.GetMask("Ground");
        _monsterPooling = FindObjectOfType<MonsterPooling>();
        _objectGenerator = GetComponent<ObjectGenerator>();
    }

    private void GenerateMap()
    {
        _GroundType prevGroundType = 0;
        int gentleSlopeCount = 0;

        for (int i = 0; i < _mapEventList.Count; i++)
        {
            float[] groundData = _mapEventList[i].GetTextValue().Split().Select(float.Parse).ToArray();

            _GroundType groundType = (_GroundType)groundData[0];
            _groundYOffset += groundData.Length > 1 ? groundData[1] : 0;

            bool isSideTile = false;
            int groundIndex = (int)groundType;
            int groundYDelta = 0;

            // 빈 타일일 때는 특정 사항들만 처리 후 다음 타일로 넘어간다.
            if (groundType == _GroundType.Empty)
            {
                _tileX += 1;
                _monsterPooling.AddTilePos(_tileX, _tileY);
                _objectGenerator.PositItems(_tileX, _tileY + 2);
                prevGroundType = groundType;

                continue;
            }

            // 경사 타일일 때 타일 위치와 번호 지정
            // 완경사일 때
            if ((groundType == _GroundType.GentleUp || prevGroundType == _GroundType.GentleDown))
            {
                gentleSlopeCount++;

                if (groundType == _GroundType.GentleUp)
                {
                    if (gentleSlopeCount == 1)
                    {
                        groundYDelta = 1;
                    }
                    else // gradualTileCount == 2
                    {
                        groundIndex = (int)groundType + 4;
                    }
                }
                else // prevGroundType == _GroundType.GentleDown
                {
                    if (gentleSlopeCount == 2)
                    {
                        groundYDelta = -1;
                    }
                    else // gradualTileCount == 1
                    {
                        groundIndex = (int)groundType + 4;
                    }
                }

                gentleSlopeCount %= 2;
            }

            // 급경사일 때
            if (groundType == _GroundType.SteepUp)
            {
                groundYDelta = 1;
            }
            else if (prevGroundType == _GroundType.SteepDown)
            {
                groundYDelta = -1;
            }

            _tileX += 1;
            _tileY += groundYDelta;

            // 사이드 타일 체크
            // 사이드 타일은 평평한 타일만 올 수 있기 때문에 현재 타일 타입이 Flat인지 체크
            if (groundType == _GroundType.Flat && i != 0 && i != _mapEventList.Count - 1)
            {
                // 이전 타일이 빈 타일이면 왼쪽 사이드
                if (prevGroundType == _GroundType.Empty)
                {
                    isSideTile = true;
                    groundIndex = 7;
                }
                // 다음 타일이 빈 타일이면 오른쪽 사이드
                else if ((_GroundType)(_mapEventList[i + 1].GetTextValue().Split().Select(float.Parse).ToArray()[0]) == _GroundType.Empty)
                {
                    isSideTile = true;
                    groundIndex = 8;
                }
            }

            // 최종 결정된 타일 위치와 번호로 타일을 배치하고 적 위치를 저장한다.
            // 적 위치 저장
            _monsterPooling.AddTilePos(_tileX, _tileY + _groundYOffset);

            // 최상단 타일 배치
            _groundTilemap.SetTile(GetTileChangeData(_TileType.GroundTop, groundIndex, new Vector3Int(_tileX, _tileY, 0), new Vector3(0f, _groundYOffset, 0f)), false);

            // 밑 영역 타일들 배치
            for (int j = _tileY - 1; j >= -10; j--)
            {
                if (!isSideTile && j != _tileY - 1)
                {
                    groundIndex = 0;
                }

                _groundTilemap.SetTile(GetTileChangeData(_TileType.GroundUnder, groundIndex, new Vector3Int(_tileX, j, 0), new Vector3(0f, _groundYOffset, 0f)), false);
            }

            // 체크포인트 배치
            for (int j = 0; j < _spdEventList.Count; j++)
            {
                if (_spdEventList[j].StartSample - 5 < _mapEventList[i].StartSample && _mapEventList[i].StartSample < _spdEventList[j].StartSample + 5)
                {
                    if (_spdEventList[j].HasFloatPayload() | (_spdEventList[j].GetTextValue() == "End"))
                    {
                        _interactionTilemap.SetTile(GetTileChangeData(_TileType.Interaction, 1, new Vector3Int(_tileX, _tileY + 1, 0), new Vector3(0f, _groundYOffset, 0f)), false);
                        // Locate CheckPoint Animation
                        _objectGenerator.PositCheckPoint(_tileX, _tileY + 1);
                    }
                }
            }

            // 이전 타일 타입을 현재 타일 타입으로 갱신
            prevGroundType = groundType;
        }

        FillMapSide();
    }

    private void FillMapSide()
    {
        //맵 오른쪽 끝 채우기
        for (int i = 0; i < 30; i++)
        {
            _groundTilemap.SetTile(GetTileChangeData(_TileType.GroundTop, 0, new Vector3Int(++_tileX, _tileY, 0), new Vector3(0f, _groundYOffset, 0f)), false);

            for (int j = _tileY - 1; j >= -10; j--)
            {
                _groundTilemap.SetTile(GetTileChangeData(_TileType.GroundUnder, 0, new Vector3Int(_tileX, j, 0), new Vector3(0f, _groundYOffset, 0f)), false);
            }
        }

        // 맵 왼쪽 끝 채우기
        for (int i = -1; i >= -15; i--)
        {
            _groundTilemap.SetTile(GetTileChangeData(_TileType.GroundTop, 0, new Vector3Int(i, 0, 0), Vector3.zero), false);

            for (int j = -1; j >= -10; j--)
            {
                _groundTilemap.SetTile(GetTileChangeData(_TileType.GroundUnder, 0, new Vector3Int(i, j, 0), Vector3.zero), false);
            }
        }
    }

    private void GenerateShortNoteTile()
    {
        int xPosition = 0;

        for (int i = 0; i < _shortEventList.Count; i++)
        {
            int shortSample = _shortEventList[i].StartSample;
            float xOffset = 0f;

            for (int j = xPosition; j < _mapEventList.Count - 1; j++)
            {
                int prevMapSample = _mapEventList[j].StartSample;
                int nextMapSample = _mapEventList[j + 1].StartSample;

                if (shortSample < nextMapSample + 5)
                {
                    xPosition = j;
                    xOffset = (float)(shortSample - prevMapSample) / (nextMapSample - prevMapSample);

                    break;
                }
            }

            //Debug.DrawRay(new Vector2(xPosition + xOffset, 100f), Vector2.down * 1000, Color.yellow, 100f);
            RaycastHit2D shortHit = Physics2D.Raycast(new Vector2(xPosition + xOffset, 100f), Vector2.down, 1000, _tileLayer);

            if (shortHit)
            {
                float yOffset = shortHit.point.y;

                _interactionTilemap.SetTile(GetTileChangeData(_TileType.Interaction, 0, new Vector3Int(xPosition, 0, 0), new Vector3(xOffset, yOffset, 0f)), false);
                //if (_shortEventList[i].GetIntValue() == 0)
                //{
                //    Instantiate(actionEffects[0], new Vector3(xPosition + xOffset, yOffset, 0f), Quaternion.identity);
                //}
                //else if (_shortEventList[i].GetIntValue() == 1)
                //{
                //    Instantiate(actionEffects[1], new Vector3(xPosition + xOffset, yOffset, 0f), Quaternion.identity);
                //}
            }
        }
    }

    private void GenerateLongNoteTile()
    {
        int startXPosition = 0;
        int endXPosition = 0;

        for (int i = 0; i < _longEventList.Count; i++)
        {
            int longStartSample = _longEventList[i].StartSample;
            int longEndSample = _longEventList[i].EndSample;
            float startXOffset = 0f;
            float endXOffset = 0f;

            for (int j = startXPosition; j < _mapEventList.Count - 1; j++)
            {
                int prevMapSample = _mapEventList[j].StartSample;
                int nextMapSample = _mapEventList[j + 1].StartSample;

                if (longStartSample < nextMapSample + 5)
                {
                    startXPosition = j;
                    startXOffset = (float)(longStartSample - prevMapSample) / (nextMapSample - prevMapSample);

                    break;
                }
            }

            for (int j = endXPosition; j < _mapEventList.Count - 1; j++)
            {
                int prevMapSample = _mapEventList[j].StartSample;
                int nextMapSample = _mapEventList[j + 1].StartSample;

                if (longEndSample < nextMapSample + 5)
                {
                    endXPosition = j;
                    endXOffset = (float)(longEndSample - prevMapSample) / (nextMapSample - prevMapSample);

                    break;
                }
            }

            //Debug.DrawRay(new Vector2(startXPosition + startXOffset, 100f), Vector2.down * 1000, Color.red, 100f);
            RaycastHit2D longStartHit = Physics2D.Raycast(new Vector2(startXPosition + startXOffset, 100f), Vector2.down, 1000, _tileLayer);

            if (longStartHit)
            {
                float yOffset = longStartHit.point.y;
                
                _interactionTilemap.SetTile(GetTileChangeData(_TileType.Interaction, 2, new Vector3Int(startXPosition, 0, 0), new Vector3(startXOffset, yOffset, 0f)), false);
                //Instantiate(actionEffects[2], new Vector3(startXPosition + startXOffset, yOffset, 0f), Quaternion.identity);
            }

            //Debug.DrawRay(new Vector2(endXPosition + endXOffset, 100f), Vector2.down * 1000, Color.blue, 100f);
            RaycastHit2D longEndHit = Physics2D.Raycast(new Vector2(endXPosition + endXOffset, 100f), Vector2.down, 1000, _tileLayer);

            if (longEndHit)
            {
                float yOffset = longEndHit.point.y;
                
                _interactionTilemap.SetTile(GetTileChangeData(_TileType.Interaction, 2, new Vector3Int(endXPosition, 0, 0), new Vector3(endXOffset, yOffset, 0f)), false);
                //Instantiate(actionEffects[2], new Vector3(endXPosition + endXOffset, yOffset, 0f), Quaternion.identity);
            }
        }
    }

    private TileChangeData GetTileChangeData(_TileType type, int index, Vector3Int position, Vector3 offset)
    {
        Matrix4x4 tileTransform = Matrix4x4.Translate(offset) * Matrix4x4.Rotate(Quaternion.identity);
        TileChangeData tileChangeData = new TileChangeData
        {
            position = position,
            tile = _tileLists[(int)type][index],
            transform = tileTransform
        };

        return tileChangeData;
    }
}
