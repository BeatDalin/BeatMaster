using System;
using SonicBloom.Koreo;
using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private bool _isAutoPlay = false;
    
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
        playerAnim = FindObjectOfType<EffectAnim>();
        _comboSystem = FindObjectOfType<ComboSystem>();
        _touchInputManager = FindObjectOfType<TouchInputManager>();
        
        isTutorial = SceneLoadManager.Instance.Scene == SceneLoadManager.SceneType.Stage1_Level1; 
        
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

        playerAnim.ChangeCharacterAnim(_playerDatas.playerChar);
        if (!isTutorial)
        {
            feverTimeController.SetPlayerIndex(_playerDatas.playerChar);
        }
        
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
        
        _events = SoundManager.instance.playingKoreo.GetTrackByID(longCheckEndID).GetAllEvents();
        _eventRangeLong = CalculateRange(_events);
        _isLongVisited = new bool[_events.Count];

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
                _pressedTime = sampleTime; // record the sample time when the button was pressed
                ShortNoteComplete();
            }
            else if (_isAutoPlay && _shortEvent[shortIdx].GetIntValue() == 0 && sampleTime > _eventRangeShort[shortIdx, 0] && sampleTime <= _eventRangeShort[shortIdx,1])
            {
                _pressedTime = sampleTime; // record the sample time when the button was pressed
                characterMovement.Jump();
                ShortNoteComplete();
            }
        }

        // The end of checking event range
        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1 && !_isCheckedShort)
        {
            _isCheckedShort = true;
            CheckBeatResult(shortResult, shortIdx, isShortKeyCorrect, _pressedTime, _eventRangeShort);

            if (!isTutorial)
            {
                CompareFeverTime(shortResult, shortIdx);
            }
            
            mapGenerator.shortTileParticleList[shortIdx].beatResult = shortResult[shortIdx].ToString();
            rewindTime.RecordRewindPoint(characterMovement.transform.position, shortResult[shortIdx].ToString());
            gameUI.ChangeOutLineColor(shortResult[shortIdx]);
            Vibration(shortResult[shortIdx]);
            //gameUI.ChangeOutLineColor(shortResult[shortIdx]);
            shortIdx++;
            if (!isShortKeyCorrect)
            {
                // ================Rewind 자리================
                Rewind();
                if (!isTutorial)
                {
                    feverTimeController.Reset();
                }
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
                _pressedTime = sampleTime; // record the sample time when the button was pressed
                SoundManager.instance.PlaySFX("Hit");
                ShortNoteComplete();
            }
            else if (_isAutoPlay && _shortEvent[shortIdx].GetIntValue() == 1 && sampleTime > _eventRangeShort[shortIdx, 0] && sampleTime <= _eventRangeShort[shortIdx,1])
            {
                _pressedTime = sampleTime; // record the sample time when the button was pressed
                SoundManager.instance.PlaySFX("Hit");
                ShortNoteComplete();
            }
        }

        // The end of checking event range
        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1 && !_isCheckedAttack)
        {
            _isCheckedAttack = true;
            CheckBeatResult(shortResult, shortIdx, isShortKeyCorrect, _pressedTime, _eventRangeShort);

            if (!isTutorial)
            {
                CompareFeverTime(shortResult ,shortIdx);
            }
            
            mapGenerator.shortTileParticleList[shortIdx].beatResult = shortResult[shortIdx].ToString();
            rewindTime.RecordRewindPoint(characterMovement.transform.position, shortResult[shortIdx].ToString());
            Vibration(shortResult[shortIdx]);
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
                // ================Rewind 자리================
                Rewind();
                if (!isTutorial)
                {
                    feverTimeController.Reset();
                }
            }
            isShortKeyCorrect = false;
        }
    } 
    
    private void ShortNoteComplete()
    {
        isShortKeyCorrect = true;
        _comboSystem.IncreaseCombo();
        _particleController.PlayJumpParticle();

        // Increase coin only once!
        if (!_isShortVisited[shortIdx])
        {
            _isShortVisited[shortIdx] = true;
            IncreaseItem();
            gameUI.UpdateText(TextType.Item, coinCount);
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

        if (_touchInputManager.CheckLeftTouch() || Input.GetKeyDown(_longNoteKey) || _isAutoPlay)
        {
            isLongPressed = true;
            _comboSystem.IncreaseCombo();
#if UNITY_EDITOR
            Debug.Log("Long Key Press");
#endif
            PlayerStatus.Instance.ChangeStatus(CharacterStatus.FastIdle);
            playerAnim.SetEffectBool(true);
        }
        else if (_touchInputManager.CheckLeftTouchEnd() || Input.GetKeyUp(_longNoteKey))
        {
            if (!_isAutoPlay)
            {
                isLongPressed = false;
                _comboSystem.ResetCombo(); // erase it later
#if UNITY_EDITOR
                Debug.Log("Long Key Up during CheckLongStart");
#endif
                playerAnim.SetEffectBool(false);
            }
        }
        
        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1f && !_isCheckedLong)
        {
            _isCheckedLong = true;
            rewindTime.RecordRewindPoint(characterMovement.transform.position, longResult[longIdx].ToString(),
                true);
            if (!isLongPressed) // Failed to press at the start of the long note
            {
                //==============Rewind 자리==============
                Rewind();
                if (!isTutorial)
                {
                    feverTimeController.Reset();
                }
            }
        }
    }

    private void CheckLongMiddle(KoreographyEvent evt)
    {
        // if action key is released during long note
        if (isLongPressed)
        {
            if (_touchInputManager.CheckLeftTouching() || Input.GetKey(_longNoteKey) || _isAutoPlay)
            {
                // Keep Touching ...
                _comboSystem.IncreaseComboInProcess(evt.StartSample);
            }
            else
            {
                // TouchPhase.Began or TouchPhase.End
                isLongPressed = false;
#if UNITY_EDITOR
                Debug.Log("Middle KeyUP => Fail!!!");
#endif
                playerAnim.SetEffectBool(false);
                //==============Rewind 자리==============
                Rewind();
                if (!isTutorial)
                {
                    feverTimeController.Reset();
                }
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
#if UNITY_EDITOR
                Debug.Log("End Key Up => Correct!");
#endif
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
                playerAnim.SetEffectBool(false);
            }
        }
        else if (_isAutoPlay)
        {
            if (sampleTime > _eventRangeLong[longIdx, 0] && sampleTime <= _eventRangeLong[longIdx, 1])
            {
                isLongKeyCorrect = true;
#if UNITY_EDITOR
                Debug.Log("End Key Up => Correct!");
#endif
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
                playerAnim.SetEffectBool(false);
            }
        }

        // The end of checking event range
        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1f && !_isCheckedLong)
        {
            _isCheckedLong = true;
            CheckBeatResult(longResult, longIdx, isLongKeyCorrect, _pressedTimeLong, _eventRangeLong); // Record Result
            
            if (!isTutorial)
            {
                CompareFeverTime(longResult, longIdx);
            }

            mapGenerator.longTileParticleList[longIdx].beatResult = longResult[longIdx].ToString();
            rewindTime.RecordRewindPoint(characterMovement.transform.position, longResult[longIdx].ToString(), false);
            Vibration(longResult[longIdx]);
            _isLongVisited[longIdx] = true;
            if (!isRewinding)
            {
                // increase index only when Rewind() has not been called
                PlayerStatus.Instance.ChangeStatus(CharacterStatus.Run);
                longIdx++;
            }
            if (!isLongKeyCorrect)
            {
#if UNITY_EDITOR
                Debug.Log("End Key Fail!!!");
#endif
                // ===============Rewind==============
                Rewind();
                if (!isTutorial)
                {
                    feverTimeController.Reset();
                }
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
        SoundManager.instance.PlaySFX("Rewind");
        curSample = rewindSampleTime;
        playerAnim.SetEffectBool(false); // Stop booster animation
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
#if !UNITY_EDITOR
        if ((_rewindCount += 1) == 100)
        {
            GPGSBinder.Instance.UnlockAchievement(GPGSIds.achievement_restart_over_hundred, success => achieve.isRestartedOverHundred = true);
        }
#endif
    }

    private void Vibration(BeatResult beatResult)
    {
        if (beatResult.Equals(BeatResult.Perfect))
        {
            ExcuteVibration.Instance.Perfect();
        }
        else if (beatResult.Equals(BeatResult.Fail))
        {
            ExcuteVibration.Instance.Fail();
        }
        else
        {
            ExcuteVibration.Instance.FastOrSlow();
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
    
    private void CompareFeverTime(BeatResult[] beatResults, int index)
    {
        if (feverTimeController.IsFeverTime)
        {
            if (beatResults[index] == BeatResult.Perfect)
            {
                IncreaseItem();
            }
        }
    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Rewind();
            if (!isTutorial)
            {
                feverTimeController.Reset();
            }
        }
    }
}