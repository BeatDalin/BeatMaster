using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BeatResult
{
    Fail,
    Fast,
    Perfect,
    Slow,
}
public abstract class Game : MonoBehaviour
{
    protected UIExperiment uiExp;
    protected Koreography playingKoreo;
    protected SimpleMusicPlayer musicPlayer;

    /// <summary>
    /// To use sample time of event tracks, this should be written first. Used for KoreographyEventCallbackWithTime.
    /// </summary>
    /// <param name="koreoEvent">Each KoreographyEvent</param>
    /// <param name="sampleTime">The current time for this KoreographyEvent.</param>
    /// <param name="sampleDelta">The number of samples that were played back since the previous frame. You can get the previous frame’s sampleTimewith(sampleTime-sampleDelta).</param>
    /// <param name="deltaSlice">Extra timing information required for simulation stability when the callback is called multiple times in a frame.</param>
    public delegate void KoreographyEventCallbackWithTime(KoreographyEvent koreoEvent, int sampleTime, int sampleDelta, DeltaSlice deltaSlice);
    
    [Header("Result Check")]
    public BeatResult[] longResult;
    public BeatResult[] shortResult;
 
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
    private static int _totalNoteCount = 0;

    #region Abstract Method
    //public abstract void CheckBeatResult(BeatResult[] resultArr, BeatResult tempResult, int idx, bool isKeyCorrect, int pressedTime, int[,] eventRange);
    //protected abstract void CalculateScore();
    #endregion

    protected virtual void Awake()
    {
        playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);
        musicPlayer = FindObjectOfType<SimpleMusicPlayer>();// Find ui manager
        uiExp = FindObjectOfType<UIExperiment>(); // temporal use of ui experiment class
        DataCenter.Instance.LoadData();
    }

    protected virtual void Start()
    {
        StartWithDelay();
    }
    
    protected virtual void Init()
    {
        musicPlayer.LoadSong(playingKoreo, 0, false);
        longResult = new BeatResult[playingKoreo.GetTrackByID("LongJump").GetAllEvents().Count];
        shortResult = new BeatResult[playingKoreo.GetTrackByID("Jump").GetAllEvents().Count];
        longIdx = 0;
        shortIdx = 0;
        isLongPressed = false;
        isLongKeyCorrect = false;
        _totalNoteCount = shortResult.Length + longResult.Length; // total number of note events
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

        if (CheckFinish())
        {
            Debug.Log("Game Ended");
            SummarizeResult();
            uiExp.ShowFinalResult(_finalSummary, _totalNoteCount); // for testing purpose ...
            RateResult(_stageIdx, _levelIdx);
        }
    }
    protected void StartWithDelay()
    {
        StartCoroutine(CoStartWithDelay());
    }

    protected IEnumerator CoStartWithDelay(int startSample = 0)
    {
        // UI 업데이트 할 거라면 이쪽에서 호출 가능
        Debug.Log("wait for playing...");
        yield return new WaitForSeconds(3);
        startSample = startSample < 0 ? 0 : startSample; // if less than zero, set as zero

        Debug.Log("Start...!");
        musicPlayer.LoadSong(playingKoreo, startSample);
        musicPlayer.Play();
    }

    protected bool CheckFinish()
    {
        // If index becomes the length of Arrays (or length -1), the game has been ended.
        if (shortIdx >= shortResult.Length-1 && longIdx >= longResult.Length-1)
        {
            return true;
        }
        return false;
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
        if (_finalSummary[2] == _totalNoteCount)
        {
            curLevelData.star = 3;
            curLevelData.alpha = 1f;
        }
        else if (_finalSummary[2] >= _totalNoteCount / 3 * 2)
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
}
