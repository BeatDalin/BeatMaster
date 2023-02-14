using System;
using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class NormalGame : Game
{
    public ObjectGenerator objectGenerator;
    private ParticleController _particleController;

    [Header("Event Check")]
    private List<KoreographyEvent> _events;
    private int[,] _eventRangeShort;
    private int[,] _eventRangeLong;
    private int _pressedTime;
    private int _pressedTimeLong;
    private bool _isCheckedShort; // to prevent double check
    private bool _isCheckedAttack; // to prevent double check
    private bool _isCheckedLong; // to prevent double check
    private bool[] _isShortVisited; // A boolean array to check whether a short note has been visited before
    [Header("Input KeyCode")]
    private KeyCode _jumpNoteKey = KeyCode.LeftArrow;
    private KeyCode _attackNoteKey = KeyCode.RightArrow;
    private KeyCode _longNoteKey = KeyCode.LeftArrow;
    private List<KoreographyEvent> _shortEvent;
    [Header("Combo System")]
    private ComboSystem _comboSystem;
    private EffectAnim _playerAnim;
    private PlayerData _playerDatas;
    public bool IsLongPressed
    {
        get => isLongPressed;
        private set
        {
            if (isLongPressed != value)
            {
                isLongPressed = value;
            }
        }
    }


    protected override void Awake()
    {
        base.Awake();
        objectGenerator = FindObjectOfType<ObjectGenerator>();
        _particleController = FindObjectOfType<ParticleController>();
        _monsterPooling = FindObjectOfType<MonsterPooling>();
        _playerAnim = FindObjectOfType<EffectAnim>();
        _comboSystem = FindObjectOfType<ComboSystem>();
        // Save Point Event Track
        Koreographer.Instance.RegisterForEventsWithTime("Level1_CheckPoint", SaveCheckPoint);
        // Short Note Event Track
        Koreographer.Instance.RegisterForEventsWithTime("Level1_JumpCheck", CheckShortEnd);
        // Attack Note Event Track
        Koreographer.Instance.RegisterForEventsWithTime("Level1_AttackCheck", CheckAttackEnd);
        // Long Note Event Track
        Koreographer.Instance.RegisterForEvents("Level1_LongCheckMiddle", CheckLongMiddle);
        Koreographer.Instance.RegisterForEventsWithTime("Level1_LongCheckStart", CheckLongStart);
        Koreographer.Instance.RegisterForEventsWithTime("Level1_LongCheckEnd", CheckLongEnd);

        // Result Array
        _shortEvent = SoundManager.instance.playingKoreo.GetTrackByID("Level1_Short").GetAllEvents();
        shortResult = new BeatResult[_shortEvent.Count];
        longResult = new BeatResult[SoundManager.instance.playingKoreo.GetTrackByID("Level1_Long").GetAllEvents().Count];
        totalNoteCount = shortResult.Length + longResult.Length; // total number of note events

        _playerDatas = DataCenter.Instance.GetPlayerData();

        _playerAnim.ChangeCharacterAnim(_playerDatas.playerChar);
    }

    protected override void Start()
    {
        base.Start();
        //PlayerStatus.Instance.ChangeStatus(CharacterStatus.Idle);
        Init();
    }

    protected override void Init()
    {
        base.Init();
        // Need CurveEvent
        _events = SoundManager.instance.playingKoreo.GetTrackByID("Level1_Short").GetAllEvents();
        _isShortVisited = new bool[_events.Count];
        List<KoreographyEvent> rangeEventList = new List<KoreographyEvent>();

        for (int i = 0; i < _events.Count; i++)
        {
            KoreographyEvent ev = new KoreographyEvent();
            ev.StartSample = _events[i].StartSample - 5000;
            ev.EndSample = _events[i].EndSample + 5000;
            rangeEventList.Add(ev);
        }

        _eventRangeShort = CalculateRange(rangeEventList);
        _events = SoundManager.instance.playingKoreo.GetTrackByID("Level1_LongCheckEnd").GetAllEvents();
        _eventRangeLong = CalculateRange(_events);
        
    }

    private void CheckShortEnd(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (_isCheckedShort && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            _isCheckedShort = false; // initialize before a curve value becomes 1
        }

        if (!isShortKeyCorrect)
        {
            if (_shortEvent[shortIdx].GetIntValue() == 0 && Input.GetKeyDown(_jumpNoteKey) && !_characterMovement.isJumping)
            {
                // 숏노트 체크 
                Debug.Log(sampleTime);
                _comboSystem.IncreaseCombo();
                PlayerStatus.Instance.ChangeStatus(CharacterStatus.Attack);
                _particleController.PlayJumpParticle();
                isShortKeyCorrect = true;
                // Increase coin only once!
                if (!_isShortVisited[shortIdx])
                {
                    _isShortVisited[shortIdx] = true;
                    IncreaseItem();
                    gameUI.UpdateText(TextType.Item, coinCount);
                }
                _pressedTime = sampleTime; // record the sample time when the button was pressed
            }
        }

        // The end of checking event range
        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1 && !_isCheckedShort)
        {
            _isCheckedShort = true;
            CheckBeatResult(shortResult, shortIdx, isShortKeyCorrect, _pressedTime, _eventRangeShort);
            gameUI.ChangeOutLineColor(shortResult[shortIdx]);
            shortIdx++;
            if (!isShortKeyCorrect)
            {
                // ================Rewind 자리================
                // Rewind();
            }
            isShortKeyCorrect = false;
        }
    }

    private void CheckAttackEnd(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (_isCheckedAttack && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            _isCheckedAttack = false; // initialize before a curve value becomes 1
        }

        if (!isShortKeyCorrect)
        {
            if (_shortEvent[shortIdx].GetIntValue() == 1 && Input.GetKeyDown(_attackNoteKey))
            {
                _comboSystem.IncreaseCombo();
                _particleController.PlayJumpParticle();
                isShortKeyCorrect = true;
                // Increase coin only once!
                if (!_isShortVisited[shortIdx])
                {
                    _isShortVisited[shortIdx] = true;
                    IncreaseItem();
                    gameUI.UpdateText(TextType.Item, coinCount);
                }
                _pressedTime = sampleTime; // record the sample time when the button was pressed
                // 몬스터 삭제
            }
        }

        // The end of checking event range
        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1 && !_isCheckedAttack)
        {
            _isCheckedAttack = true;
            //Debug.Log($"AttackIdx: {shortIdx}");
            CheckBeatResult(shortResult, shortIdx, isShortKeyCorrect, _pressedTime, _eventRangeShort);
            gameUI.ChangeOutLineColor(shortResult[shortIdx]);
            _monsterPooling.DisableMonster();
            shortIdx++;
            if (!isShortKeyCorrect)
            {
                _monsterPooling.DisableMonster();
                // ================Rewind 자리================
                Rewind();
            }
            isShortKeyCorrect = false;
        }
    }

    private void CheckLongStart(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (_isCheckedLong && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            _isCheckedLong = false; // initialize before a curve value becomes 1
            //isLongFailed = false;
        }

        if (Input.GetKeyDown(_longNoteKey))
        {
            _comboSystem.IncreaseCombo();
            IsLongPressed = true;
            Debug.Log("Long Key Press");
            _playerAnim.SetEffectBool(true);
        }
        else if (Input.GetKeyUp(_longNoteKey))
        {
            _comboSystem.ResetCombo();
            IsLongPressed = false;
            Debug.Log("Long Key Up during CheckLongStart");
            _playerAnim.SetEffectBool(false);
        }

        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1f && !_isCheckedLong)
        {
            _isCheckedLong = true;
            if (!IsLongPressed) // Failed to press at the start of the long note
            {
                _comboSystem.ResetCombo();
                _playerAnim.SetEffectBool(false);
                //=======Rewind 자리=========
                //isLongFailed = true; // for testing purpose... death 카운트 3번 올라가는 거 방지하려고
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
            _comboSystem.ResetCombo();
            _playerAnim.SetEffectBool(false);
            //==============Rewind 자리==============
            // Rewind();
            
            //if (!isLongFailed)
            //{
                // Rewind( ); // for testing purpose... death 카운트 3번 올라가는 거 방지하려고}
                //isLongFailed = true; // for testing purpose... death 카운트 3번 올라가는 거 방지하려고
            //}
        }
        else if (IsLongPressed)
        {
            _comboSystem.IncreaseComboInProcess(evt.StartSample);
        }
    }
    private void CheckLongEnd(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (_isCheckedLong && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            _isCheckedLong = false; // initialize before a curve value becomes 1
        }
        if (IsLongPressed && Input.GetKeyUp(_longNoteKey))
        {
            if (!isLongKeyCorrect) // increase item only once
            {
                Debug.Log("End Key Up => Correct!");
                _comboSystem.IncreaseCombo();
                _comboSystem.ResetCurrentAmount();
                PlayerStatus.Instance.ChangeStatus(CharacterStatus.Attack);
                isLongKeyCorrect = true;
                IncreaseItem();
                gameUI.UpdateText(TextType.Item, coinCount);

                _pressedTimeLong = sampleTime;
                _playerAnim.SetEffectBool(false);
            }
        }

        // The end of checking event range
        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1f && !_isCheckedLong)
        {
            _isCheckedLong = true;
            CheckBeatResult(longResult, longIdx, isLongKeyCorrect, _pressedTimeLong, _eventRangeLong); // Record Result
            longIdx++;
            if (!isLongKeyCorrect)
            {
                _playerAnim.SetEffectBool(false);
                Debug.Log("End Key Fail!!!");
                // ===============Rewind==============
                // Rewind();
                //if (!isLongFailed)
                //{
                    // Rewind(); // for testing purpose... death 카운트 3번 올라가는 거 방지하려고
                //}
            }

            IsLongPressed = false;
            isLongKeyCorrect = false;
            _playerAnim.SetEffectBool(false);
        }
    }

    private void Rewind()
    {
        PlayerStatus.Instance.ChangeStatus(CharacterStatus.Damage);
        curState = GameState.Pause;
        SoundManager.instance.PlayBGM(false); // pause
        curSample = rewindSampleTime;
        _playerAnim.SetEffectBool(false); // Stop booster animation
        
        _characterMovement.RewindPosition(); // Relocate player
        ContinueGame(); // wait 3 sec and start
        // Item, Death, Combo
        DecreaseItem(5);
        gameUI.UpdateText(TextType.Item, coinCount);
        gameUI.UpdateText(TextType.Death, IncreaseDeath());// increase death count
        _comboSystem.ResetCombo();
        _comboSystem.ResetCurrentAmount();
        // Reset Array Index
        shortIdx = rewindShortIdx;
        longIdx = rewindLongIdx;
        // Obstacle
        objectGenerator.ResetObstAnimation();
    }

    private void IncreaseItem()
    {
        coinCount++;
    }

    private void DecreaseItem(int amount)
    {
        coinCount -= amount;
        if (coinCount < 0)
        {
            coinCount = 0;
        }
    }

    private void SaveCheckPoint(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        Debug.Log($"SaveCheckPoint: {evt}");
        if (sampleTime > rewindSampleTime)
        {
            Debug.Log($"SaveCheckPoint: Sample {sampleTime} > Rewind {rewindSampleTime}");
            // DisableMonster Clear
            // sampleTime = 0 이면 첫시작이므로 ResetPool 안해도됨
            if (evt.StartSample != 0)
            {
                _monsterPooling.ResetPool();
            }
            // Entered new check point
            // checkPointIdx++;
            // Record sample time to play music
            // rewindSampleTime = checkPointList[checkPointIdx].StartSample;
            
            // checkPointVisited[checkPointIdx] = true;
            rewindSampleTime = objectGenerator.MoveCheckPointForward();
            // Debug.Log(rewindSampleTime);
            // Play Particle or Animation
            
            // objectGenerator.PlayCheckAnim(checkPointIdx);
            
            
            // Record Index
            rewindShortIdx = shortIdx;
            rewindLongIdx = longIdx;
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Rewind();
        }
    }
}