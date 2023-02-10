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
        Top, Under, Interaction
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
    [SerializeField] [EventID] private string _checkPointEventID;
    private KoreographyTrackBase _shortTrack;
    private KoreographyTrackBase _longTrack;
    private List<KoreographyEvent> _mapEventList = new List<KoreographyEvent>();
    private List<KoreographyEvent> _shortEventList = new List<KoreographyEvent>();
    private List<KoreographyEvent> _longEventList = new List<KoreographyEvent>();
    private List<KoreographyEvent> _spdEventList = new List<KoreographyEvent>();
    private List<KoreographyEvent> _checkPointEventList = new List<KoreographyEvent>();
    private int _division = 1;
    CurvePayload _longPayload;
    KoreographyEvent _longEvent;

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

        //GenerateShortNoteTile();
        Invoke("GenerateShortNoteTile", 0.1f);
        Invoke("GenerateLongNoteTile", 0.1f);
    }

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
        _shortTrack = SoundManager.instance.playingKoreo.GetTrackByID(_shortEventID);
        _longTrack = SoundManager.instance.playingKoreo.GetTrackByID(_longEventID);
        
        _shortEventList = _shortTrack.GetAllEvents();
        _longEventList = _longTrack.GetAllEvents();
        _mapEventList = SoundManager.instance.playingKoreo.GetTrackByID(_mapEventID).GetAllEvents();
        _spdEventList = SoundManager.instance.playingKoreo.GetTrackByID(_spdEventID).GetAllEvents();
        _checkPointEventList = SoundManager.instance.playingKoreo.GetTrackByID(_checkPointEventID).GetAllEvents();

        // 기타 세팅
        _tileLayer = LayerMask.GetMask("Ground");
        //_monsterPooling = FindObjectOfType<MonsterPooling>();
        _objectGenerator = GetComponent<ObjectGenerator>();
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

                //_monsterPooling.AddTilePos(_tileX, _tileY);

                prevGroundType = groundType;
                _objectGenerator.PositItems(_tileX, _tileY + 2);

                continue;
            }

            // 경사 타일(1, 2, 3, 4)일 때 타일 위치와 번호 지정
            // 완경사(1, 2)일 때
            if ((groundType == 1 || prevGroundType == 2))
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
            //_monsterPooling.AddTilePos(_tileX, _tileY + _groundYOffset);

            // 최상단 타일 배치
            _groundTilemap.SetTile(GetTileChangeData(_TileType.Top, groundIndex, new Vector3Int(_tileX, _tileY, 0), new Vector3(0f, _groundYOffset, 0f)), false);

            // 밑 영역 타일들 배치
            for (int j = _tileY - 1; j >= -10; j--)
            {
                if (!isSideTile && j != _tileY - 1)
                {
                    groundIndex = 0;
                }

                _groundTilemap.SetTile(GetTileChangeData(_TileType.Under, groundIndex, new Vector3Int(_tileX, j, 0), new Vector3(0f, _groundYOffset, 0f)), false);
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

        // 맵 오른쪽 끝 채우기
        //for (int i = 0; i < 30; i++)
        //{
        //    _groundTilemap.SetTile(GetTileChangeData(_TileType.Top, 0, new Vector3Int(++_tileX, _tileY, 0), _groundYOffset), false);

        //    for (int j = _tileY - 1; j >= -10; j--)
        //    {
        //        _groundTilemap.SetTile(GetTileChangeData(_TileType.Under, 0, new Vector3Int(_tileX, j, 0), _groundYOffset), false);
        //    }
        //}

        //// 맵 왼쪽 끝 채우기
        //for (int i = -1; i >= -15; i--)
        //{
        //    _groundTilemap.SetTile(GetTileChangeData(_TileType.Top, 0, new Vector3Int(i, 0, 0), 0), false);

        //    for (int j = -1; j >= -10; j--)
        //    {
        //        _groundTilemap.SetTile(GetTileChangeData(_TileType.Under, 0, new Vector3Int(i, j, 0), 0), false);
        //    }
        //}
    }

    private void GenerateShortNoteTile()
    {
        int checkPointIndex = 0;
        int tileX = 0;

        for (int i = 0; i < _shortEventList.Count; i++)
        {
            // CheckPoint 트랙의 Event에 입력한 division을 가져온다.
            // 가져온 division 기준으로 현재 Short Event에서 가장 가까운 비트를 먼저 찾고,
            // 해당 비트가 Map 트랙의 어디인지 Map 트랙을 돌면서 일치하는 이벤트를 찾는다.
            // 찾은 이벤트의 인덱스가 시작 지점
            // Short 노트 x 위치 = 시작 지점 + ((shortSample - nearestSample) / SamplesPerBeat(div))

            for (int j = checkPointIndex; j >= 0; j--)
            {
                if (_longEventList[i].StartSample > _checkPointEventList[j].StartSample - 5)
                {
                    _division = _checkPointEventList[j].GetIntValue();

                    break;
                }
            }

            int shortEventSample = _shortEventList[i].StartSample;
            int nearestBeatSample = SoundManager.instance.playingKoreo.GetSampleOfNearestBeat(shortEventSample, _division);
            double samplesPerBeat = SoundManager.instance.playingKoreo.GetSamplesPerBeat(0, _division);
            float xOffset = 0f;

            for (int j = tileX; j < _mapEventList.Count; j++)
            {
                if (_mapEventList[j].StartSample - 5 < nearestBeatSample && nearestBeatSample < _mapEventList[j].StartSample + 5)
                {
                    tileX = j;
                    xOffset = (float)((shortEventSample - nearestBeatSample) / samplesPerBeat);

                    break;
                }
            }

            Debug.DrawRay(new Vector2(tileX + xOffset, 100f), Vector2.down * 1000, Color.yellow, 100f);

            RaycastHit2D shortHit = Physics2D.Raycast(new Vector2(tileX + xOffset, 100f), Vector2.down, 1000, _tileLayer);

            if (shortHit)
            {
                float yOffset = shortHit.point.y;

                _interactionTilemap.SetTile(GetTileChangeData(_TileType.Interaction, 0, new Vector3Int(tileX, 0, 0), new Vector3(xOffset, yOffset, 0f)), false);
            }
        }
    }

    private void GenerateLongNoteTile()
    {
        int checkPointIndex = _checkPointEventList.Count - 1;
        int startTileX = 0;
        int endTileX = 0;

        for (int i = 0; i < _longEventList.Count; i++)
        {
            for (int j = checkPointIndex; j >= 0; j--)
            {
                if (_longEventList[i].StartSample > _checkPointEventList[j].StartSample - 5)
                {
                    _division = _checkPointEventList[j].GetIntValue();

                    break;
                }
            }

            if (_longEventList[i].StartSample > _checkPointEventList[checkPointIndex].StartSample - 5)
            {
                _division = _checkPointEventList[checkPointIndex].GetIntValue();
                
                if (checkPointIndex != 0)
                {
                    checkPointIndex--;
                }
            }

            int longEventStartSample = _longEventList[i].StartSample;
            int longEventEndSample = _longEventList[i].EndSample;
            int startNearestBeatSample = SoundManager.instance.playingKoreo.GetSampleOfNearestBeat(longEventStartSample, _division);
            int endNearestBeatSample = SoundManager.instance.playingKoreo.GetSampleOfNearestBeat(longEventEndSample, _division);
            double samplesPerBeat = SoundManager.instance.playingKoreo.GetSamplesPerBeat(0, _division);
            float startXOffset = 0f;
            float endXOffset = 0f;

            for (int j = startTileX; j < _mapEventList.Count; j++)
            {
                if (_mapEventList[j].StartSample - 5 < startNearestBeatSample && startNearestBeatSample < _mapEventList[j].StartSample + 5)
                {
                    startTileX = j;
                    startXOffset = (float)((longEventStartSample - startNearestBeatSample) / samplesPerBeat);

                    break;
                }
            }

            Debug.DrawRay(new Vector2(startTileX + startXOffset, 100f), Vector2.down * 1000, Color.red, 100f);

            RaycastHit2D longStartHit = Physics2D.Raycast(new Vector2(startTileX + startXOffset, 100f), Vector2.down, 1000, _tileLayer);

            if (longStartHit)
            {
                float yOffset = longStartHit.point.y;
                // 타일 인덱스 수정 필
                _interactionTilemap.SetTile(GetTileChangeData(_TileType.Interaction, 2, new Vector3Int(startTileX, 0, 0), new Vector3(startXOffset, yOffset, 0f)), false);
            }

            for (int j = endTileX; j < _mapEventList.Count; j++)
            {
                if (_mapEventList[j].StartSample - 5 < endNearestBeatSample && endNearestBeatSample < _mapEventList[j].StartSample + 5)
                {
                    endTileX = j;
                    endXOffset = (float)((longEventEndSample - endNearestBeatSample) / samplesPerBeat);

                    break;
                }
            }

            Debug.DrawRay(new Vector2(endTileX + endXOffset, 100f), Vector2.down * 1000, Color.blue, 100f);

            RaycastHit2D longEndHit = Physics2D.Raycast(new Vector2(endTileX + endXOffset, 100f), Vector2.down, 1000, _tileLayer);

            if (longEndHit)
            {
                float yOffset = longEndHit.point.y;
                // 타일 인덱스 수정 필
                _interactionTilemap.SetTile(GetTileChangeData(_TileType.Interaction, 2, new Vector3Int(endTileX, 0, 0), new Vector3(endXOffset, yOffset, 0f)), false);
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
