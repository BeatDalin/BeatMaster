using System;
using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class NormalGame : Game
{
    private ParticleController _particleController;
    private ResourcesChanger _resourcesChanger;
    private TouchInputManager _touchInputManager;

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
    private PlayerData _playerDatas;

    [SerializeField]
    private ChangeCharSprite _changeChar;

    private int _rewindCount=0;

    protected override void Awake()
    {
        base.Awake();
        objectGenerator = FindObjectOfType<ObjectGenerator>();
        monsterPooling = FindObjectOfType<MonsterPooling>();
        _particleController = FindObjectOfType<ParticleController>();
        _resourcesChanger = FindObjectOfType<ResourcesChanger>();
        _playerAnim = FindObjectOfType<EffectAnim>();
        _comboSystem = FindObjectOfType<ComboSystem>();
        _touchInputManager = FindObjectOfType<TouchInputManager>();
        // Short Note Event Track
        Koreographer.Instance.RegisterForEventsWithTime(jumpCheckID, CheckJumpEnd);
        // Attack Note Event Track
        Koreographer.Instance.RegisterForEventsWithTime(attackCheckID, CheckAttackEnd);
        // Long Note Event Track
        Koreographer.Instance.RegisterForEvents(longCheckMiddleID, CheckLongMiddle);
        Koreographer.Instance.RegisterForEventsWithTime(longCheckStartID, CheckLongStart);
        Koreographer.Instance.RegisterForEventsWithTime(longCheckEndID, CheckLongEnd);

        // Result Array
        _shortEvent = SoundManager.instance.playingKoreo.GetTrackByID(shortID).GetAllEvents();
        shortResult = new BeatResult[_shortEvent.Count];
        longResult = new BeatResult[SoundManager.instance.playingKoreo.GetTrackByID(longID).GetAllEvents().Count];
        totalNoteCount = shortResult.Length + longResult.Length; // total number of note events

        _playerDatas = DataCenter.Instance.GetPlayerData();

        _playerAnim.ChangeCharacterAnim(_playerDatas.playerChar);
        _changeChar.ChangeItemInItemScroll(_playerDatas);
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
        _events = SoundManager.instance.playingKoreo.GetTrackByID(shortID).GetAllEvents();
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
        
        _events = SoundManager.instance.playingKoreo.GetTrackByID(longCheckStartID).GetAllEvents();
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
            if (_shortEvent[shortIdx].GetIntValue() == 0 && !characterMovement.isJumping && (Input.GetKeyDown(_jumpNoteKey) || _touchInputManager.CheckLeftTouch()))
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
            mapTemp.shortTileParticleList[shortIdx].GetComponent<Note>().beatResult = shortResult[shortIdx].ToString();
            rewindTime.RecordCheckPoint(characterMovement.transform.position, shortResult[shortIdx].ToString());
            //gameUI.ChangeOutLineColor(shortResult[shortIdx]);
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
            if (_shortEvent[shortIdx].GetIntValue() == 1 && (_touchInputManager.CheckRightTouch() || Input.GetKeyDown(_attackNoteKey)))
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
            }
        }

        // The end of checking event range
        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1 && !_isCheckedAttack)
        {
            _isCheckedAttack = true;
            CheckBeatResult(shortResult, shortIdx, isShortKeyCorrect, _pressedTime, _eventRangeShort);
            mapTemp.shortTileParticleList[shortIdx].GetComponent<Note>().beatResult = shortResult[shortIdx].ToString();
            rewindTime.RecordCheckPoint(characterMovement.transform.position, shortResult[shortIdx].ToString());
            //gameUI.ChangeOutLineColor(shortResult[shortIdx]);
            monsterPooling.DisableMonster();
            _isShortVisited[shortIdx] = true;
            if (!isRewinding)
            {
                // increase index only when Rewind() has not been called
                shortIdx++;
            }
            if (!isShortKeyCorrect)
            {
                // ================Rewind 자리================
                Rewind();
            }
            isShortKeyCorrect = false;
        }
    }

    private void CheckLongStart(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (!characterMovement.isLongNote)
        {
            characterMovement.isLongNote = true; // block jump for now
        }

        if (_isCheckedLong && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            _isCheckedLong = false; // initialize before a curve value becomes 1
        }

        if (_touchInputManager.CheckLeftTouch() || Input.GetKeyDown(_longNoteKey))
        {
            isLongPressed = true;
            _comboSystem.IncreaseCombo();
            Debug.Log("Long Key Press");
            PlayerStatus.Instance.ChangeStatus(CharacterStatus.FastIdle);
            _playerAnim.SetEffectBool(true);
        }
        else if (_touchInputManager.CheckLeftTouchEnd() || Input.GetKeyUp(_longNoteKey))
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
        if (isLongPressed)
        {
            if (_touchInputManager.CheckLeftTouching() || Input.GetKey(_longNoteKey))
            {
                // Keep Touching ...
                _comboSystem.IncreaseComboInProcess(evt.StartSample);
            }
            else
            {
                // TouchPhase.Began or TouchPhase.End
                isLongPressed = false;
                Debug.Log("Middle KeyUP => Fail!!!");
                _playerAnim.SetEffectBool(false);
                //==============Rewind 자리==============
                Rewind();
            }
        }
    }
    private void CheckLongEnd(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (_isCheckedLong && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            _isCheckedLong = false; // initialize before a curve value becomes 1
        }
        if (isLongPressed && (_touchInputManager.CheckLeftTouchEnd() || Input.GetKeyUp(_longNoteKey)))
        {
            if (!isLongKeyCorrect)
            {
                isLongKeyCorrect = true;
                Debug.Log("End Key Up => Correct!");
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
            mapTemp.longTileParticleList[longIdx].GetComponent<Note>().beatResult = longResult[longIdx].ToString();
            _isLongVisited[longIdx] = true;
            if (!isRewinding)
            {
                // increase index only when Rewind() has not been called
                PlayerStatus.Instance.ChangeStatus(CharacterStatus.Run);
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
            characterMovement.isLongNote = false; // Can jump after a long note ends
        }
    }

    private void Rewind()
    {
        isRewinding = true;
        PlayerStatus.Instance.ChangeStatus(CharacterStatus.Damage);
        curState = GameState.Rewind;
        SoundManager.instance.PlayBGM(false); // pause
        curSample = rewindSampleTime;
        _playerAnim.SetEffectBool(false); // Stop booster animation
        characterMovement.RewindPosition(); // Relocate player
        characterMovement.isLongNote = false;
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
        Achievement achieve = DataCenter.Instance.GetAchievementData();
        if ((_rewindCount += 1) == 100)
        {
            GPGSBinder.Instance.UnlockAchievement(GPGSIds.achievement_restart_over_hundred, success => achieve.isRestartedOverHundred = true);
        }
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