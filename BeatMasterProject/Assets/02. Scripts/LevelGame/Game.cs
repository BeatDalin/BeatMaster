using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

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
}

public abstract class Game : MonoBehaviour
{
    [SerializeField] protected GameUI gameUI; // LevelGameUI or BossGameUI will come in.
    [SerializeField] [EventID] private string _mapEventID;

    [Header("Game Play")]
    public GameState curState = GameState.Idle;
    public int curSample;

    [Header("Result Check")]
    public BeatResult[] longResult;
    public BeatResult[] shortResult;
    protected static int totalNoteCount = 0;
    // long notes
    protected int longIdx = 0;
    protected bool isLongPressed = false;
    protected bool isLongKeyCorrect = false;
    // short notes
    protected int shortIdx = 0;
    //protected bool isShortKeyPressed = false; // To prevent double check...
    protected bool isShortKeyCorrect = false;

    // death
    [SerializeField] protected int deathCount = 0;
    protected bool isLongFailed = false; // for testing purpose ....

    [Header("Data")]
    [SerializeField] private int _stageIdx; // Stage number-1 : This is an index!!!
    [SerializeField] private int _levelIdx; // Level number-1 : This is an index!!!
    public int itemCount;
    
    private int[] _longSummary = new int[4]; // Record the number of Fail, Fast, Perfect, Slow results from short notes
    private int[] _shortSummary = new int[4]; // Record the number of Fail, Fast, Perfect, Slow results from long notes
    private int[] _finalSummary = new int[4]; // Summed number of short note & long note results for each result type

    protected virtual void Awake()
    {
        gameUI = FindObjectOfType<GameUI>(); // This will get LevelGameUI or BossGameUI object
        gameUI.InitUI();
        itemCount = 0;
        DataCenter.Instance.LoadData();
    }

    protected virtual void Start()
    {
        StartWithDelay();

        Koreographer.Instance.RegisterForEvents(_mapEventID, CheckEnd);
    }
    
    protected virtual void Init()
    {
        longIdx = 0;
        shortIdx = 0;
        isLongPressed = false;
        isLongKeyCorrect = false;
    }

    private void CheckEnd(KoreographyEvent evt)
    {
        int endEvent = evt.GetTextValue().Split().Select(int.Parse).ToArray()[3];

        if (endEvent == 2)
        {
            SummarizeResult();
            RateResult(_stageIdx, _levelIdx);
            gameUI.ShowFinalResult(_finalSummary, totalNoteCount, _stageIdx, _levelIdx); // for testing purpose ...
        }
        if (endEvent == 3)
        {
            PlayerStatus.Instance.ChangeStatus(Status.Idle);
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

        //if (CheckFinish())
        //{
        //    SummarizeResult();
        //    RateResult(_stageIdx, _levelIdx);
        //    gameUI.ShowFinalResult(_finalSummary, totalNoteCount, _stageIdx, _levelIdx); // for testing purpose ...
        //}
    }
    protected void StartWithDelay(int startSample = 0)
    {
        StartCoroutine(CoStartWithDelay(startSample));
    }

    protected IEnumerator CoStartWithDelay(int startSample = 0)
    {
        // UI Timer
        gameUI.timePanel.SetActive(true);
        int waitTime = 3;
        while (waitTime > 0)
        {
            gameUI.UpdateText(TextType.Time, waitTime);
            waitTime--;
            yield return new WaitForSeconds(1);
        }
        gameUI.timePanel.SetActive(false);
        // Music Play & Game Start
        startSample = startSample < 0 ? 0 : startSample; // if less than zero, set as zero

        SoundManager.instance.PlayBGM(true, startSample);
        curState = GameState.Play;
        PlayerStatus.Instance.ChangeStatus(Status.Run);
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
    
    //protected bool CheckFinish()
    //{
    //    // If index becomes the length of Arrays (or length -1), the game has been ended.
    //    if (isEndPoint)
    //    {
    //        return true;
    //    }
    //    return false;
    //}

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
        }
        else if (_finalSummary[2] >= totalNoteCount / 3 * 2)
        {
            curLevelData.star = 2;
            curLevelData.alpha = 2 / 3f;
        }
        else
        {
            curLevelData.star = 1;
            curLevelData.alpha = 1 / 3f;
        }
        // Save updated level data into json file
        DataCenter.Instance.SaveData(curLevelData, stageIdx, levelIdx);

        if (levelIdx == 4)
        {
            // boss game clear
            DataCenter.Instance.UpdateStageData(stageIdx);
            DataCenter.Instance.AddStageData();
            DataCenter.Instance.UpdatePlayerData(stageIdx + 2, 1, itemCount);
        }
        else
        {
            // normal game clear
            DataCenter.Instance.UpdatePlayerData(stageIdx + 1, levelIdx + 2, itemCount);
        }
    }

    public void PauseGame()
    {
        // Get current sample for RestartGame()
        curSample = SoundManager.instance.musicPlayer.GetSampleTimeForClip(SoundManager.instance.clipName);
        SoundManager.instance.PlayBGM(false);
        curState = GameState.Pause;
    }

    public void ContinueGame()
    {
        StartWithDelay(curSample);
    }
}
