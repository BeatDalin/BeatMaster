using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class StageInfo
{
    public int stageNum; // stage index
    public int nextstageNum; // 다음 level index
    public int formerStageNum; // 이전 level index
    public int maxStageNum = 3; // 최대 level index
    public string stageDescription; // levelUI TextArea에 들어갈 level 설명
    
    public Vector3 mapPos; // StageUI에서 보여줄 map 영역
    public Vector3 camPos; // StageUI BG에서 보여줄 map 영역

    public StageInfo(int stageNum, string stageDescription, Vector3 mapPos, Vector3 camPos)
    {
        this.stageNum = stageNum;
        nextstageNum = this.stageNum == maxStageNum ? this.stageNum : this.stageNum + 1; // 현재 레벨이 최대 레벨이라면, 다음 레벨 없음
        formerStageNum = this.stageNum == 0 ? this.stageNum : this.stageNum - 1; // 현재 레벨이 최소 레벨이라면, 이전 레벨 없음
        this.stageDescription = stageDescription;
        this.mapPos = mapPos;
        this.camPos = camPos;
    }
}

public class StageInformation : MonoBehaviour
{
    [SerializeField] private GameObject _stagePanel;
    // private CanvasGroup _levelCanvasGroup;
    [Header("StageData")]
    private int _maxStageNum = 3; // 최대 stage index
    public LevelData[] curStageData = new LevelData[4];
    public int uiStage;
    
    private readonly StageInfo[] _stageInfo = new StageInfo[4];

    [Header("Level UI Info")] 
    [SerializeField] private Camera _mainCam;
    [SerializeField] private GameObject _stageBtns;
    [SerializeField] private Button[] _stageBtn;
    
    [SerializeField] private Text _stageTitle;
    [SerializeField] private Text _description;
    [SerializeField] private GameObject _maskTarget;
    [SerializeField] private Vector3[] _mapPos;
    [SerializeField] private Toggle[] _levelToggles;
    [SerializeField] private ToggleGroup _toggleGroup;
    [SerializeField] private Sprite[] _toggleSprite;
    [SerializeField] private GameObject[] _moveBtn;
    [SerializeField] private GameObject[] _camPos;
    [SerializeField] private GameObject _clearImg;
    [SerializeField] private GameObject[] _starImg;
    private readonly String[] _stageDescription =
    {
        "\"설레는 첫 번째 모험!\"", "\"도시에서는 어떤 일이 일어날까?\"", 
        "\"난 기쁠 때 리듬과 모래바람을 타\"", "\"음악과 함께라면 추위도 무섭지 않아!\""
    };
    
    private void Awake()
    {
        DataCenter.Instance.LoadData();
        
        for (int i = 0; i <= _maxStageNum; i++)
        {
            curStageData[i] = DataCenter.Instance.GetLevelData(uiStage, i);
            _stageInfo[i] = new StageInfo(i, _stageDescription[i], _mapPos[i], _camPos[i+1].gameObject.transform.position);
        }
        
        AddStageBtnListener();
        _stageBtns.SetActive(true);
    }

    private void AddStageBtnListener()
    {
        for (int i = 0; i < _stageBtn.Length; i++)
        {
            var stageNum = i;
            _stageBtn[i].onClick.AddListener(() => SetStageInfo(stageNum));
        }
    }

    // private IEnumerator CoFadeIn()
    // {
    //     _levelCanvasGroup.alpha = 0;
    //     _levelCanvasGroup.blocksRaycasts = false;
    //     _levelPanel.SetActive(true);
    //     yield return new WaitUntil(() => SceneLoadManager.Instance.isLoaded);
    //     while (_levelCanvasGroup.alpha < 1f)
    //     {
    //         _levelCanvasGroup.alpha += 0.003f;
    //         yield return new WaitForEndOfFrame();
    //     }
    //     _levelCanvasGroup.blocksRaycasts = true;
    // }
    //
    // private IEnumerator CoFadeOut()
    // {
    //     _levelCanvasGroup.alpha = 1;
    //     _levelCanvasGroup.blocksRaycasts = true;
    //     while (_levelCanvasGroup.alpha > 0f)
    //     {
    //         _levelCanvasGroup.alpha -= 0.003f;
    //         yield return new WaitForEndOfFrame();
    //     }
    //     _levelCanvasGroup.blocksRaycasts = false;
    //     _levelPanel.SetActive(false);
    // }
    
