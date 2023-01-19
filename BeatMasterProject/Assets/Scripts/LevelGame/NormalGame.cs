using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalGame : Game
{
    private BeatResult _tempLResult;
    private BeatResult _tempSResult;

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
        count = 0;
    }

    void CheckShortEnd(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if(_isChecked && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            _isChecked = false; // initialize before a curve value becomes 1
        }
        if (!isShortKeyCorrect && Input.GetKeyDown(KeyCode.LeftArrow)) // short key false�� ���� ��� �˻�, true�̸� �˻��� �ʿ� ����
        {
            isShortKeyCorrect = true;
        }

        // The end of checking event range
        if(evt.GetValueOfCurveAtTime(sampleTime) >= 1 && !_isChecked)
        {
            _isChecked= true;
            CheckBeatResult(shortResult, _tempSResult, shortIdx, isShortKeyCorrect);
            Debug.Log($"SampleTime {shortIdx}: {sampleTime} || SampleDelta: {sampleDelta} || Key {isShortKeyCorrect}");
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
            Debug.Log("during long jump -> key up!!!");
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
            }

        }

        // The end of checking event range
        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1f && !_isChecked)
        {
            Debug.Log($"Long Jump Ended~~~~~ {sampleTime}");
            _isChecked = true;
            CheckBeatResult(longResult, _tempLResult, longIdx, isLongKeyCorrect); // Record Result
            longIdx++;
            isLongPressed = false;
            isLongKeyCorrect = false;
        }
    }

    void Rewind(Vector2 goBackPos, int musicSampleTime)
    {
        // longIdx, shortIdx 체크 포인트 다음 노트로 돌려놓음 -> longResult 새로 기록할 수 있게
        // Player 위치 돌려놓음
        // 체크 포인트 이후로 획득한 아이템 개수 계산, 만큼 decrease item
        //DecreaseItem()
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

    public override void CheckBeatResult(BeatResult[] resultArr, BeatResult tempResult, int idx, bool isKeyCorrect)
    {
        tempResult = isKeyCorrect ? BeatResult.Perfect : BeatResult.Fail;
        resultArr[idx] = tempResult;

        if( CheckFinish() )
        {
            Debug.Log("Game Ended");
        }
    }

}
