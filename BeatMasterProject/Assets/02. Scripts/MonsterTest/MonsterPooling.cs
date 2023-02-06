using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SonicBloom.Koreo;
using UnityEngine;

public class MonsterPooling : MonoBehaviour
{
    public Queue<GameObject> disableMonsterQueue = new Queue<GameObject>();
    
    [Space][Header("Event")]
    [SerializeField] [EventID] private string _mapEventID;
    [SerializeField] [EventID] private string _shortEventID;
    [SerializeField] [EventID] private string _spdEventID;
    [SerializeField] private List<KoreographyEvent> _mapEventList = new List<KoreographyEvent>();
    [SerializeField] private List<KoreographyEvent> _shortEventList = new List<KoreographyEvent>();
    [SerializeField] private List<KoreographyEvent> _spdEventList = new List<KoreographyEvent>();

    [Header("No ObjectPool")] 
    [SerializeField] private GameObject _monsterPrefab;

    [SerializeField] private List<Vector3> tilePos = new List<Vector3>();

    private void Start()
    {
        _mapEventList = SoundManager.instance.playingKoreo.GetTrackByID(_mapEventID).GetAllEvents();
        _shortEventList = SoundManager.instance.playingKoreo.GetTrackByID(_shortEventID).GetAllEvents();
        _spdEventList = SoundManager.instance.playingKoreo.GetTrackByID(_spdEventID).GetAllEvents();

        for (int i = 0; i < _shortEventList.Count; i++)
        {
            for (int j = 0; j < _mapEventList.Count; j++)
            {
                if (_mapEventList[j].EndSample - 5 <= _shortEventList[i].EndSample &&
                    _shortEventList[i].EndSample <= _mapEventList[j].EndSample + 5)
                {
                    float[] groundData = _mapEventList[j].GetTextValue().Split().Select(float.Parse).ToArray();
                    int groundType = (int)groundData[0];
                    if (groundType == 0)
                    {
                        Instantiate(_monsterPrefab, new Vector3(tilePos[j].x + 1f, tilePos[j].y + 1.5f), Quaternion.identity, transform);
                        break;
                    }
                }
            }
        }
    }

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

    public void MonsterInit(float posX, float posY)
    {
        tilePos.Add(new Vector3(posX, posY, 0));
    }

    // public void ResetPool() //캐릭터가 체크포인트를 지났으면 현재 꺼져있는 몬스터 오브젝트를 다시 오브젝트 풀로 넣어줌
    // {
    //     while (disableMonsterQueue.Count != 0)
    //     {
    //         ReturnObject(disableMonsterQueue.Dequeue());
    //     }
    // }

    public void ReArrange() //캐릭터가 다음 체크포인트를 지나지 못하고 죽으면 꺼져있는 몬스터들을 다시 켜줌
    {
        while (disableMonsterQueue.Count != 0)
        {
            disableMonsterQueue.Dequeue().SetActive(true);
        }
    }
}

/*
 * private void Rewind()
    {
        SoundManager.instance.PlayBGM(false); // pause
        curSample = rewindSampleTime;
        //1. 캐릭터가 체크포인트를 지났는지 어떻게 아는가?
        speedTrack에서 속도바뀌는 시작부분과 끝부분이 체크포인트
        //Rewind함수에서 MonsterPooling.cs ReArrange()부르기
        //2. 그러면 체크포인트를 지났으면 부르는 함수은 ResetPool()은 어디서 불러야하는가? -> 아마 1번 질문이 해결되면 부르는곳을 알 수 있을 네
        ContinueGame(); // wait 3 sec and start
        DecreaseItem(5);
        gameUI.UpdateText(TextType.Item, coinCount);
        int death = IncreaseDeath(); // increase death count
        gameUI.UpdateText(TextType.Death, death);
        shortIdx = rewindShortIdx;
        longIdx = rewindLongIdx;
    }
 */
