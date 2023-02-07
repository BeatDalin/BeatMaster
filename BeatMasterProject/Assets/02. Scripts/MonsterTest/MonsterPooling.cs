using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using SonicBloom.Koreo;
using UnityEngine;
using UnityEngine.Serialization;

public class MonsterPooling : MonoBehaviour
{
    [SerializeField] List<GameObject> _disableMonsterList = new List<GameObject>();
    public List<GameObject> monsterList = new List<GameObject>();
    public List<float> checkPoint = new List<float>();

    [Space][Header("Event")]
    [SerializeField] [EventID] private string _mapEventID;
    [SerializeField] [EventID] private string _shortEventID;
    [SerializeField] [EventID] private string _spdEventID;
    [SerializeField] private List<KoreographyEvent> _mapEventList = new List<KoreographyEvent>();
    [SerializeField] private List<KoreographyEvent> _shortEventList = new List<KoreographyEvent>();
    [SerializeField] private List<KoreographyEvent> _spdEventList = new List<KoreographyEvent>();

    [Header("No ObjectPool")] 
    [SerializeField] private GameObject _monsterPrefab;

    [SerializeField] private List<Vector3> _tilePos = new List<Vector3>();
    private int _checkPointIdx = 0;
    private CharacterMovement _characterMovement;
    private int _maxMonsterCount;
    private int _deleteMonsterCount;
    private int _monsterIdx;
    private int _count;

    private void Awake()
    {
        _monsterIdx = 0;
        //Koreographer.Instance.RegisterForEventsWithTime("Level1_Short", AddMonsterQueue);
    }

    private void Start()
    {
        _characterMovement = FindObjectOfType<CharacterMovement>();
        
        _mapEventList = SoundManager.instance.playingKoreo.GetTrackByID(_mapEventID).GetAllEvents();
        _shortEventList = SoundManager.instance.playingKoreo.GetTrackByID(_shortEventID).GetAllEvents();
        _spdEventList = SoundManager.instance.playingKoreo.GetTrackByID(_spdEventID).GetAllEvents();

        foreach (var spd in _spdEventList)
        {
            checkPoint.Add(spd.EndSample);
        }

        for (int i = 0; i < _shortEventList.Count; i++)
        {
            for (int j = 0; j < _mapEventList.Count; j++)
            {
                if (_mapEventList[j].EndSample - 5 <= _shortEventList[i].EndSample &&
                    _shortEventList[i].EndSample <= _mapEventList[j].EndSample + 5)
                {
                    
                    GameObject g = Instantiate(_monsterPrefab, new Vector3(_tilePos[j].x + 1f, _tilePos[j].y + 2f), Quaternion.identity, transform);
                        
                    monsterList.Add(g);
                        
                    break;
                    // float[] groundData = _mapEventList[j].GetTextValue().Split().Select(float.Parse).ToArray();
                    // int groundType = (int)groundData[0];
                    // if (groundType == 0)
                    // {
                    //     
                    // }
                }
            }
        }

        _maxMonsterCount = monsterList.Count;
    }

    // void AddMonsterQueue(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    // {
    //     if (sampleTime >= _shortEventList[_checkPointIdx].EndSample)
    //     {
    //         disableMonsterQueue.Enqueue(monsterList[_checkPointIdx]);
    //         Debug.Log("sampletime"+sampleTime);
    //         Debug.Log("monstertime" + _shortEventList[_checkPointIdx].EndSample);
    //         monsterList[_checkPointIdx].SetActive(false);
    //         _checkPointIdx++;
    //     }
    // }

    // public override void Init()
    // {
    //     //shortEventList의 sampleTime을 가지고
    //     //_mapEventList의 sampleTime과 비교해서 같은게 있다면 _mapEventList index+1 한곳에다가 몬스터를 놓으면됨
    //     //몬스터를 놓을때는 y값에 +0.5만큼 올려두면됨.
    //
    //     for (int i = 0; i < _shortEventList.Count; i++)
    //     {
    //         for (int j = 0; j < _mapEventList.Count; j++)
    //         {
    //             if (_shortEventList[i].EndSample == _mapEventList[j].EndSample)
    //             {
    //                 
    //                 
    //                 break;
    //             }
    //         }
    //     }
    //
    //     float startPos = 5f;
    //     for (int i = 0; i < initCount; i++)
    //     {
    //         //몬스터프리팹 위치를 초기화(monster.cs에 transform위치를 저장하는 변수가 있는데 일단 그건 사용안함)
    //         poolingPrefab.transform.position = new Vector3(startPos, poolingPrefab.transform.position.y, 0f);
    //         GameObject monster = CreateNewObject(); 
    //         monster.SetActive(false);
    //         poolingObjectQueue.Enqueue(monster); //오브젝트풀 큐에 넣어줌
    //         startPos += 5f;
    //     }
    //     
    //     for (int i = 0; i < 20; i++)
    //     {
    //         GetObject(); //오브젝트풀 큐에서 하나씩빼면서 오브젝트 켜주기
    //     }
    // }

    public void DisableMonster()
    {
        _disableMonsterList.Add(monsterList[_monsterIdx]);
        monsterList[_monsterIdx].SetActive(false);
        _monsterIdx++;
    }

    public void AddTilePos(float posX, float posY)
    {
        _tilePos.Add(new Vector3(posX, posY, 0));
    }

    public void ResetPool() //캐릭터가 체크포인트를 지났으면 몬스터를 Destroy
    {
        _monsterIdx = _disableMonsterList.Count - 1;
        // for (int i = 0; i < _disableMonsterList.Count; i++)
        // {
        //     Destroy(_disableMonsterList[i]);
        // }
        // _disableMonsterList.Clear();
        _count = _monsterIdx - 1;
    }

    public void ReArrange() //캐릭터가 다음 체크포인트를 지나지 못하고 죽으면 꺼져있는 몬스터들을 다시 켜줌
    {
        for (int i = _count; i < _monsterIdx; i++)
        {
            monsterList[i].SetActive(true);
        }
        _monsterIdx = _count;
        // _disableMonsterList.Clear();
    }
}
