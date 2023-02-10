using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;

public enum BeatResult
{
    Fail,
    Fast,
    Perfect,
    Slow,
}

public enum GameState
{
    Idle,
    Play,
    Pause,
    End
}

public abstract class Game : MonoBehaviour
{
    [SerializeField] protected GameUI gameUI; // LevelGameUI or BossGameUI will come in.
    [SerializeField][EventID] private string _spdEventID;

    [Header("Game Play")]
    public GameState curState = GameState.Idle;
    public int curSample;

    [Header("Check Point")]
    protected int rewindShortIdx;
    protected int rewindLongIdx;
    protected int rewindSampleTime = -1;
    [SerializeField] protected List<KoreographyEvent> checkPointList;
    [SerializeField] protected bool[] checkPointVisited;
    protected int checkPointIdx = -1;

    [Header("Result Check")]
    public BeatResult[] longResult;
    public BeatResult[] shortResult;
    protected static int totalNoteCount = 0;
    // long notes
    [SerializeField] protected int longIdx = 0;
    protected bool isLongPressed = false;
    protected bool isLongKeyCorrect = false;
    // short notes
    [SerializeField] protected int shortIdx = 0;
    //protected bool isShortKeyPressed = false; // To prevent double check...
    protected bool isShortKeyCorrect = false;

    // death
    [SerializeField] protected int deathCount = 0;
    protected bool isLongFailed = false; // for testing purpose ....

    [Header("Data")]
    [SerializeField] private int _stageIdx; // Stage number-1 : This is an index!!!
    [SerializeField] private int _levelIdx; // Level number-1 : This is an index!!!
    public int coinCount;

    private int[] _longSummary = new int[4]; // Record the number of Fail, Fast, Perfect, Slow results from short notes
    private int[] _shortSummary = new int[4]; // Record the number of Fail, Fast, Perfect, Slow results from long notes
    private int[] _finalSummary = new int[4]; // Summed number of short note & long note results for each result type

    protected virtual void Awake()
    {
        gameUI = FindObjectOfType<GameUI>(); // This will get LevelGameUI or BossGameUI object
        Koreographer.Instance.ClearEventRegister(); // Initialize Koreographer Event Regiser
        // Data
        DataCenter.Instance.LoadData();
    }

    protected virtual void Start()
    {
        // StartWithDelay();
        StartCoroutine(CoStartWithDelay());
        Koreographer.Instance.RegisterForEvents(_spdEventID, CheckEnd);
    }

    protected virtual void Init()
    {
        longIdx = 0;
        shortIdx = 0;
        isLongPressed = false;
        isLongKeyCorrect = false;
        coinCount = 0;
        // Save Point
        checkPointList = SoundManager.instance.playingKoreo.GetTrackByID("Level1_CheckPoint").GetAllEvents();
        rewindShortIdx = 0;
        rewindLongIdx = 0;
        rewindSampleTime = -1;
        checkPointIdx = -1;
    }

    protected void StartWithDelay(int startSample = 0)
    {
        StartCoroutine(SceneLoadManager.Instance.CoSceneEnter());
        StartCoroutine(CoStartWithDelay(startSample));
    }

    protected IEnumerator CoStartWithDelay(int startSample = 0)
    {
        // UI Timer
        // gameUI.timePanel.SetActive(true);
        // // Wait for Scene Transition to end
        // yield return new WaitWhile(() => !SceneLoadManager.Instance.GetTransitionEnd());
        // int waitTime = 3;
        // while (waitTime > 0)
        // {
        //     gameUI.UpdateText(TextType.Time, waitTime);
        //     waitTime--;
        //     yield return new WaitForSeconds(1);
        // }
        gameUI.timePanel.SetActive(false);

        yield return new WaitUntil(() => SceneLoadManager.Instance.isLoaded);
        
        // Music Play & Game Start
        startSample = startSample < 0 ? 0 : startSample; // if less than zero, set as zero
        yield return new WaitForSeconds(1f);
        SoundManager.instance.PlayBGM(true, startSample);
        curState = GameState.Play;
        PlayerStatus.Instance.ChangeStatus(CharacterStatus.Run);
    }

    protected int[,] CalculateRange(List<KoreographyEvent> koreographyEvents)
    {
        int[,] sampleRange = new int[koreographyEvents.Count, 2];
        for (int i = 0; i < koreographyEvents.Count; i++)
        {
            KoreographyEvent curEvent = koreographyEvents[i];
            int eventLength = curEvent.EndSample - curEvent.StartSample;
            sampleRange[i, 0] = curEvent.StartSample + eventLength / 5;
            sampleRange[i, 1] = curEvent.StartSample + eventLength / 5 * 4;
        }
        return sampleRange;
    }

