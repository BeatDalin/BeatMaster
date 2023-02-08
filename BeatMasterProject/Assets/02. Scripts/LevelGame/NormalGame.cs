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
    private Anim _animScript;
    [Header("Event Check")]
    private List<KoreographyEvent> _events;
    private int[,] _eventRangeShort;
    private int[,] _eventRangeLong;
    private int _pressedTime;
    private int _pressedTimeLong;
    private bool _isCheckedShort; // to prevent double check
    private bool _isCheckedLong; // to prevent double check
    [Header("Input KeyCode")]
    private KeyCode _jumpNoteKey = KeyCode.LeftArrow;
    private KeyCode _attackNoteKey = KeyCode.RightArrow;
    private KeyCode _longNoteKey = KeyCode.LeftArrow;
    [Header("MonsterPool")]
    private MonsterPooling _monsterPooling;
    private CharacterMovement _characterMovement;
    [Header("SpriteChanger")]
    private PlayerStatus _playerStatus;

    private Anim _anim;
    private PlayerData _playerDatas;

    public bool IsLongPressed
    {
        get => isLongPressed;
        private set
        {
            if (isLongPressed != value)
            {
                isLongPressed = value;
                // TODO
                // _playerStatus.OnLongPressed(isLongPressed);
            }
        }
    }


    protected override void Awake()
    {
        base.Awake();
        _particleController = FindObjectOfType<ParticleController>();
        _monsterPooling = FindObjectOfType<MonsterPooling>();
        _characterMovement = FindObjectOfType<CharacterMovement>();
        _playerStatus = FindObjectOfType<PlayerStatus>();
        _animScript = FindObjectOfType<Anim>();
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

        _playerDatas = DataCenter.Instance.GetPlayerData();
        _anim = FindObjectOfType<Anim>();
        _anim.ChangeCharacterAnim(_playerDatas.playerChar);
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
        _events = SoundManager.instance.playingKoreo.GetTrackByID("Level1_JumpCheck").GetAllEvents();
        _eventRangeShort = CalculateRange(_events);
        _events = SoundManager.instance.playingKoreo.GetTrackByID("LongJumpCheckEnd").GetAllEvents();
        _eventRangeLong = CalculateRange(_events);
        // Save Point Initialize
        checkPointVisited = new bool[checkPointList.Count];
    }

    private void CheckShortEnd(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (_isCheckedShort && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            _isCheckedShort = false; // initialize before a curve value becomes 1
        }

        if (!isShortKeyCorrect)
        {
            if (evt.GetIntValue() == 0 && Input.GetKeyDown(_jumpNoteKey))
            {
                PlayerStatus.Instance.ChangeStatus(CharacterStatus.Attack);
                _particleController.PlayJumpParticle();
                isShortKeyCorrect = true;
                IncreaseItem();
                gameUI.UpdateText(TextType.Item, coinCount);
                _pressedTime = sampleTime; // record the sample time when the button was pressed
            }
            else if (evt.GetIntValue() == 1 && Input.GetKeyDown(_attackNoteKey))
            {
                _particleController.PlayJumpParticle();
                isShortKeyCorrect = true;
                IncreaseItem();
                gameUI.UpdateText(TextType.Item, coinCount);
                _pressedTime = sampleTime; // record the sample time when the button was pressed
                // 몬스터 삭제
            }
        }

        // The end of checking event range
        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1 && !_isCheckedShort)
        {
            _isCheckedShort = true;
            Debug.Log($"shortIdx: {shortIdx}");
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
        if (_isCheckedLong && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            _isCheckedLong = false; // initialize before a curve value becomes 1
            isLongFailed = false;
        }

        if (Input.GetKeyDown(_longNoteKey))
        {
            IsLongPressed = true;
            Debug.Log("Long Key Press");
            _animScript.SetEffectBool(true);
        }
        else if (Input.GetKeyUp(_longNoteKey))
        {
            IsLongPressed = false;
            Debug.Log("Long Key Up during CheckLongStart");
            _animScript.SetEffectBool(false);
        }

        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1f && !_isCheckedLong)
        {
            _isCheckedLong = true;
            if (!IsLongPressed) // Failed to press at the start of the long note
            {
                _animScript.SetEffectBool(false);
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
                _animScript.SetEffectBool(false);
                // Rewind( ); // for testing purpose... death 카운트 3번 올라가는 거 방지하려고}
                isLongFailed = true; // for testing purpose... death 카운트 3번 올라가는 거 방지하려고
            }
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
                PlayerStatus.Instance.ChangeStatus(CharacterStatus.Attack);
                isLongKeyCorrect = true;
                IncreaseItem();
                gameUI.UpdateText(TextType.Item, coinCount);

                _pressedTimeLong = sampleTime;
                _animScript.SetEffectBool(false);
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
        PlayerStatus.Instance.ChangeStatus(CharacterStatus.Damage);
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
        if (coinCount < 0)
        {
            coinCount = 0;
        }
    }

    private int check = 0;
    private void SaveCheckPoint(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        check++;
        Debug.Log($"SaveCheckPoint {check}");

        if (sampleTime > rewindSampleTime)
        {
            // DisableMonster Clear
            // sampleTime = 0 이면 첫시작이므로 ResetPool 안해도됨
            if (evt.StartSample != 0)
            {
                _monsterPooling.ResetPool();
            }
            // Entered new check point
            checkPointIdx++;
            // Record sample time to play music
            rewindSampleTime = checkPointList[checkPointIdx].StartSample;
            Debug.Log(rewindSampleTime);
            Debug.Log($"{checkPointIdx}");
            checkPointVisited[checkPointIdx] = true;
            // Play Particle or Animation
            // ex) particleSystem.Play();
            objectGenerator.PlayCheckAnim(checkPointIdx);
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
