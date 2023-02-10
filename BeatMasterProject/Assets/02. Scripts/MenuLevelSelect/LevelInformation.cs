using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;
using UnityEngine.EventSystems;
public class LevelInfo
{
    public int levelNum; // 레벨 index
    public int nextLevelNum; // 다음 level index
    public int beforeLevelNum; // 이전 level index
    public int maxLevelNum = 3; // 최대 level index
    public string levelDescription; // levelUI TextArea에 들어갈 level 설명
    public bool isLocked; // 레벨 잠금 여부
    
    public Vector3 mapPos; // levelUI에서 보여줄 map 영역
    public Vector3 camPos; // levelUI BG에서 보여줄 map 영역

    public LevelInfo(int levelNum, string levelDescription, Vector3 mapPos, Vector3 camPos)
    {
        this.levelNum = levelNum;
        nextLevelNum = this.levelNum == maxLevelNum ? this.levelNum : this.levelNum + 1; // 현재 레벨이 최대 레벨이라면, 다음 레벨 없음
        beforeLevelNum = this.levelNum == 0 ? this.levelNum : this.levelNum - 1; // 현재 레벨이 최소 레벨이라면, 이전 레벨 없음
        this.levelDescription = levelDescription;
        isLocked = this.levelNum == 0 ? false : true; // 현재 레벨이 최소 레벨이라면, 잠금 false
        this.mapPos = mapPos;
        this.camPos = camPos;
    }
}

public class LevelInformation : MonoBehaviour
{
    [SerializeField] private GameObject _levelPanel;
    private CanvasGroup _levelCanvasGroup;
    [Header("StageData")]
    public int curStage = 0;
    public int curMaxLevel; // 현재 clear한 레벨 중 가장 높은 레벨 index
    private int _maxLevelNum = 3; // 최대 level index
    public LevelData[] curStageData = new LevelData[4];
    public int uiLevel;
    
    private readonly LevelInfo[] _levelInfo = new LevelInfo[4];
    
    [Header("Level UI Info")]
    [SerializeField] private Camera _mainCam;
    [SerializeField] private GameObject _maskTarget;
    [SerializeField] private Text _levelTitle;
    [SerializeField] private Text _description;
    [SerializeField] private GameObject _lockedPanel;
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
                _levelInfo[i].isLocked = !curStageData[_levelInfo[i].beforeLevelNum].levelClear;
            }
        }
        
        curMaxLevel = GetMaxLevelInStage();

        SetLevelInfo(curMaxLevel+1);
        uiLevel = curMaxLevel + 1;

        // Level Canvas Fade In during Scene Transition Effect is playing
        _levelCanvasGroup = GetComponent<CanvasGroup>();
        StartCoroutine(CoFadeIn());
    }

    private IEnumerator CoFadeIn()
    {
        _levelCanvasGroup.alpha = 0;
        _levelCanvasGroup.blocksRaycasts = false;
        _levelPanel.SetActive(true);
        yield return new WaitUntil(() => SceneLoadManager.Instance.isLoaded);
        while (_levelCanvasGroup.alpha < 1f)
        {
            _levelCanvasGroup.alpha += 0.003f;
            yield return new WaitForEndOfFrame();
        }
        _levelCanvasGroup.blocksRaycasts = true;
    }

    private IEnumerator CoFadeOut()
    {
        _levelCanvasGroup.alpha = 1;
        _levelCanvasGroup.blocksRaycasts = true;
        while (_levelCanvasGroup.alpha > 0f)
        {
            _levelCanvasGroup.alpha -= 0.003f;
            yield return new WaitForEndOfFrame();
        }
        _levelCanvasGroup.blocksRaycasts = false;
        _levelPanel.SetActive(false);
    }
    
    private void SetLevelInfo(int levelNum)
    {
        // levelNum 최댓 최솟값 한정
        levelNum = levelNum > _maxLevelNum ? _maxLevelNum : levelNum;
        levelNum = levelNum < 0 ? 0 : levelNum;
        
        // move Btn
        _moveBtn[0].SetActive(levelNum != 0);
        _moveBtn[1].SetActive(levelNum != _maxLevelNum);
        
        // Camera
        _mainCam.transform.position = Vector3.MoveTowards(_mainCam.transform.position, _levelInfo[levelNum].camPos, 10f);
        
        // locked Img
        _lockedPanel.SetActive(_levelInfo[levelNum].isLocked);
        
        // levelTitle Txt
        int currLevel = _levelInfo[levelNum].levelNum + 1;
        _levelTitle.text = "LEVEL " + currLevel;
        
        // map position
        _maskTarget.GetComponent<RectTransform>().localPosition =
            new Vector3(_levelInfo[levelNum].mapPos.x, _levelInfo[levelNum].mapPos.y, 0);
        
        // description Txt
        _description.text = _levelInfo[levelNum].levelDescription;
    }

    public void OnClickStartBtn()
    {
        StartCoroutine(CoFadeOut());
        SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.Level1);
    }

    public void OnClickLeftBtn()
    {
        SetLevelInfo(--uiLevel);
    }

    public void OnClickRightBtn()
    {
       SetLevelInfo(++uiLevel);
    }

    public void OnClickTitleBtn()
    {
        StartCoroutine(CoFadeOut());
        SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.Title);
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
