using System;
using System.Collections;
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
    // [SerializeField] private GameObject _clearImg;
    // [SerializeField] private GameObject[] _starImg;
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
        
        // Add StageBtn Listener
        for (int i = 0; i < _stageBtn.Length; i++)
        {
            var stageNum = i;
            _stageBtn[i].onClick.AddListener(() => SetStageInfo(stageNum));
        }

        _stageBtns.SetActive(true);
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
        SoundManager.instance.PlaySFX("Touch");
        _stageBtns.SetActive(false);
        
        // levelNum 최댓 최솟값 한정
        stageNum = stageNum > _maxStageNum ? _maxStageNum : stageNum;
        stageNum = stageNum < 0 ? 0 : stageNum;
        
        // move Btn
        _moveBtn[0].SetActive(stageNum != 0);
        _moveBtn[1].SetActive(stageNum != _maxStageNum);
        
        
        // Toggles
        _levelToggles[0].isOn = true;
        SetToggleStatus(_levelToggles, stageNum);
        
        // StageTitle Txt
        int curStage = _stageInfo[stageNum].stageNum + 1;
        _stageTitle.text = "Stage " + curStage;
        
        // map position
        _maskTarget.GetComponent<RectTransform>().localPosition = _stageInfo[stageNum].mapPos;
        
        // description Txt
        _description.text = _stageInfo[stageNum].stageDescription;

        uiStage = _stageInfo[stageNum].stageNum;
        
        // Toggles
        SetToggleStatus(_levelToggles, uiStage);
        
        // Camera
        StartCoroutine(CoZoomIn(_mainCam.transform.position, _stageInfo[stageNum].camPos, 8, 3));
    }

    private IEnumerator CoZoomIn(Vector3 current, Vector3 target, float currentSize, float targetSize)
    {
        float time = 0.5f;
        float elapsedTime = 0.0f;
            // _mainCam.orthographicSize > 5
        while(elapsedTime < time)
        {
            elapsedTime += (Time.deltaTime);
            
            _mainCam.transform.position = 
                Vector3.Lerp(current, target, elapsedTime/time);
            _mainCam.orthographicSize =
                Mathf.Lerp(currentSize, targetSize, elapsedTime / time);
            
            yield return null;
        }

        _mainCam.transform.position = target;
        _mainCam.orthographicSize = targetSize;

        yield return new WaitForSeconds(0.3f);  
        _stagePanel.SetActive(currentSize > targetSize);
        _stageBtns.SetActive(!(currentSize > targetSize));
    }
    
    private IEnumerator CoZoomOut(Vector3 current, Vector3 target, float currentSize, float targetSize)
        {
            _stagePanel.SetActive(currentSize > targetSize);
            
            float time = 0.5f;
            float elapsedTime = 0.0f;
                // _mainCam.orthographicSize > 5
            while(elapsedTime < time)
            {
                elapsedTime += (Time.deltaTime);
                
                _mainCam.transform.position = 
                    Vector3.Lerp(current, target, elapsedTime/time);
                _mainCam.orthographicSize =
                    Mathf.Lerp(currentSize, targetSize, elapsedTime / time);
                
                yield return null;
            }
    
            _mainCam.transform.position = target;
            _mainCam.orthographicSize = targetSize;
    
            yield return new WaitForSeconds(0.3f);
            
            _stageBtns.SetActive(!(currentSize > targetSize));
        }
    
    
    private void SetToggleStatus(Toggle[] toggles, int stageNum)
    {
        for (int i = 0; i < toggles.Length; i++)
        {
            LevelData levelData = DataCenter.Instance.GetLevelData(stageNum, i);
            
            toggles[i].GetComponent<Toggle>().interactable = levelData.isUnlocked; // 클릭 막기
            toggles[i].transform.GetChild(0).GetComponent<Image>().sprite = _toggleSprite[levelData.isUnlocked ? 0 : 1]; // 잠금 여부에 따라 toggle sprite 변경
            toggles[i].transform.GetChild(2).gameObject.SetActive(levelData.isUnlocked); // Text 영역
            //Debug.Log("stage"+ stageNum+" level "+i+" "+levelData.isUnlocked);

            if (levelData.isUnlocked)
            {
                toggles[i].transform.GetChild(0).gameObject.SetActive(!toggles[i].isOn); // Shadow 끄고
                toggles[i].transform.GetChild(1).gameObject.SetActive(toggles[i].isOn); 
            }
            else
            {
                toggles[i].transform.GetChild(0).gameObject.SetActive(true); // Shadow 키고
                toggles[i].transform.GetChild(1).gameObject.SetActive(false);   // isOn Object 끔
            }

            // clear Img
            if (levelData.levelClear)
            {
                for (int j = 0; j < 2; j++)
                {
                    GameObject starImg = toggles[i].transform.GetChild(3).GetChild(j).GetChild(1).gameObject;

                    if (i < levelData.star)
                    {
                        starImg.SetActive(true);
                    }
                    else
                    {
                        starImg.SetActive(false);
                    }
                }
            }

            toggles[i].transform.GetChild(3).gameObject.SetActive(levelData.levelClear);
        }
    }

    public void OnToggleValueChange()
    {
        SoundManager.instance.PlaySFX("Touch");
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
        // SoundManager.instance.PlaySFX("Touch");
        SetStageInfo(uiStage-1);
    }

    public void OnClickRightBtn()
    { 
        // SoundManager.instance.PlaySFX("Touch");
        SetStageInfo(uiStage+1);
    }

    public void OnClickCloseBtn()
    {
        SoundManager.instance.PlaySFX("Touch");
        StartCoroutine(
            CoZoomOut(_mainCam.transform.position, _camPos[0].transform.position, _mainCam.orthographicSize, 8));

    }

    public void OnClickTitleBtn()
    {
        SoundManager.instance.PlaySFX("Touch");
        // StartCoroutine(CoFadeOut());
        SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.Title);
    }
}