    private void CheckEnd(KoreographyEvent evt)
    {
        if (!evt.HasTextPayload())
        {
            return;
        }

        string message = evt.GetTextValue();

        if (message == "End")
        {
            SummarizeResult();
            RateResult(_stageIdx, _levelIdx);
            gameUI.ShowFinalResult(_finalSummary, totalNoteCount, _stageIdx, _levelIdx); // for testing purpose ...
        }
        else if (message == "Stop")
        {
            PlayerStatus.Instance.ChangeStatus(CharacterStatus.Idle);
        }
    }

    protected void CheckBeatResult(BeatResult[] resultArr, int idx, bool isKeyCorrect, int pressedTime, int[,] eventRange)
    {
        BeatResult tempResult = BeatResult.Fail;
        if (isKeyCorrect)
        {
            if (pressedTime <= eventRange[idx, 0])
            {
                tempResult = BeatResult.Fast;
            }
            else if (pressedTime <= eventRange[idx, 1])
            {
                tempResult = BeatResult.Perfect;
            }
            else
            {
                tempResult = BeatResult.Slow;
            }
        }
        resultArr[idx] = tempResult;
    }

    protected int IncreaseDeath()
    {
        deathCount++;
        return deathCount;
    }

    private void SummarizeResult()
    {
        // Count each result type in shortResult and longResult array
        foreach (BeatResult result in shortResult)
        {
            switch (result)
            {
                case BeatResult.Fail:
                    _shortSummary[0] += 1;
                    break;
                case BeatResult.Fast:
                    _shortSummary[1] += 1;
                    break;
                case BeatResult.Perfect:
                    _shortSummary[2] += 1;
                    break;
                case BeatResult.Slow:
                    _shortSummary[3] += 1;
                    break;
                default:
                    _shortSummary[0] += 1;
                    break;
            }
        }

        foreach (BeatResult result in longResult)
        {
            switch (result)
            {
                case BeatResult.Fail:
                    _longSummary[0] += 1;
                    break;
                case BeatResult.Fast:
                    _longSummary[1] += 1;
                    break;
                case BeatResult.Perfect:
                    _longSummary[2] += 1;
                    break;
                case BeatResult.Slow:
                    _longSummary[3] += 1;
                    break;
                default:
                    _longSummary[0] += 1;
                    break;
            }
        }

        _finalSummary[0] = _shortSummary[0] + _longSummary[0]; // Fail
        _finalSummary[1] = _shortSummary[1] + _longSummary[1]; // Fast
        _finalSummary[2] = _shortSummary[2] + _longSummary[2]; // Perfect
        _finalSummary[3] = _shortSummary[3] + _longSummary[3]; // Slow
    }

    private void RateResult(int stageIdx, int levelIdx)
    {
        // Load current level's data
        LevelData curLevelData = DataCenter.Instance.GetLevelData(stageIdx, levelIdx);
        curLevelData.fastCount = _finalSummary[1];
        curLevelData.perfectCount = _finalSummary[2];
        curLevelData.slowCount = _finalSummary[3];
        curLevelData.levelClear = true;
        // Push data into current level's data
        if (_finalSummary[2] == totalNoteCount)
        {
            gameUI.ShowStar(3);
            curLevelData.alpha = 1f;
            
            // Unlock Character
            DataCenter.Instance.GetStoreData().characterData[curLevelData.unlockCharNum].isUnlocked = true;
        }
        
        // generous condition for test: _finalSummary[2] >= totalNoteCount / 3
        else if (_finalSummary[2] >= totalNoteCount / 3 * 2)
        {
            curLevelData.star = 2;
            curLevelData.alpha = 2 / 3f;
            
            // Unlock Character
            DataCenter.Instance.GetStoreData().characterData[curLevelData.unlockCharNum].isUnlocked = true;
        }
        else
        {
            curLevelData.star = 1;
            curLevelData.alpha = 1 / 3f;
        }
        // Save updated level data into json file
        DataCenter.Instance.SaveData(curLevelData, stageIdx, levelIdx);
        
        if (levelIdx != 0 && levelIdx % 4 == 0)
        {
            //Debug.Log("stage clear");
            // boss game clear
            DataCenter.Instance.UpdateStageData(stageIdx);
            DataCenter.Instance.AddStageData();
            DataCenter.Instance.UpdatePlayerData(stageIdx + 2, 1, coinCount);
        }
        else
        {
            //Debug.Log("normal game clear");
            // normal game clear
            DataCenter.Instance.UpdatePlayerData(stageIdx + 1, levelIdx + 2, coinCount);
        }
    }

    public void PauseGame()
    {
        // Get current sample for RestartGame()
        curState = GameState.Pause;
        curSample = SoundManager.instance.musicPlayer.GetSampleTimeForClip(SoundManager.instance.clipName);
        SoundManager.instance.PlayBGM(false, curSample);
    }

    public void ContinueGame()
    {
        StartWithDelay(curSample);
    }
}
