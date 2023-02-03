using System;
using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NormalGame : Game
{
    [Header("Event Check")]
    private List<KoreographyEvent> _events;
    private int[,] _eventRangeShort;
    private int[,] _eventRangeLong;
    private int _pressedTime;
    private int _pressedTimeLong;
    private bool _isChecked; // to prevent double check
    [Header("Input KeyCode")]
    private KeyCode _shortNoteKey = KeyCode.LeftArrow;
    private KeyCode _longNoteKey = KeyCode.RightArrow;
    protected override void Awake()
    {
        base.Awake();
        // Save Point Event Track
        Koreographer.Instance.RegisterForEventsWithTime("Level1_Spd", SaveCheckPoint);
        // Short Note Event Track
        Koreographer.Instance.RegisterForEventsWithTime("Level1_JumpCheck", CheckShortEnd);
        // Long Note Event Track
        // Koreographer.Instance.RegisterForEvents("LongJumpMiddle", CheckLongMiddle);
        // Koreographer.Instance.RegisterForEventsWithTime("LongJumpCheckStart", CheckLongStart);
        // Koreographer.Instance.RegisterForEventsWithTime("LongJumpCheckEnd", CheckLongEnd);
        
        // Result Array
        shortResult = new BeatResult[SoundManager.instance.playingKoreo.GetTrackByID("Level1_JumpCheck").GetAllEvents().Count];
        // longResult = new BeatResult[SoundManager.instance.playingKoreo.GetTrackByID("LongJump").GetAllEvents().Count];
        totalNoteCount = shortResult.Length + longResult.Length; // total number of note events
        
    }

    protected override void Start()
    {
        base.Start();
        PlayerStatus.Instance.ChangeStatus(Status.Idle);
        Init();
    }

    protected override void Init()
    {
        base.Init();
        _events = SoundManager.instance.playingKoreo.GetTrackByID("Level1_JumpCheck").GetAllEvents();
        _eventRangeShort = CalculateRange(_events);
        _events = SoundManager.instance.playingKoreo.GetTrackByID("LongJumpCheckEnd").GetAllEvents();
        _eventRangeLong = CalculateRange(_events);
        // Save Point Initialize
        checkPointVisited = new bool[savePointList.Count];
        checkPointVisited[0] = true;
    }

    private void CheckShortEnd(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    { 
        if(_isChecked && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            _isChecked = false; // initialize before a curve value becomes 1
        }
        if (!isShortKeyCorrect && Input.GetKeyDown(_shortNoteKey))
        {
            isShortKeyCorrect = true;
            IncreaseItem();
            gameUI.UpdateText(TextType.Item, coinCount);
            _pressedTime = sampleTime; // record the sample time when the button was pressed
        }

        // The end of checking event range
        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1 && !_isChecked)
        {
            _isChecked= true;
            CheckBeatResult(shortResult, shortIdx, isShortKeyCorrect, _pressedTime, _eventRangeShort);
            gameUI.ChangeOutLineColor(shortResult[shortIdx]);
            shortIdx++;
            if (!isShortKeyCorrect)
            {
                // ================Rewind 자리================
                Rewind(Vector2.zero, sampleTime-50000);
            }
            isShortKeyCorrect = false;
        }
    }

    private void CheckLongStart(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (_isChecked && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            _isChecked = false; // initialize before a curve value becomes 1
            isLongFailed = false;
        }
        if (Input.GetKeyDown(_longNoteKey))
        {
            isLongPressed = true;
        }

        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1f && !_isChecked)
        {
            _isChecked = true;
            if (!isLongPressed) // Failed to press at the start of the long note
            {
                //=======Rewind 자리=========
                isLongFailed = true; // for testing purpose... death 카운트 3번 올라가는 거 방지하려고
                Rewind(Vector2.zero, sampleTime); // for testing purpose... death 카운트 3번 올라가는 거 방지하려고
            }
        }
    }
    private void CheckLongMiddle(KoreographyEvent evt)
    {
        // if space key is released during long note
        if (isLongPressed && Input.GetKeyUp(_longNoteKey))
        {
            isLongPressed = false;
            //==============Rewind 자리==============
            if (!isLongFailed) 
            {
                Rewind( ); // for testing purpose... death 카운트 3번 올라가는 거 방지하려고}
                isLongFailed = true; // for testing purpose... death 카운트 3번 올라가는 거 방지하려고
            }
        }
    }
    private void CheckLongEnd(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (_isChecked && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            _isChecked = false; // initialize before a curve value becomes 1
        }
        if (isLongPressed && Input.GetKeyUp(_longNoteKey))
        {
            if (!isLongKeyCorrect) // increase item only once
            {
                isLongKeyCorrect = true;
                IncreaseItem();
                gameUI.UpdateText(TextType.Item, coinCount);

                _pressedTimeLong = sampleTime;
            }
        }

        // The end of checking event range
        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1f && !_isChecked)
        {
            _isChecked = true;
            CheckBeatResult(longResult, longIdx, isLongKeyCorrect, _pressedTimeLong, _eventRangeLong); // Record Result
            longIdx++;
            if (!isLongKeyCorrect)
            {
                // ===============Rewind==============
                if (!isLongFailed)
                {
                    Rewind(Vector2.zero, sampleTime); // for testing purpose... death 카운트 3번 올라가는 거 방지하려고
                }
            }
            
            isLongPressed = false;
            isLongKeyCorrect = false;
        }
    }

    private void Rewind(Vector2 goBackPos, int musicSampleTime) //, bool isShort
    {
        // SoundManager.instance.PlayBGM(false, musicSampleTime);
        // character move stop
        
        // index update: longIdx, shortIdx 체크 포인트 다음 노트로 돌려놓음 -> longResult 새로 기록할 수 있게
        // if (isShort)
        // {
        //     shortIdx--;
        // }
        // else
        // {
        //     longIdx--;
        // }
        // Player 위치 돌려놓음
        
        // 체크 포인트 이후로 획득한 아이템 개수 계산
        DecreaseItem(1); // for testing purpose ... 
        gameUI.UpdateText(TextType.Item, coinCount);
        int death = IncreaseDeath(); // increase death count
        gameUI.UpdateText(TextType.Death, death);
        // StartCoroutine(CoStartWithDelay(musicSampleTime)); // plays music after delay, at a certain point
    }

    private void Rewind()
    {
        SoundManager.instance.PlayBGM(false); // pause
        curSample = rewindSampleTime;
        ContinueGame(); // wait 3 sec and start
        DecreaseItem(5);
        gameUI.UpdateText(TextType.Item, coinCount);
        int death = IncreaseDeath(); // increase death count
        gameUI.UpdateText(TextType.Death, death);
        shortIdx = rewindShortIdx;
        longIdx = rewindLongIdx;
    }

    private void IncreaseItem()
    {
        coinCount++;
    }

    private void DecreaseItem(int amount)
    {
        coinCount -= amount;
        if(coinCount < 0)
        {
            coinCount = 0;
        }
    }

    private void SaveCheckPoint(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        // Record sample time to play music
        rewindSampleTime = sampleTime;
        if (sampleTime > rewindSampleTime)
        {
            // Entered new check point
            checkPointIdx++;
            checkPointVisited[checkPointIdx] = true;
            // Play Particle
            // ex) particleSystem.Play();
        }
        // Record Index
        rewindShortIdx = shortIdx;
        rewindLongIdx = longIdx;
    }
    
    // public override void CheckBeatResult(BeatResult[] resultArr, BeatResult tempResult, int idx, bool isKeyCorrect, int pressedTime, int[,] eventRange)
    // {
    //     if (isKeyCorrect)
    //     {
    //         if (pressedTime <= eventRange[idx, 0])
    //         {
    //             tempResult = BeatResult.Fast;
    //         }
    //         else if (pressedTime <= eventRange[idx, 1])
    //         {
    //             tempResult = BeatResult.Perfect;
    //         }
    //         else
    //         {
    //             tempResult = BeatResult.Slow;
    //         }
    //     }
    //     else
    //     {
    //         tempResult = BeatResult.Fail;
    //     }
    //     resultArr[idx] = tempResult;
    //
    //     if ( CheckFinish() )
    //     {
    //         Debug.Log("Game Ended");
    //     }
    // }
    
}
