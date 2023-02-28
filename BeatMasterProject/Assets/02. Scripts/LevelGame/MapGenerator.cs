using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;
using SonicBloom.Koreo;

public class MapGenerator : MonoBehaviour
{
    public enum Theme
    {
        City, Forest, Desert, Glacier
    }
    private enum _TileType
    {
        GroundTop, GroundUnder, Interaction
    }
    private enum _GroundType
    {
        Flat, GentleUp, GentleDown, SteepUp, SteepDown, Empty, LeftSide = 7, RightSide = 8
    }
    private enum _NoteType
    {
        Jump, Attack, Long, LongMid
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
    private List<Tile> _interactionTiles = new List<Tile>();
    private List<GameObject> _noteObjects = new List<GameObject>();
    private const int _tileCount = 9;
    private const int _noteObjectCount = 4;
    private const int _minTileY = -30;
    private int _tileX = -1, _tileY;
    private float _groundYOffset = 0f;
    private LayerMask _tileLayer;

    [Header("Objects")]
    private MonsterPooling _monsterPooling;
    private ObjectGenerator _objectGenerator;

    [HideInInspector] public List<Note> shortTileParticleList = new List<Note>();
    [HideInInspector] public List<Note> longTileParticleList = new List<Note>();
    private WaitForSeconds _waitForSec;

    private void Awake()
    {
        Init(theme);
        GenerateMap();
        StartCoroutine(CoGenerateNoteObject());
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
        string tilePath = $"Tile/{theme}/{theme}";

        for (int i = 0; i < _tileCount; i++)
        {
            _topTiles.Add(Resources.Load<Tile>($"{tilePath}_Top_{i}"));
            _underTiles.Add(Resources.Load<Tile>($"{tilePath}_Under_{i}"));
        }
        _interactionTiles.Add(Resources.Load<Tile>($"Tile/Interaction/Flag"));

        _tileLists.Add(_topTiles);
        _tileLists.Add(_underTiles);
        _tileLists.Add(_interactionTiles);

        // 노트 오브젝트 세팅
        for (int i = 0; i < _noteObjectCount; i++)
        {
            _noteObjects.Add(Resources.Load<GameObject>($"Note/{(_NoteType)i}"));
        }

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
        _waitForSec = new WaitForSeconds(0.01f);
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
                _objectGenerator.PositItems(_tileX, _tileY + _groundYOffset + 2); // posit star item
                prevGroundType = groundType;

                continue;
            }

            // 경사 타일일 때 타일 위치와 번호 지정
            // 완경사일 때
            if ((groundType == _GroundType.GentleUp || prevGroundType == _GroundType.GentleDown))
            {
                if (groundType == _GroundType.GentleUp)
                {
                    gentleSlopeCount++;

                    if (gentleSlopeCount == 1)
                    {
                        groundYDelta += 1;
                    }
                    else // gentleSlopeCount == 2
                    {
                        groundIndex = (int)groundType + 4;
                    }
                }

                if (prevGroundType == _GroundType.GentleDown)
                {
                    gentleSlopeCount++;

                    if (gentleSlopeCount == 2)
                    {
                        groundYDelta += -1;
                    }
                    else // gentleSlopeCount == 1
                    {
                        groundIndex = groundType == _GroundType.GentleUp ? (int)groundType : (int)groundType + 4;
                    }
                }

                gentleSlopeCount %= 2;
            }

            // 급경사일 때
            if (groundType == _GroundType.SteepUp)
            {
                groundYDelta += 1;
            }
            
            if (prevGroundType == _GroundType.SteepDown)
            {
                groundYDelta += -1;
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
                    groundIndex = (int)_GroundType.LeftSide;
                }
                // 다음 타일이 빈 타일이면 오른쪽 사이드
                else if ((_GroundType)(_mapEventList[i + 1].GetTextValue().Split().Select(float.Parse).ToArray()[0]) == _GroundType.Empty)
                {
                    isSideTile = true;
                    groundIndex = (int)_GroundType.RightSide;
                }
            }

