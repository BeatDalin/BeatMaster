using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class MonsterPooling : MonoBehaviour
{
    public List<GameObject> monsterList = new List<GameObject>();

    [Space][Header("Event")]
    [SerializeField] [EventID] private string _mapEventID;
    [SerializeField] [EventID] private string _shortEventID;
    [SerializeField] private List<KoreographyEvent> _mapEventList = new List<KoreographyEvent>();
    [SerializeField] private List<KoreographyEvent> _shortEventList = new List<KoreographyEvent>();

    [Header("Variable")] 
    [SerializeField] private GameObject _monsterPrefab;
    [SerializeField] private int _monsterIdx;
    [SerializeField] private Camera _camera;

    [Header("Transform")]
    public RectTransform coinPos;
    
    [Header("Tile")]
    [SerializeField] private List<Vector3> _tilePos = new List<Vector3>();

    private Vector3 _coinScreenPos;

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

        Debug.Log(_camera.name);
        _coinScreenPos = _camera.WorldToScreenPoint(coinPos.position);
        Debug.Log(_coinScreenPos);

        _coinScreenPos = _camera.ScreenToWorldPoint(_coinScreenPos);
        Debug.Log(_coinScreenPos);

        Invoke("SpawnMonster", 0.3f);
    }

    private void SpawnMonster()
    {
        for (int i = 0; i < _shortEventList.Count; i++)
        {
            if (_shortEventList[i].GetIntValue() == 1)
            {
                GameObject g = Instantiate(_monsterPrefab, new Vector3(_tilePos[i].x + 1f, _tilePos[i].y + 2f), Quaternion.identity, transform);

                monsterList.Add(g);
            }
            //for (int j = 0; j < _mapEventList.Count; j++)
            //{
            //    if (_mapEventList[j].EndSample - 5 <= _shortEventList[i].EndSample &&
            //        _shortEventList[i].EndSample <= _mapEventList[j].EndSample + 5)
            //    {

            //        break;
            //    }
            //}
        }
    }

    public void DisableMonster()
    {
        monsterList[_monsterIdx].GetComponent<Monster>().ShowAnim(_coinScreenPos);
        _monsterIdx++;
    }

    public void AddTilePos(float posX, float posY)
    {
        _tilePos.Add(new Vector3(posX, posY, 0));
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
}
