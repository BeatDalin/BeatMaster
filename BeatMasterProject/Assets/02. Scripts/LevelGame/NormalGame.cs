using System;
using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class NormalGame : Game
{
    public MapTemp mapTemp;
    private ParticleController _particleController;
    [Header("Event Check")]
    private List<KoreographyEvent> _events;
    private int[,] _eventRangeShort;
    private int[,] _eventRangeLong;
    private int _pressedTime;
    private int _pressedTimeLong;
    private bool _isChecked; // to prevent double check
    [Header("Input KeyCode")]
    private KeyCode _jumpNoteKey = KeyCode.LeftArrow;
    private KeyCode _attackNoteKey = KeyCode.RightArrow;
    private KeyCode _longNoteKey = KeyCode.LeftArrow;
    [Header("MonsterPool")] 
    private MonsterPooling _monsterPooling;
    private CharacterMovement _characterMovement;
    [Header("SpriteChanger")]
    private SpriteChanger _spriteChanger;
    
    public bool IsLongPressed
    {
        get => isLongPressed;
        private set
        {
            if (isLongPressed != value)
            {
                isLongPressed = value;
                // TODO
                _spriteChanger.OnLongPressed();
            }
        }
    }
    

    protected override void Awake()
    {
        base.Awake();
        _particleController = FindObjectOfType<ParticleController>();
        _monsterPooling = FindObjectOfType<MonsterPooling>();
        _characterMovement = FindObjectOfType<CharacterMovement>();
        _spriteChanger = FindObjectOfType<SpriteChanger>();
        // Save Point Event Track
        Koreographer.Instance.RegisterForEventsWithTime("Level1_CheckPoint", SaveCheckPoint);
        // Short Note Event Track
        Koreographer.Instance.RegisterForEventsWithTime("Level1_JumpCheck", CheckShortEnd);
        // Long Note Event Track
        Koreographer.Instance.RegisterForEvents("Level1_LongCheckMiddle", CheckLongMiddle);
        Koreographer.Instance.RegisterForEventsWithTime("Level1_LongCheckStart", CheckLongStart);
        Koreographer.Instance.RegisterForEventsWithTime("Level1_LongCheckEnd", CheckLongEnd);
        
        // Result Array
        shortResult = new BeatResult[SoundManager.instance.playingKoreo.GetTrackByID("Level1_JumpCheck").GetAllEvents().Count];
        longResult = new BeatResult[SoundManager.instance.playingKoreo.GetTrackByID("Level1_Long").GetAllEvents().Count];
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
    }

    private void CheckShortEnd(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    { 
        if(_isChecked && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            _isChecked = false; // initialize before a curve value becomes 1
        }

        if (!isShortKeyCorrect)
        {
            _particleController.PlayJumpParticle();
            if (evt.GetIntValue() == 0 && Input.GetKeyDown(_jumpNoteKey))
            {
                isShortKeyCorrect = true;
                IncreaseItem();
                gameUI.UpdateText(TextType.Item, coinCount);
                _pressedTime = sampleTime; // record the sample time when the button was pressed
            }
            else if (evt.GetIntValue() == 1 && Input.GetKeyDown(_attackNoteKey))
            {
                isShortKeyCorrect = true;
                IncreaseItem();
                gameUI.UpdateText(TextType.Item, coinCount);
                _pressedTime = sampleTime; // record the sample time when the button was pressed
                // 몬스터 삭제
            }
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
                _monsterPooling.DisableMonster();
                // ================Rewind 자리================
                // Rewind();
            }
            else
            {
                _monsterPooling.DisableMonster();
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
            IsLongPressed = true;
            Debug.Log("Long Key Press");
        }

        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1f && !_isChecked)
        {
            _isChecked = true;
            if (!IsLongPressed) // Failed to press at the start of the long note
            {
                //=======Rewind 자리=========
                isLongFailed = true; // for testing purpose... death 카운트 3번 올라가는 거 방지하려고
                // Rewind(); // for testing purpose... death 카운트 3번 올라가는 거 방지하려고
            }
        }
    }
    private void CheckLongMiddle(KoreographyEvent evt)
    {
        // if space key is released during long note
        if (IsLongPressed && Input.GetKeyUp(_longNoteKey))
        {
            IsLongPressed = false;
            Debug.Log("Middle KeyUP => Fail!!!");

            //==============Rewind 자리==============
            if (!isLongFailed) 
            {
                // Rewind( ); // for testing purpose... death 카운트 3번 올라가는 거 방지하려고}
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
        if (IsLongPressed && Input.GetKeyUp(_longNoteKey))
        {
            if (!isLongKeyCorrect) // increase item only once
            {
                Debug.Log("End Key Up => Correct!");

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
                
                Debug.Log("End Key Fail!!!");
                // ===============Rewind==============
                if (!isLongFailed)
                {
                    // Rewind(); // for testing purpose... death 카운트 3번 올라가는 거 방지하려고
                }
            }
            
            IsLongPressed = false;
            isLongKeyCorrect = false;
        }
    }
    
    private void Rewind()
    {
        curState = GameState.Pause;
        SoundManager.instance.PlayBGM(false); // pause
        curSample = rewindSampleTime;
        _monsterPooling.ReArrange();
        //curSample = (int)_monsterPooling.currentPlayerTime;
        _characterMovement.RewindPosition();
        ContinueGame(); // wait 3 sec and start
        DecreaseItem(5);
        gameUI.UpdateText(TextType.Item, coinCount);
        int death = IncreaseDeath(); // increase dea    th count
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
        if (sampleTime > rewindSampleTime)
        {
            // DisableMonster Clear
            // sampleTime = 0 이면 첫시작이므로 ResetPool 안해도됨
            if (evt.StartSample != 0)
            {
                _monsterPooling.ResetPool();
            }
            // Record sample time to play music
            rewindSampleTime = sampleTime;
            Debug.Log(rewindSampleTime);
            // Entered new check point
            checkPointIdx++;
            checkPointVisited[checkPointIdx] = true;
            // Play Particle or Animation
            // ex) particleSystem.Play();
            mapTemp.PlayCheckAnim(checkPointIdx);
        }
        // Record Index
        rewindShortIdx = shortIdx;
        rewindLongIdx = longIdx;
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Rewind();
        }
    }
}