            // 최종 결정된 타일 위치와 번호로 타일을 배치
            // 최상단 타일 배치
            _groundTilemap.SetTile(GetTileChangeData(_TileType.GroundTop, groundIndex,
                new Vector3Int(_tileX, _tileY, 0), new Vector3(0f, _groundYOffset, 0f)), false);

            // 밑 영역 타일들 배치
            for (int j = _tileY - 1; j >= _minTileY; j--)
            {
                if (!isSideTile && j != _tileY - 1)
                {
                    groundIndex = 0;
                }

                _groundTilemap.SetTile(GetTileChangeData(_TileType.GroundUnder, groundIndex,
                    new Vector3Int(_tileX, j, 0), new Vector3(0f, _groundYOffset, 0f)), false);
            }

            // 체크포인트 배치
            for (int j = 0; j < _spdEventList.Count; j++)
            {
                if (_spdEventList[j].StartSample - 5 < _mapEventList[i].StartSample && _mapEventList[i].StartSample < _spdEventList[j].StartSample + 5)
                {
                    if (_spdEventList[j].HasFloatPayload() || (_spdEventList[j].GetTextValue() == "End"))
                    {
                        _interactionTilemap.SetTile(GetTileChangeData(_TileType.Interaction, 0,
                            new Vector3Int(_tileX, _tileY + 1, 0), new Vector3(0f, _groundYOffset, 0f)), false);
                        // Record CheckPoint Animation
                        _objectGenerator.RecordCheckPoint(_tileX, _tileY + _groundYOffset + 1f);
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
            _groundTilemap.SetTile(GetTileChangeData(_TileType.GroundTop, 0,
                new Vector3Int(++_tileX, _tileY, 0), new Vector3(0f, _groundYOffset, 0f)), false);

            for (int j = _tileY - 1; j >= _minTileY; j--)
            {
                _groundTilemap.SetTile(GetTileChangeData(_TileType.GroundUnder, 0,
                    new Vector3Int(_tileX, j, 0), new Vector3(0f, _groundYOffset, 0f)), false);
            }
        }

        // 맵 왼쪽 끝 채우기
        for (int i = -1; i >= -15; i--)
        {
            _groundTilemap.SetTile(GetTileChangeData(_TileType.GroundTop, 0,
                new Vector3Int(i, 0, 0), Vector3.zero), false);

            for (int j = -1; j >= -10; j--)
            {
                _groundTilemap.SetTile(GetTileChangeData(_TileType.GroundUnder, 0,
                    new Vector3Int(i, j, 0), Vector3.zero), false);
            }
        }
    }

    private void GenerateShortNoteObject()
    {
        int xPosition = 0;
        float yPosition = 0;

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
                yPosition = shortHit.point.y;
                var noteObj = Instantiate(_noteObjects[_shortEventList[i].GetIntValue()], new Vector3(xPosition + xOffset, yPosition, 0f), Quaternion.identity, transform);
                shortTileParticleList.Add(noteObj.GetComponent<Note>());
                
                // 몬스터 위치와 타입 저장
                // Default는 지상 몬스터
                _monsterPooling.AddMonsterInfo(xPosition + xOffset, yPosition, MonsterPooling.MonsterType.Ground);
                
                // 롱 노트 구간에는 공중 몬스터로 변경
                for (int j = 0; j < _longEventList.Count; j++)
                {
                    // 롱 노트 구간 확인용 값, 롱 노트 구간에는 0과 1사이 값을 가진다.
                    float checkInLong = _longEventList[j].GetEventDeltaAtSampleTime(shortSample);

                    if (checkInLong > 0 && checkInLong < 1)
                    {
                        _monsterPooling.ChangeMonsterType(xPosition + xOffset, yPosition, MonsterPooling.MonsterType.Air);
                        
                        break;
                    }
                }
            }

            if (_shortEventList[i].GetIntValue() == 0)
            {
                // Record jump position
                _objectGenerator.RecordJumpPos(new Vector3(xPosition + xOffset, yPosition, 0));
                if (int.Parse(_mapEventList[xPosition + 2].GetTextValue()) != 5)
                {
                    // Posit object only when next tile is not empty tile
                    _objectGenerator.PositObstacles(xPosition + xOffset, yPosition);
                }
            }
        }
    }


    private void GenerateLongNoteObject()
    {
        int startXPosition = 0;
        int endXPosition = 0;
        float startYPosition = 0f;
        float endYPosition = 0f;

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

                if (longStartSample < nextMapSample + 1)
                {
                    startXPosition = j;
                    startXOffset = (float)(longStartSample - prevMapSample) / (nextMapSample - prevMapSample);

                    for (int k = startXPosition; k < _mapEventList.Count - 1; k++)
                    {
                        int endPrevMapSample = _mapEventList[k].StartSample;
                        int endNextMapSample = _mapEventList[k + 1].StartSample;

                        if (longEndSample < endNextMapSample + 1)
                        {
                            endXPosition = k;
                            endXOffset = (float)(longEndSample - endPrevMapSample) / (endNextMapSample - endPrevMapSample);

                            break;
                        }
                    }

                    RaycastHit2D longStartHit = Physics2D.Raycast(new Vector2(startXPosition + startXOffset, 100f), Vector2.down, 1000, _tileLayer);

                    if (longStartHit)
                    {
                        startYPosition = longStartHit.point.y;

                        Instantiate(_noteObjects[(int)_NoteType.Long],
                            new Vector3(startXPosition + startXOffset, startYPosition, 0f), Quaternion.identity, transform);
                        _objectGenerator.RecordLongPos(new Vector3(startXPosition + startXOffset, startYPosition, 0));
                    }

                    RaycastHit2D longEndHit = Physics2D.Raycast(new Vector2(endXPosition + endXOffset, 100f), Vector2.down, 1000, _tileLayer);

                    if (longEndHit)
                    {
                        endYPosition = longEndHit.point.y;
                        var noteObj = Instantiate(_noteObjects[(int)_NoteType.Long], new Vector3(endXPosition + endXOffset, endYPosition, 0f), Quaternion.identity, transform);
                        longTileParticleList.Add(noteObj.GetComponent<Note>());
                        _objectGenerator.RecordLongPos(new Vector3(endXPosition + endXOffset, endYPosition, 0));
                    }

                    // 롱 노트 시작과 끝 중간에 오브젝트들 생성
                    float longDistance = (endXPosition + endXOffset) - (startXPosition + startXOffset);
                    float longMidCount = Mathf.Ceil(longDistance);
                    float longMidGap = longDistance / (longMidCount + 1);

                    for (int k = 1; k <= longMidCount; k++)
                    {
                        float midXPosition = startXPosition + startXOffset + (longMidGap * k);
                        RaycastHit2D longMidHit = Physics2D.Raycast(new Vector2(midXPosition, 100f), Vector2.down, 1000, _tileLayer);

                        if (longMidHit)
                        {
                            float midYPosition = longMidHit.point.y;

                            Instantiate(_noteObjects[(int)_NoteType.LongMid],
                                new Vector3(midXPosition, midYPosition, 0f), Quaternion.identity, transform);
                        }
                    }

                    // 원하는 롱 노트 구간 타일 투명화
                    if (_longEventList[i].GetCurveValue().Evaluate(0) != _longEventList[i].GetCurveValue().Evaluate(1))
                    {
                        MakeTransparentLongNote(startXPosition, startXOffset, endXPosition, endXOffset);
                    }

                    break;
                }
            }
        }
    }

    private void MakeTransparentLongNote(int startXPosition, float startXOffset, int endXPosition, float endXOffset)
    {
        // 투명하게 할 롱 노트 구간의 시작점과 끝점을 찾는다.
        int startXCeil = Mathf.CeilToInt(startXPosition + startXOffset);

        if ((Mathf.Round(startXOffset * 100) / 100) % 1 >= 0.199f || (Mathf.Round(startXOffset * 100) / 100) == 1)
        {
            startXCeil += 1;
        }

        int endXFloor = Mathf.FloorToInt(endXPosition + endXOffset);

        if ((Mathf.Round(endXOffset * 100) / 100) % 1 <= 0.801f)
        {
            endXFloor -= 1;
        }

        // 시작과 끝 사이 구간을 투명하게 한다.
        for (int k = startXCeil; k <= endXFloor; k++)
        {
            float midY = 0;
            RaycastHit2D midHit = Physics2D.Raycast(new Vector2(k, 100f), Vector2.down, 1000, _tileLayer);
            
            if (midHit)
            {
                midY = midHit.point.y;
            }

            for (float l = midY + 3; l >= _minTileY - 1; l--)
            {
                Vector3Int midPosition = _groundTilemap.WorldToCell(new Vector3(k, l, 0f));
                Color midColor = new Color(1f, 1f, 1f, 0f);

                _groundTilemap.SetTileFlags(midPosition, TileFlags.None);
                _groundTilemap.SetColor(midPosition, midColor);
            }
        }

        // 롱 노트 시작의 전 타일과 끝의 뒤 타일을 사이드 타일로 바꿔 준다.
        // 시작
        float sideY = 0;
        float sideYOffset = 0f;
        RaycastHit2D sideHit = Physics2D.Raycast(new Vector2(startXCeil - 1, 100f), Vector2.down, 1000, _tileLayer);

        if (sideHit)
        {
            sideY = sideHit.point.y;
            sideYOffset = sideY - (int)sideY;
            sideYOffset = (sideYOffset < 0) ? (sideYOffset + 0.5f) : (sideYOffset - 0.5f);
        }

        Vector3Int sidePosition = _groundTilemap.WorldToCell(new Vector3(startXCeil - 1, sideY, 0f));

        _groundTilemap.SetTile(GetTileChangeData(_TileType.GroundTop, (int)_GroundType.RightSide,
                sidePosition, new Vector3(0f, sideYOffset, 0f)), false);

        for (float k = sideY - 1; k >= _minTileY - 1; k--)
        {
            sidePosition = _groundTilemap.WorldToCell(new Vector3(startXCeil - 1, k, 0f));
            _groundTilemap.SetTile(sidePosition, _underTiles[(int)_GroundType.RightSide]);
        }

        // 끝
        sideHit = Physics2D.Raycast(new Vector2(endXFloor + 1, 100f), Vector2.down, 1000, _tileLayer);

        if (sideHit)
        {
            sideY = sideHit.point.y;
            sideYOffset = sideY - (int)sideY;
            sideYOffset = (sideYOffset < 0) ? (sideYOffset + 0.5f) : (sideYOffset - 0.5f);
        }

        sidePosition = _groundTilemap.WorldToCell(new Vector3(endXFloor + 1, sideY, 0f));

        _groundTilemap.SetTile(GetTileChangeData(_TileType.GroundTop, (int)_GroundType.LeftSide,
                sidePosition, new Vector3(0f, sideYOffset, 0f)), false);

        for (float k = sideY - 1; k >= _minTileY - 1; k--)
        {
            sidePosition = _groundTilemap.WorldToCell(new Vector3(endXFloor + 1, k, 0f));
            _groundTilemap.SetTile(sidePosition, _underTiles[(int)_GroundType.LeftSide]);
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

    private IEnumerator CoGenerateNoteObject()
    {
        yield return _waitForSec;

        GenerateShortNoteObject();
        GenerateLongNoteObject();
    }
}