    private void SetStageInfo(int stageNum)
    {
        _stageBtns.SetActive(false);
        
        // levelNum 최댓 최솟값 한정
        stageNum = stageNum > _maxStageNum ? _maxStageNum : stageNum;
        stageNum = stageNum < 0 ? 0 : stageNum;
        
        // move Btn
        _moveBtn[0].SetActive(stageNum != 0);
        _moveBtn[1].SetActive(stageNum != _maxStageNum);
        
        // clear Img
        if (curStageData[stageNum].levelClear)
        {
            for (int i = 0; i < 3; i++)
            {
                if (i < curStageData[stageNum].star)
                {
                   _starImg[i].SetActive(true);
                }
                else
                {
                    _starImg[i].SetActive(false);
                }
            }
        }
        _clearImg.SetActive(curStageData[stageNum].levelClear);
        
        // Camera
        _mainCam.orthographicSize = 5f;
        _mainCam.transform.position = 
            Vector3.MoveTowards(_mainCam.transform.position, _stageInfo[stageNum].camPos, 10f);
        
        // Toggles
        SetToggleStatus(_levelToggles, stageNum);
        
        // StageTitle Txt
        int curStage = _stageInfo[stageNum].stageNum + 1;
        _stageTitle.text = "Stage " + curStage;
        
        // map position
        _maskTarget.GetComponent<RectTransform>().localPosition = _stageInfo[stageNum].mapPos;
        
        // description Txt
        _description.text = _stageInfo[stageNum].stageDescription;

        uiStage = _stageInfo[stageNum].stageNum;
        
        _stagePanel.SetActive(true);
    }
    
    private void SetToggleStatus(Toggle[] toggles, int stageNum)
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            LevelData levelData = DataCenter.Instance.GetLevelData(stageNum, i);
            
            toggles[i].GetComponent<Toggle>().interactable = levelData.isUnlocked;
            toggles[i].transform.GetChild(2).gameObject.SetActive(levelData.isUnlocked);
            toggles[i].transform.GetChild(0).GetComponent<Image>().sprite = _toggleSprite[levelData.isUnlocked ? 0 : 1];
            Debug.Log("level "+i+" "+levelData.isUnlocked);

            if (levelData.isUnlocked)
            {
                toggles[i].transform.GetChild(0).gameObject.SetActive(!toggles[i].isOn);
                toggles[i].transform.GetChild(1).gameObject.SetActive(toggles[i].isOn);
            }
        }
    }

    public void OnToggleValueChange()
    {
        SetToggleStatus(_levelToggles, uiStage);
    }

    private int GetSelectedToggle(Toggle[] toggles)
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            if (toggles[i].isOn)
            {
                Debug.Log(i);
                return i;
            }
        }
        return -1;
    }

    public void OnClickStartBtn()
    {
        SoundManager.instance.PlaySFX("Touch");

        switch (GetSelectedToggle(_levelToggles))
        {
            case 0:
                SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.Level1);
                break;
            case 1:
                SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.Level2);
                break;
            case 2:
                SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.Level3);
                break;
            case 3:
                SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.Level4);
                break;
        }
        // StartCoroutine(CoFadeOut());
        // SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.Level1);
    }

    public void OnClickLeftBtn()
    {
        SoundManager.instance.PlaySFX("Touch");
        SetStageInfo(uiStage-1);
    }

    public void OnClickRightBtn()
    { 
        SoundManager.instance.PlaySFX("Touch");
        SetStageInfo(uiStage+1);
    }

    public void OnClickCloseBtn()
    {
        SoundManager.instance.PlaySFX("Touch");
        _mainCam.orthographicSize = 8f;
        _mainCam.transform.position = _camPos[0].transform.position;
        _stagePanel.SetActive(false);
        _stageBtns.SetActive(true);

    }

    public void OnClickTitleBtn()
    {
        SoundManager.instance.PlaySFX("Touch");
        // StartCoroutine(CoFadeOut());
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

        for (int i = _maxStageNum; i >= 0; i--)
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
