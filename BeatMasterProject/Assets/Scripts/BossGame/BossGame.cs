using System;
using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo.Demos;

public class BossGame : Game
{
    [Header("Event Check")]
    private List<KoreographyEvent> _events;
    private int[,] _eventRangeShort;
    private int[,] _eventRangeLong;

    private int _pressedTime;
    private int _pressedTimeLong;

    private bool _isChecked; // to prevent double check

    private RhythmGameController _rhythmGameController;
    private LaneController _laneController;
    private NoteCreator _noteCreator;
    private bool _isStart;
    protected override void Awake()
    {
        base.Awake();
        // Short Note Event Track
        Koreographer.Instance.RegisterForEventsWithTime("BossCheck", CheckShortEnd);

        // Long Note Event Track
        /*Koreographer.Instance.RegisterForEvents("LongJumpMiddle", CheckLongMiddle);
        Koreographer.Instance.RegisterForEventsWithTime("LongJumpCheckStart", CheckLongStart);
        Koreographer.Instance.RegisterForEventsWithTime("LongJumpCheckEnd", CheckLongEnd);*/
        
        // Result Array
        shortResult = new BeatResult[SoundManager.instance.playingKoreo.GetTrackByID("BossCheck").GetAllEvents().Count];
        //longResult = new BeatResult[SoundManager.instance.playingKoreo.GetTrackByID("LongJump").GetAllEvents().Count];
        totalNoteCount = shortResult.Length + longResult.Length; // total number of note events
    }

    protected override void Start()
    {
        base.Start();

        _rhythmGameController = GetComponent<RhythmGameController>();
        _laneController = GameObject.Find("Target").GetComponent<LaneController>();
        _noteCreator = GameObject.Find("NoteCreator").GetComponent<NoteCreator>();
        StartCoroutine(CoCheckStart());
        PlayerStatus.Instance.ChangeStatus(Status.Idle);
        Init();
    }

    private void CheckGameState()
    {
        if (curState == GameState.Play)
        {
            _isStart = true;
        }
    }

    private IEnumerator CoCheckStart()
    {
        while (!_isStart)
        {
            CheckGameState();
            yield return null;
        }
        foreach (var lanes in _rhythmGameController.noteLanes)
        {
            lanes.enabled = true;
        }
        PlayerStatus.Instance.ChangeStatus(Status.Run);
    }

    protected override void Init()
    {
        base.Init();
        _events = SoundManager.instance.playingKoreo.GetTrackByID("BossCheck").GetAllEvents();
        _eventRangeShort = CalculateRange(_events);
        /*_events = SoundManager.instance.playingKoreo.GetTrackByID("LongJumpCheckEnd").GetAllEvents();
        _eventRangeLong = CalculateRange(_events);*/
        itemCount = 0;
        gameUI.InitUI();
    }

    private void CheckShortEnd(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (_isChecked && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            _isChecked = false; // initialize before a curve value becomes 1
        }
        if (!isShortKeyCorrect && Input.GetKeyDown(KeyCode.LeftArrow))
        {
            isShortKeyCorrect = true;
            // gameUI.UpdateText(TextType.Item, itemCount);
            _pressedTime = sampleTime; // record the sample time when the button was pressed
            _laneController.trackedNotes.Peek().CorrectHit();
            _noteCreator.ReturnLastObject();
        }

        // The end of checking event range
        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1 && !_isChecked)
        {
            _isChecked = true;
            CheckBeatResult(shortResult, shortIdx, isShortKeyCorrect, _pressedTime, _eventRangeShort);
            shortIdx++;
            if (!isShortKeyCorrect)
            {
                PlayerStatus.Instance.DecreaseHP();
                _laneController.trackedNotes.Peek().Missed();

                _noteCreator.ReturnLastObject();
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
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            isLongPressed = true;
        }

        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1f && !_isChecked)
        {
            _isChecked = true;
            if (!isLongPressed) // Failed to press at the start of the long note
            {
                isLongFailed = true; // for testing purpose... death 카운트 3번 올라가는 거 방지하려고
                PlayerStatus.Instance.DecreaseHP();
            }
        }
    }
    private void CheckLongMiddle(KoreographyEvent evt)
    {
        if (isLongPressed && Input.GetKeyUp(KeyCode.RightArrow))
        {
            isLongPressed = false;
            if (!isLongFailed)
            {
                PlayerStatus.Instance.DecreaseHP();
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
        if (isLongPressed && Input.GetKeyUp(KeyCode.RightArrow))
        {
            if (!isLongKeyCorrect) // increase item only once
            {
                // correct!
                isLongKeyCorrect = true;
                //gameUI.UpdateText(TextType.Item, itemCount);

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
                if (!isLongFailed)
                {
                    PlayerStatus.Instance.DecreaseHP();
                }
            }

            isLongPressed = false;
            isLongKeyCorrect = false;
        }
    }
}
