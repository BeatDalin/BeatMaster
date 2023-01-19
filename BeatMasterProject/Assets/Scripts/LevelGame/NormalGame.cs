using System;
using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalGame : Game
{
    private BeatResult _tempLResult;
    private BeatResult _tempSResult;
    private List<KoreographyEvent> _events;
    private int[,] _eventRangeShort;
    private int[,] _eventRangeLong;

    private int _pressedTime;
    private int _pressedTimeLong;
    static public int count;
    private bool _isChecked; // to prevent double check


    protected override void Awake()
    {
        base.Awake();
        Init();
        // Short Note Event Track
        Koreographer.Instance.RegisterForEventsWithTime("JumpCheck", CheckShortEnd);
        
        // Long Note Event Track
        Koreographer.Instance.RegisterForEvents("LongJump", CheckLongMiddle);
        Koreographer.Instance.RegisterForEventsWithTime("LongJumpCheckStart", CheckLongStart);
        Koreographer.Instance.RegisterForEventsWithTime("LongJumpCheckEnd", CheckLongEnd);
    }
    protected override void Start()
    {
        base.Start();
    }

    protected override void Init()
    {
        base.Init();
        _events = playingKoreo.GetTrackByID("JumpCheck").GetAllEvents();
        _eventRangeShort = CalculateRange(_events);
        _events = playingKoreo.GetTrackByID("LongJumpCheckEnd").GetAllEvents();
        _eventRangeLong = CalculateRange(_events);
        count = 0;
    }

    int[,] CalculateRange(List<KoreographyEvent> koreographyEvents)
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

    void CheckShortEnd(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    { 
        if(_isChecked && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            _isChecked = false; // initialize before a curve value becomes 1
        }
        if (!isShortKeyCorrect && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            isShortKeyCorrect = true;
            _pressedTime = sampleTime; // record the sample time when the button was pressed
        }

        // The end of checking event range
        if(evt.GetValueOfCurveAtTime(sampleTime) >= 1 && !_isChecked)
        {
            _isChecked= true;
            CheckBeatResult(shortResult, _tempSResult, shortIdx, isShortKeyCorrect, _pressedTime, _eventRangeShort);
            shortIdx++;

            isShortKeyCorrect = false;

            if (_tempSResult == BeatResult.Fail)
            {
                // ================Rewind 자리================
            }

        }
    }

    void CheckLongStart(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (_isChecked && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            _isChecked = false; // initialize before a curve value becomes 1
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isLongPressed = true;
        }

        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1 && !_isChecked)
        {
            _isChecked = true;
            if (!isLongPressed) // Failed to press at the start of the long note
            {
                //=======Rewind 자리=========
            }
        }
    }
    void CheckLongMiddle(KoreographyEvent evt)
    {
        // if space key is released during long note
        if (isLongPressed && Input.GetKeyUp(KeyCode.Space))
        {
            isLongPressed = false;
            //==============Rewind 자리==============
        }
    }
    void CheckLongEnd(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (_isChecked && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            _isChecked = false; // initialize before a curve value becomes 1
        }
        if (isLongPressed && Input.GetKeyUp(KeyCode.Space))
        {
            if (!isLongKeyCorrect) // increase item only once
            {
                // correct!
                isLongKeyCorrect = true;
                IncreaseItem();
                _pressedTimeLong = sampleTime;
            }
        }

        // The end of checking event range
        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1f && !_isChecked)
        {
            _isChecked = true;
            CheckBeatResult(longResult, _tempLResult, longIdx, isLongKeyCorrect,_pressedTimeLong, _eventRangeLong); // Record Result
            longIdx++;
            isLongPressed = false;
            isLongKeyCorrect = false;
        }
    }

    void Rewind(Vector2 goBackPos, int musicSampleTime)
    {
        // music stop, character move stop
        // longIdx, shortIdx 체크 포인트 다음 노트로 돌려놓음 -> longResult 새로 기록할 수 있게
        // Player 위치 돌려놓음
        // 체크 포인트 이후로 획득한 아이템 개수 계산, 만큼 decrease item
        //DecreaseItem()
        
        // music 다시 시작
    }


    void IncreaseItem()
    {
        count++;
    }

    void DecreaseItem(int amount)
    {
        count -= amount;
        if(count < 0)
        {
            count = 0;
        }
    }

    public override void CheckBeatResult(BeatResult[] resultArr, BeatResult tempResult, int idx, bool isKeyCorrect, int pressedTime, int[,] eventRange)
    {
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
        else
        {
            tempResult = BeatResult.Fail;
        }
        resultArr[idx] = tempResult;

        if( CheckFinish() )
        {
            Debug.Log("Game Ended");
        }
    }

}
