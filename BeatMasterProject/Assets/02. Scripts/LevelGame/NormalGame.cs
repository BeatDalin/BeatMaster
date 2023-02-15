using System;
using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalGame : Game
{
    private ParticleController _particleController;
    private ResourcesChanger _resourcesChanger;

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
    private bool[] _isLongVisited; // A boolean array to check whether a long note has been visited before
    [Header("Input KeyCode")]
    private KeyCode _jumpNoteKey = KeyCode.LeftArrow;
    private KeyCode _attackNoteKey = KeyCode.RightArrow;
    private KeyCode _longNoteKey = KeyCode.LeftArrow;
    private List<KoreographyEvent> _shortEvent;
    [Header("Combo System")]
    private ComboSystem _comboSystem;
    private EffectAnim _playerAnim;
    private PlayerData _playerDatas;

    protected override void Awake()
    {
        base.Awake();
        objectGenerator = FindObjectOfType<ObjectGenerator>();
        _particleController = FindObjectOfType<ParticleController>();
        _resourcesChanger = FindObjectOfType<ResourcesChanger>();
        monsterPooling = FindObjectOfType<MonsterPooling>();
        _playerAnim = FindObjectOfType<EffectAnim>();
        _comboSystem = FindObjectOfType<ComboSystem>();
        // Short Note Event Track
        Koreographer.Instance.RegisterForEventsWithTime("Level1_JumpCheck", CheckJumpEnd);
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
        Init();
    }

    protected override void Init()
    {
        base.Init();
        // Need Curve Event to execute CalculateRange()
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
        _isLongVisited = new bool[+_events.Count];

    }

    private void CheckJumpEnd(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (_isCheckedShort && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            _isCheckedShort = false; // initialize before a curve value becomes 1
        }

        if (!isShortKeyCorrect)
        {
            if (_shortEvent[shortIdx].GetIntValue() == 0 && Input.GetKeyDown(_jumpNoteKey) && !characterMovement.isJumping)
            {
                isShortKeyCorrect = true;
                PlayerStatus.Instance.ChangeStatus(CharacterStatus.Attack);
                _comboSystem.IncreaseCombo();
                _particleController.PlayJumpParticle();
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
                Rewind();
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
            monsterPooling.DisableMonster();
            _isShortVisited[shortIdx] = true;
            if (!isRewinding)
            {
                // increase index only when Rewind() has not been called
                shortIdx++;
            }
            if (!isShortKeyCorrect)
            {
                monsterPooling.DisableMonster();
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
        }

        if (Input.GetKeyDown(_longNoteKey))
        {
            isLongPressed = true;
            _comboSystem.IncreaseCombo(); 
            Debug.Log("Long Key Press");
            PlayerStatus.Instance.ChangeStatus(CharacterStatus.FastIdle);
            _playerAnim.SetEffectBool(true);
        }
        else if (Input.GetKeyUp(_longNoteKey))
        {
            isLongPressed = false;
            _comboSystem.ResetCombo(); // erase it later
            Debug.Log("Long Key Up during CheckLongStart");
            _playerAnim.SetEffectBool(false);
        }

        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1f && !_isCheckedLong)
        {
            _isCheckedLong = true;
            if (!isLongPressed) // Failed to press at the start of the long note
            {
                //==============Rewind 자리==============
                Rewind();
            }
        }
    }
    private void CheckLongMiddle(KoreographyEvent evt)
    {
        // if action key is released during long note
        if (isLongPressed && !Input.GetKey(_longNoteKey))
        {
            isLongPressed = false;
            Debug.Log("Middle KeyUP => Fail!!!");
            _playerAnim.SetEffectBool(false);
            //==============Rewind 자리==============
            Rewind();
        }
        else if (isLongPressed)
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
        if (isLongPressed && Input.GetKeyUp(_longNoteKey))
        {
            if (!isLongKeyCorrect)
            {
                isLongKeyCorrect = true;
                Debug.Log("End Key Up => Correct!");
                PlayerStatus.Instance.ChangeStatus(CharacterStatus.Attack);
                // Combo
                _comboSystem.IncreaseCombo();
                _comboSystem.ResetCurrentAmount();
                // Increase coin only once!
                if (!_isLongVisited[longIdx])
                {
                    _isLongVisited[longIdx] = true;
                    IncreaseItem();
                    gameUI.UpdateText(TextType.Item, coinCount);
                }
                _pressedTimeLong = sampleTime;
                _playerAnim.SetEffectBool(false);
            }
        }

        // The end of checking event range
        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1f && !_isCheckedLong)
        {
            _isCheckedLong = true;
            CheckBeatResult(longResult, longIdx, isLongKeyCorrect, _pressedTimeLong, _eventRangeLong); // Record Result
            PlayerStatus.Instance.ChangeStatus(CharacterStatus.Run);
            _isLongVisited[longIdx] = true;
            if (!isRewinding)
            {
                // increase index only when Rewind() has not been called
                longIdx++;
            }
            if (!isLongKeyCorrect)
            {
                Debug.Log("End Key Fail!!!");
                // ===============Rewind==============
                Rewind();
            }
            isLongPressed = false;
            isLongKeyCorrect = false;
        }
    }

    private void Rewind()
    {
        isRewinding = true;
        PlayerStatus.Instance.ChangeStatus(CharacterStatus.Damage);
        curState = GameState.Pause;
        SoundManager.instance.PlayBGM(false); // pause
        curSample = rewindSampleTime;
        _playerAnim.SetEffectBool(false); // Stop booster animation
        characterMovement.RewindPosition(); // Relocate player
        ContinueGame(); // wait 3 sec and start
        // Item, Death, Combo
        gameUI.UpdateText(TextType.Item, DecreaseItem(5));
        IncreaseDeath(); // increase death count
        _comboSystem.ResetCombo();
        _comboSystem.ResetCurrentAmount();
        // Reset Array Index
        shortIdx = rewindShortIdx;
        longIdx = rewindLongIdx;
        // Obstacle
        objectGenerator.ResetObstAnimation();
        // Post Processing
        _resourcesChanger.ResetPostProcessing();
    }

    private void IncreaseItem()
    {
        coinCount++;
    }

    private int DecreaseItem(int amount)
    {
        coinCount -= amount;
        if (coinCount < 0)
        {
            coinCount = 0;
        }
        return coinCount;
    }

    
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Rewind();
        }
    }
}