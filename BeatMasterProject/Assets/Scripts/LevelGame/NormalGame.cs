using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalGame : Game
{
    BeatResult _tempLResult;
    BeatResult _tempSResult;

    static public int count;
    bool _isChecked; // to prevent double check


    protected override void Awake()
    {
        base.Awake();
        Init();
        // Short Note Event Track
        Koreographer.Instance.RegisterForEventsWithTime("JumpCheck", CheckShortJump);
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

    void CheckShortJump(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if(_isChecked && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            // 1 �Ǳ� ���� _isChecked true�̸� �ʱ�ȭ �ʿ�
            _isChecked = false; // initialize
        }
        if (!isShortKeyCorrect && Input.GetKeyDown(KeyCode.LeftArrow)) // short key false�� ���� ��� �˻�, true�̸� �˻��� �ʿ� ����
        {
            isShortKeyCorrect = true;
        }

        // The end of checking event range
        if(evt.GetValueOfCurveAtTime(sampleTime) == 1f && !_isChecked)
        {
            _isChecked= true;
            CheckBeatResult(false);
            Debug.Log($"SampleTime {shortIdx}: {sampleTime} || SampleDelta: {sampleDelta} || Key {isShortKeyCorrect}");
            shortIdx++;

            isShortKeyCorrect = false;

            
            if (_tempSResult == BeatResult.Fail)
            {
                // ================Rewind ȣ�� ��ġ================
            }

        }
    }

    void CheckLongMiddle(KoreographyEvent evt)
    {
        // if space key is released during long note
        if (isLongPressed && Input.GetKeyUp(KeyCode.Space))
        {
            isLongPressed = false;
            //==============Rewind ȣ����ġ==============
            Debug.Log("during long jump -> key up!!!");
            return;
        }

    }
    void CheckLongStart(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (_isChecked && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            // 1 �Ǳ� ���� _isChecked true�̸� �ʱ�ȭ �ʿ�
            _isChecked = false; // initialize
        }
        if (Input.GetKeyDown(KeyCode.Space))
        {
            isLongPressed = true;
        }

        if (evt.GetValueOfCurveAtTime(sampleTime) == 1f && !_isChecked)
        {
            _isChecked = true;
            if (!isLongPressed) // Failed to press at the start of the long note
            {
                //=======Rewindȣ��=========
            }
        }
    }
    void CheckLongEnd(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (_isChecked && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            // 1 �Ǳ� ���� _isChecked true�̸� �ʱ�ȭ �ʿ�
            _isChecked = false; // initialize
        }
        if (isLongPressed && Input.GetKeyUp(KeyCode.Space))
        {
            if (!isLongKeyCorrect) // increase item only once
            {
                // correct!
                isLongKeyCorrect = true;
                IncreaseItem();
            }
            //Debug.Log($"Long Jump & Space Up~!~! isLongKeyEnd:{isLongKeyCorrect} count: {count}");

        }

        // The end of checking event range
        if (evt.GetValueOfCurveAtTime(sampleTime) == 1f && !_isChecked)
        {
            Debug.Log($"Long Jump Ended~~~~~ {sampleTime}");
            _isChecked = true;
            CheckBeatResult(true); // Record Result
            longIdx++;
            isLongPressed = false;
            isLongKeyCorrect = false;
        }
    }

    void Rewind(Vector2 goBackPos, int musicSampleTime)
    {
        // longIdx, shortIdx üũ ����Ʈ ���� ��Ʈ�� �������� -> longResult ���� ����� �� �ְ�
        // Player ��ġ ��������
        // üũ ����Ʈ ���ķ� ȹ���� ������ ���� ���, ��ŭ decrease item
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

    public override void CheckBeatResult(bool isLongNote)
    {
        if (isLongNote)
        {
            Debug.Log(longIdx);
            _tempLResult = isLongKeyCorrect ? BeatResult.Perfect : BeatResult.Fail;
            longResult[longIdx] = _tempLResult;
        }
        else
        {
            _tempSResult = isShortKeyCorrect ? BeatResult.Perfect : BeatResult.Fail;
            shortResult[shortIdx] = _tempSResult;
        }

        if( CheckFinish() )
        {
            Debug.Log("Game Ended");
        }
    }

}
