using System;
using SonicBloom.Koreo;
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

public enum GameState
{
    Idle,
    Play,
    Pause,
    Rewind,
    End
}

public abstract class Game : MonoBehaviour
{
    [SerializeField] protected GameUI gameUI; // LevelGameUI or BossGameUI will come in.
    protected CharacterMovement characterMovement;
    protected MonsterPooling monsterPooling;
    protected EffectAnim _playerAnim;
    private LeaderboardManager _leaderboardManager;

    [Header("Game Play")]
    public GameState curState = GameState.Idle;
    public int curSample;
    protected bool isRewinding;
    [Header("Check Game End")]
    [SerializeField][EventID] private string _spdEventID;

    [Header("Check Point")]
    [SerializeField][EventID] private string _checkPointID;
    [SerializeField] protected int rewindShortIdx;
    [SerializeField] protected int rewindLongIdx;
    [SerializeField] protected int rewindSampleTime = -1;
    protected ObjectGenerator objectGenerator;

    [Header("Result Check")]
    public BeatResult[] longResult;
    public BeatResult[] shortResult;
    protected static int totalNoteCount = 0;
    // long notes
    [SerializeField] protected int longIdx = 0;
    protected bool isLongPressed = false;
    protected bool isLongKeyCorrect = false;
    // short notes (jump, attack)
    [SerializeField] protected int shortIdx = 0;
    protected bool isShortKeyCorrect = false;
    // death
    [SerializeField] protected int deathCount = 0;

    [Header("Data")]
    [SerializeField] private int _stageIdx; // Stage number-1 : This is an index!!!
    [SerializeField] private int _levelIdx; // Level number-1 : This is an index!!!
    public int coinCount;

    private int[] _longSummary = new int[4]; // Record the number of Fail, Fast, Perfect, Slow results from short notes
    private int[] _shortSummary = new int[4]; // Record the number of Fail, Fast, Perfect, Slow results from long notes
    private int[] _finalSummary = new int[4]; // Summed number of short note & long note results for each result type

    [Header("Rewind")]
    protected RewindTime rewindTime;
    protected MapGenerator mapGenerator;

    [Header("Event Tracks")]
    [SerializeField][EventID] protected string shortID;
    [SerializeField][EventID] protected string longID;
    [SerializeField][EventID] protected string jumpCheckID;
    [SerializeField][EventID] protected string attackCheckID;
    [SerializeField][EventID] protected string longCheckMiddleID;
    [SerializeField][EventID] protected string longCheckStartID;
    [SerializeField][EventID] protected string longCheckEndID;

    protected virtual void Awake()
    {
        mapGenerator = FindObjectOfType<MapGenerator>();
        rewindTime = FindObjectOfType<RewindTime>();
        _playerAnim = FindObjectOfType<EffectAnim>();
        characterMovement = FindObjectOfType<CharacterMovement>();
        monsterPooling = FindObjectOfType<MonsterPooling>();
        objectGenerator = FindObjectOfType<ObjectGenerator>();
        gameUI = FindObjectOfType<GameUI>(); // This will get LevelGameUI or BossGameUI object
        _leaderboardManager = FindObjectOfType<LeaderboardManager>();
        Koreographer.Instance.ClearEventRegister(); // Initialize Koreographer Event Regiser
        // Save Point Event Track
        Koreographer.Instance.RegisterForEventsWithTime(_checkPointID, SaveCheckPoint);

        // Data
        DataCenter.Instance.LoadData();
    }

    protected virtual void Start()
    {
        // StartWithDelay();
        StartCoroutine(CoStartWithDelay(0));
        Koreographer.Instance.RegisterForEvents(_spdEventID, CheckEnd);
    }

    protected virtual void Init()
    {
        longIdx = 0;
        shortIdx = 0;
        isLongPressed = false;
        isLongKeyCorrect = false;
        coinCount = 0;
        deathCount = 0;
        rewindShortIdx = 0;
        rewindLongIdx = 0;
        rewindSampleTime = -1;
        isRewinding = false;
    }

    protected void StartWithDelay(int startSample = 0)
    {
        StartCoroutine(SceneLoadManager.Instance.CoSceneEnter());
        StartCoroutine(CoStartWithDelay(startSample));
    }

    protected IEnumerator CoStartWithDelay(int startSample = 0)
    {
        _playerAnim.SetEffectBool(false);
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

        // Wait for Scene Transition to end
        // yield return new WaitWhile(() => !SceneLoadManager.Instance.GetTransitionEnd());
        yield return new WaitUntil(() => SceneLoadManager.Instance.isLoaded);

        if (rewindTime.isRewind)
        {
            while (!rewindTime.isRecord)
            {
                yield return null;
            }
        }
        gameUI.timePanel.SetActive(true);

        int waitTime = 3;
        while (waitTime > 0)
        {
            gameUI.UpdateText(TextType.Time, waitTime);
            waitTime--;
            if (waitTime == 1 && curState.Equals(GameState.Rewind))
            {
                // Activate Monster
                monsterPooling.ReArrange();
            }
            yield return new WaitForSeconds(1);
        }
        gameUI.timePanel.SetActive(false);

        // // Rewind Character Position
        // if (curState.Equals(GameState.Rewind))
        // {
        //     characterMovement.RewindPosition();
        // }
        curState = GameState.Play;
        PlayerStatus.Instance.ChangeStatus(CharacterStatus.Run);
        isRewinding = false;

        // Music Play & Game Start
        startSample = startSample < 0 ? 0 : startSample; // if less than zero, set as zero
        SoundManager.instance.PlayBGM(true, startSample);

    }

