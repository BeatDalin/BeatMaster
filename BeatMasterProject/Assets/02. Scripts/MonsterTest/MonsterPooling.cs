using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class MonsterPooling : MonoBehaviour
{
    public enum MonsterType
    {
        Ground, Air
    }

    public List<GameObject> monsterList = new List<GameObject>();
    public GameObject coinParent;

    [Space][Header("Event")]
    [SerializeField] [EventID] private string _mapEventID;
    [SerializeField] [EventID] private string _shortEventID;
    [SerializeField] private List<KoreographyEvent> _mapEventList = new List<KoreographyEvent>();
    [SerializeField] private List<KoreographyEvent> _shortEventList = new List<KoreographyEvent>();

    [Header("Variable")]
    [SerializeField] private List<GameObject> _monsterPrefabList;
    [SerializeField] private GameObject _monsterPrefab;
    [SerializeField] private GameObject _coinPrefab;
    [SerializeField] private int _monsterIdx;
    [SerializeField] private Camera _camera;

    [Header("Transform")]
    public RectTransform coinPos;

    [Header("Tile")]
    private Dictionary<Vector3, int> _monsterInfos = new Dictionary<Vector3, int>();
    [SerializeField] private List<Vector3> _tilePos = new List<Vector3>();

    public Vector2 _coinScreenPos;

    private int _checkPointIdx = 0;
    private int _deleteMonsterCount;
    private int _count;

    private void Awake()
    {
        _monsterIdx = 0;
    }

    private void Start()
    {
        _mapEventList = SoundManager.instance.playingKoreo.GetTrackByID(_mapEventID).GetAllEvents();
        _shortEventList = SoundManager.instance.playingKoreo.GetTrackByID(_shortEventID).GetAllEvents();
        
        _coinScreenPos = _camera.ScreenToWorldPoint(coinPos.position);
        
        StartCoroutine(CoWaitForList());
        //Invoke("SpawnMonster", 0.3f);
        // for (int i = 0; i < _shortEventList.Count; i++)
        // {
        //     for (int j = 0; j < _mapEventList.Count; j++)
        //     {
        //         if (_mapEventList[j].EndSample - 5 <= _shortEventList[i].EndSample &&
        //             _shortEventList[i].EndSample <= _mapEventList[j].EndSample + 5)
        //         {
        //             if (_shortEventList[i].GetIntValue() == 1)
        //             {
        //                 GameObject g = Instantiate(_monsterPrefab, new Vector3(_tilePos[j].x + 1f, _tilePos[j].y + 2f), Quaternion.identity, transform);
        //                 
        //                 monsterList.Add(g);
        //             }
        //             break;
        //         }
        //     }
        // }
    }

    IEnumerator CoWaitForList()
    {
        while (_tilePos.Count == 0)
        {
            yield return null;
        }
        SpawnMonster();
    }

    public void DisableMonster()
    {
        monsterList[_monsterIdx].GetComponent<Monster>().ShowAnim(_coinScreenPos, coinParent);
        _monsterIdx++;
    }

    public void AddMonsterInfo(float posX, float posY, MonsterType monsterType)
    {
        _tilePos.Add(new Vector3(posX, posY, 0));
        _monsterInfos.Add(new Vector3(posX, posY, 0f), (int)monsterType);
    }

    public void ChangeMonsterType(float posX, float posY, MonsterType monsterType)
    {
        _monsterInfos[new Vector3(posX, posY, 0f)] = (int)monsterType;
    }

    public void ResetPool() //캐릭터가 체크포인트를 지났으면 인덱스 시작부분을 변경
    {
        _count = _monsterIdx;
    }

    public void ReArrange() //캐릭터가 다음 체크포인트를 지나지 못하고 죽으면 꺼져있는 몬스터들을 다시 켜줌
    {
        for (int i = _count; i < _monsterIdx; i++)
        {
            monsterList[i].GetComponent<Monster>().ChangeAlpha(true);
            //monsterList[i].SetActive(true);
        }

        _monsterIdx = _count;
    }
    
    private void SpawnMonster()
    {
        for (int i = 0; i < _shortEventList.Count; i++)
        {
            if (_shortEventList[i].GetIntValue() == 1)
            {
                int monsterType;

                _monsterInfos.TryGetValue(_tilePos[i], out monsterType);

                GameObject g = Instantiate(_monsterPrefabList[monsterType], new Vector3(_tilePos[i].x + 1f, _tilePos[i].y + 1f), Quaternion.identity, transform);
                Instantiate(_coinPrefab, new Vector3(_tilePos[i].x + 1f, _tilePos[i].y + 1f), Quaternion.identity, transform);
                
                monsterList.Add(g);
            }
        }
    }
}
