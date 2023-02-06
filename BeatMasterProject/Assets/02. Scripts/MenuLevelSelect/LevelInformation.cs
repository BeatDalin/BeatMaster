using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class LevelInfo
{
    public int _levelNum; // 레벨 index
    public int _nextLevelNum; // 다음 level index
    public int _beforeLevelNum; // 이전 level index
    public int _maxLevelNum = 3; // 최대 level index
    public string _levelDescription; // levelUI TextArea에 들어갈 level 설명
    public bool _isLocked; // 레벨 잠금 여부
    
    public Vector3 _mapPos; // levelUI에서 보여줄 map 영역
    public Vector3 _camPos; // levelUI BG에서 보여줄 map 영역

    public LevelInfo(int levelNum, string levelDescription, Vector3 mapPos, Vector3 camPos)
    {
        _levelNum = levelNum;
        _nextLevelNum = _levelNum == _maxLevelNum ? _levelNum : _levelNum + 1; // 현재 레벨이 최대 레벨이라면, 다음 레벨 없음
        _beforeLevelNum = _levelNum == 0 ? _levelNum : _levelNum - 1; // 현재 레벨이 최소 레벨이라면, 이전 레벨 없음
        _levelDescription = levelDescription;
        _isLocked = _levelNum == 0 ? false : true; // 현재 레벨이 최소 레벨이라면, 잠금 false
        _mapPos = mapPos;
        _camPos = camPos;
    }
}

public class LevelInformation : MonoBehaviour
{
    [SerializeField] private GameObject _levelPanel;

    [Header("StageData")]
    public int curStage = 0;
    public int curMaxLevel; // 현재 clear한 레벨 중 가장 높은 레벨 index
    private int _maxLevelNum = 3; // 최대 level index
    public LevelData[] curStageData = new LevelData[4];
    
    private readonly LevelInfo[] _levelInfo = new LevelInfo[4];
    
    [Header("Level UI Info")]
    [SerializeField] private Camera _mainCam;
    [SerializeField] private GameObject _maskTarget;
    [SerializeField] private Text _levelTitle;
    [SerializeField] private Text _description;
    [SerializeField] private GameObject _lockedPanel;
    [SerializeField] private Button _startBtn;
    [SerializeField] private Vector3[] _mapPos;
    [SerializeField] private GameObject[] _moveBtn;
    [SerializeField] private GameObject[] _camPos;
    private readonly String[] _levelDescription =
    {
        "첫 번째\n발걸음을\n떼어보세요", "두 번째\n발걸음을\n떼어보세요", "세 번째\n발걸음을\n떼어보세요", "네 번째\n발걸음을\n떼어보세요"
    };
    
    private void Awake()
    {
        DataCenter.Instance.LoadData();
        
        for (int i = 0; i <= _maxLevelNum; i++)
        {
            curStageData[i] = DataCenter.Instance.GetLevelData(curStage, i);
            _levelInfo[i] = new LevelInfo(i, _levelDescription[i], _mapPos[i], _camPos[i].gameObject.transform.position);
            
            if (i > 0)
            {
                // 이전 levelClear했다면, 현재 레벨 unlock
                _levelInfo[i]._isLocked = !curStageData[_levelInfo[i]._beforeLevelNum].levelClear;
            }
        }
        
        curMaxLevel = GetMaxLevelInStage();

        SetLevelInfo(curMaxLevel+1);

        _levelPanel.SetActive(true);
    }
    
    private void SetLevelInfo(int levelNum)
    {
        // levelNum 최댓값 한정
        levelNum = levelNum > _maxLevelNum ? _maxLevelNum : levelNum;
        
        // Camera
        _mainCam.transform.position = Vector3.MoveTowards(_mainCam.transform.position, _levelInfo[levelNum]._camPos, 10f);
        
        // move Btn
        _moveBtn[1].GetComponent<Button>().onClick.AddListener(() => SetLevelInfo(_levelInfo[levelNum]._nextLevelNum));
        _moveBtn[0].GetComponent<Button>().onClick.AddListener(() => SetLevelInfo(_levelInfo[levelNum]._beforeLevelNum));

        _moveBtn[0].SetActive(levelNum != 0);
        _moveBtn[1].SetActive(levelNum != _maxLevelNum);
        
        // locked Img
        _lockedPanel.SetActive(_levelInfo[levelNum]._isLocked);
        
        // start Btn
        _startBtn.onClick.AddListener(() =>
            SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.Level1));

        // levelTitle Txt
        int currLevel = _levelInfo[levelNum]._levelNum + 1;
        _levelTitle.text = "LEVEL " + currLevel;
        
        // map position
        _maskTarget.GetComponent<RectTransform>().localPosition =
            new Vector3(_levelInfo[levelNum]._mapPos.x, _levelInfo[levelNum]._mapPos.y, 0);
        
        // description Txt
        _description.text = _levelInfo[levelNum]._levelDescription;
    }

    /// <summary>
    /// 현재 Stage 내에서 클리어한 최대 level의 index를 반환하는 함수
    /// </summary>
    /// <returns>return -1 == level 1 Not Cleared, 0 = level 1 Cleared, 2 == level 3 Cleared</returns>
    public int GetMaxLevelInStage()
    {
        int maxLevel;
        
        maxLevel = -1;

        for (int i = _maxLevelNum; i >= 0; i--)
        {
            if (curStageData[i].levelClear)
            {
                maxLevel = i;
                break;
            }
        }
        
        return maxLevel;
    }
}