    private void SaveCheckPoint(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (evt.StartSample > rewindSampleTime)
        {
            // Entered new check point
            Debug.Log($"SaveCheckPoint: Sample {sampleTime} > Rewind {rewindSampleTime}");
            // DisableMonster Clear
            if (evt.StartSample != 0)
            {
                monsterPooling.ResetPool();
                //rewindTime.ClearRewindList();
            }
            // Record sample time to play music
            curSample = objectGenerator.MoveCheckPointForward();
            rewindSampleTime = curSample;

            // Record Index
            rewindShortIdx = shortIdx;
            rewindLongIdx = longIdx;
        }
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
            gameUI.UpdateText(TextType.Death, deathCount);// increase death count
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

        //LeaderboardData leaderboard=DataCenter.Instance.GetLeaderboardData(stageIdx, levelIdx);

        Achievement achieve = DataCenter.Instance.GetAchievementData();
#if !UNITY_EDITOR
        if (achieve.playCount <= 10)
        {
            if ((achieve.playCount += 1) == 1)
            {
                GPGSBinder.Instance.UnlockAchievement(GPGSIds.achievement_clear_once, success => print("achievement_clear_once"));
            }
            else if (achieve.playCount == 10)
            {
                GPGSBinder.Instance.UnlockAchievement(GPGSIds.achievement_clear_10_times, success => print("achievement_clear_10_times"));
            }
        }
#endif
        // Push data into current level's data
        if (_finalSummary[2] == totalNoteCount)
        {
            curLevelData.star = 3;
            //curLevelData.alpha = 1f;
            gameUI.ShowStar(3);

            // Unlock Character
            DataCenter.Instance.GetStoreData().characterData[curLevelData.unlockCharNum].isUnlocked = true;
#if !UNITY_EDITOR
            if (achieve.isMaster == false)
            {
                GPGSBinder.Instance.UnlockAchievement(GPGSIds.achievement_master, success => achieve.isMaster = true);
            }
#endif
        }

        // generous condition for test: _finalSummary[2] >= totalNoteCount / 3
        else if (_finalSummary[2] >= totalNoteCount / 3 * 2)
        {
            curLevelData.star = curLevelData.star > 2 ? curLevelData.star : 2;
            //curLevelData.alpha = 2 / 3f;
            gameUI.ShowStar(2);

            // Unlock Character
            Debug.Log(curLevelData.unlockCharNum);
            DataCenter.Instance.GetStoreData().characterData[curLevelData.unlockCharNum].isUnlocked = true;
#if !UNITY_EDITOR
            if (achieve.isGrown == false)
            {
                GPGSBinder.Instance.UnlockAchievement(GPGSIds.achievement_grown, success => achieve.isGrown = true);
            }
#endif
        }
        else
        {
            curLevelData.star = curLevelData.star > 1 ? curLevelData.star : 1;
            //curLevelData.alpha = 1 / 3f;
            gameUI.ShowStar(1);
#if !UNITY_EDITOR
            if (achieve.isStarted == false)
            {
                GPGSBinder.Instance.UnlockAchievement(GPGSIds.achievement_first_one_star, success => achieve.isStarted = true);
            }
#endif
        }

        //leaderboard.score = _leaderboardManager.CalculateScore(curLevelData.star, deathCount);
        int score = _leaderboardManager.CalculateScore(curLevelData.star, deathCount);
#if !UNITY_EDITOR
        _leaderboardManager.ReportScore(stageIdx, levelIdx, score);
#endif

        // Save updated level data into json file
        //DataCenter.Instance.SaveData(curLevelData, stageIdx, levelIdx, leaderboard);
        DataCenter.Instance.SaveData(curLevelData, stageIdx, levelIdx);
        // Unlock Next Level
        if (levelIdx < 3)
        {
            LevelData nextLevelData = DataCenter.Instance.GetLevelData(stageIdx, levelIdx + 1);
            nextLevelData.isUnlocked = true;

            //DataCenter.Instance.SaveData(nextLevelData, stageIdx, levelIdx + 1, leaderboard);
            DataCenter.Instance.SaveData(nextLevelData, stageIdx, levelIdx + 1);
        }
        else
        {
            Debug.Log("Stage Clear");
        }

        // if (levelIdx != 0 && levelIdx % 4 == 0)
        // {
        //     //Debug.Log("stage clear");
        //     // boss game clear
        //     DataCenter.Instance.UpdateStageData(stageIdx);
        //     DataCenter.Instance.AddStageData();
        //     DataCenter.Instance.UpdatePlayerData(stageIdx + 2, 1, coinCount);
        // }
        DataCenter.Instance.UpdatePlayerData(stageIdx + 1, levelIdx + 2, coinCount);


    }

    public void PauseGame()
    {
        // Get current sample for RestartGame()
        curState = GameState.Pause;
        PlayerStatus.Instance.ChangeStatus(CharacterStatus.Idle);
        curSample = SoundManager.instance.musicPlayer.GetSampleTimeForClip(SoundManager.instance.clipName);
        SoundManager.instance.PlayBGM(false, curSample);
    }

    public void ContinueGame()
    {
        StartWithDelay(curSample);
    }
}
