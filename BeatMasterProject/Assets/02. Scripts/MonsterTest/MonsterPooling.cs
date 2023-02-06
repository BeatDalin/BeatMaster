using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MonsterPooling : ObjectPooling
{
    public Queue<GameObject> disableMonsterQueue = new Queue<GameObject>();
    public override void Init()
    {
        float startPos = 5f;
        for (int i = 0; i < initCount; i++)
        {
            //몬스터프리팹 위치를 초기화(monster.cs에 transform위치를 저장하는 변수가 있는데 일단 그건 사용안함)
            poolingPrefab.transform.position = new Vector3(startPos, poolingPrefab.transform.position.y, 0f);
            GameObject monster = CreateNewObject(); 
            monster.SetActive(false);
            poolingObjectQueue.Enqueue(monster); //오브젝트풀 큐에 넣어줌
            startPos += 5f;
        }
        
        for (int i = 0; i < 20; i++)
        {
            GetObject(); //오브젝트풀 큐에서 하나씩빼면서 오브젝트 켜주기
        }
    }

    public void ResetPool() //캐릭터가 체크포인트를 지났으면 현재 꺼져있는 몬스터 오브젝트를 다시 오브젝트 풀로 넣어줌
    {
        while (disableMonsterQueue.Count != 0)
        {
            ReturnObject(disableMonsterQueue.Dequeue());
        }
    }

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
