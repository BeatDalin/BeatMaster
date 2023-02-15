using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    private Game _game;
    private ResourcesChanger _resourcesChanger;
    private Rigidbody2D _rigidbody;
    private Vector3 _characterPosition;

    private TouchInputManager _touchInputManager;
    
    [Header("Music")]
    [EventID] public string speedEventID;
    [SerializeField] private float _moveSpeed;

    private bool _isPaused;
    public float MoveSpeed
    {
        get => _moveSpeed;
        set
        {
            if (_moveSpeed == 0)
            {
                _moveSpeed = value;
                _resourcesChanger.SetDefaultSpeed(_moveSpeed);
                return;
            }

            if (!_moveSpeed.Equals(value))
            {
                _moveSpeed = value;
                _resourcesChanger.OnSpeedChanged(_moveSpeed);
            }
        }
    }
    public float gravityScale;
    public float startGravityAccel;
    private float _gravityAccel;
    private float _previousBeatTime = 0;
    private float _currentBeatTime = 0;
    private float _checkPointCurrentBeatTime = 0f;

    [Header("Jump")]
    [SerializeField] private float _jumpGapRate = 0.5f;
    private float _jumpHeight = 1.75f;
    private int _jumpTileCount = 2;
    private const int _maxJumpCount = 1;
    private int _jumpCount;
    private Vector2 _jumpStartPosition;
    private float _jumpEndY, _jumpMidY;
    private bool _canGroundCheck = true;
    public bool _canJump = true;
    public bool isJumping;

    [Header("Ray")]
    [SerializeField] private Transform _rayOriginPoint;
    [SerializeField] private float _rayDistanceOffset = 0.2f;
    [SerializeField] private float _positionYOffset;
    private LayerMask _tileLayer;

    private RewindTime _rewindTime;

    private void Awake()
    {
        _touchInputManager = FindObjectOfType<TouchInputManager>();
    }

    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (_game.curState.Equals(GameState.Play))
        {
            GetInput();
        }
        else
        {
            _isPaused = true;
        }

        // if (_game.curState.Equals(GameState.Pause))
        // {
        //     _currentBeatTime = _checkPointCurrentBeatTime;
        //     _previousBeatTime = _checkPointCurrentBeatTime;
        // }
    }

    private void FixedUpdate()
    {
        if (_game.curState == GameState.Play)
        {
            Move();
        }
    }

    private void Init()
    {
        _rewindTime = FindObjectOfType<RewindTime>();
        _game = FindObjectOfType<Game>();
        _resourcesChanger = FindObjectOfType<ResourcesChanger>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody.interpolation = RigidbodyInterpolation2D.Extrapolate;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        _tileLayer = LayerMask.GetMask("Ground");
        _characterPosition = transform.position;
        MoveSpeed = 2f;

        Koreographer.Instance.RegisterForEvents(speedEventID, ChangeMoveSpeed);
    }

    private void GetInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow) && _canJump)
        {
            Jump();
        }

        if (_touchInputManager.isJumpTouch && _canJump)
        {
            Jump();
        }
        
    }

    private void Jump()
    {
        SoundManager.instance.PlaySFX("Jump");
        PlayerStatus.Instance.ChangeStatus(CharacterStatus.Jump);

        _jumpMidY = _jumpHeight;
        _jumpStartPosition = transform.position;
        _canJump = ++_jumpCount < _maxJumpCount;
        isJumping = true;
        _canGroundCheck = false;

        Invoke("GroundCheckOn", 0.2f);

        for (int i = 2; i <= 5; i++)
        {
            RaycastHit2D jumpEndCheckHit = Physics2D.Raycast(new Vector2(_jumpStartPosition.x + i, 100f), Vector2.down, 1000, _tileLayer);

            if (jumpEndCheckHit)
            {
                _jumpTileCount = i;

                switch (_jumpTileCount)
                {
                    case 3:
                        _jumpMidY = 2f;
                        break;
                    case 4:
                        _jumpMidY = 2.25f;
                        break;
                    case 5:
                        _jumpMidY = 2.5f;
                        break;
                    default: // _jumpTileCount 2
                        _jumpMidY = 1.75f;
                        break;
                }

                float yGap = jumpEndCheckHit.point.y - (_jumpStartPosition.y - _positionYOffset);
                _jumpMidY += yGap * _jumpGapRate;

                break;
            }
        }
    }

    private void GroundCheckOn()
    {
        _canGroundCheck = true;
    }

    /// <summary>
    /// 캐릭터의 움직임을 결정하는 메소드
    /// x는 노래에 맞추어 결정되고, y는 캐릭터의 행동이나 조건에 따라 결정
    /// </summary>
    private void Move()
    {
        float x = 0f;
        float y = 0f;
        
        if (!_isPaused)
        {
            _currentBeatTime = (float)Koreographer.Instance.GetMusicBeatTime();
            
            x = transform.position.x + (_currentBeatTime - _previousBeatTime) * MoveSpeed;
            _previousBeatTime = _currentBeatTime;
        }
        else
        {
            transform.position = _characterPosition;
            x = transform.position.x + (_currentBeatTime - _checkPointCurrentBeatTime) * MoveSpeed;
            _previousBeatTime = _currentBeatTime;
            _isPaused = false;
        }
        
        // 점프 중이 아닐 때 캐릭터의 y값 설정
        RaycastHit2D positionCheckHit = Physics2D.Raycast(_rayOriginPoint.position, Vector2.down, -_rayOriginPoint.localPosition.y + _rayDistanceOffset, _tileLayer);

        // 땅 위에 있을 때
        if (positionCheckHit)
        {
            y = positionCheckHit.point.y + _positionYOffset;
        }
        else
        {
            // 점프 중이 아니고 발 밑에 아무것도 없을 때
            if (!isJumping)
            {
                _canJump = false;
                _gravityAccel += Time.fixedDeltaTime;
                y = _rigidbody.position.y + Physics2D.gravity.y * gravityScale * _gravityAccel;
            }
        }

        // 점프하고 나서 다시 땅에 다다랐는지 체크
        if (_canGroundCheck)
        {
            RaycastHit2D groundCheckHit = Physics2D.Raycast(_rayOriginPoint.position, Vector2.down, -_rayOriginPoint.localPosition.y + _rayDistanceOffset, _tileLayer);

            if (groundCheckHit)
            {
                isJumping = false;
                _canJump = true;
                _jumpCount = 0;
                _gravityAccel = startGravityAccel;
                PlayerStatus.Instance.ChangeStatus(CharacterStatus.Run);
            }
        }

        // 점프 중일 때 캐릭터 y값 설정
        if (isJumping)
        {
            y = GetJumpingY(_rigidbody.position.x - _jumpStartPosition.x, _jumpTileCount) + _jumpStartPosition.y;
        }

        // 최종적으로 계산된 x, y로 캐릭터 이동
        _rigidbody.MovePosition(new Vector2(x, y));
    }

    /// <summary>
    /// 점프 시 캐릭터 Position의 y를 계산하는 메소드
    /// 이차함수 포물선을 따름(y = ax^2 + bx)
    /// jumpTileCount로 x로 몇 칸만큼을 점프할지 지정(최대 5칸)
    /// </summary>
    private float GetJumpingY(float x, int jumpTileCount)
    {
        float a, b;

        switch (jumpTileCount)
        {
            case 3:
                a = ((2 * _jumpEndY) - (4 * _jumpMidY)) / 9;
                b = (_jumpEndY - (9 * a)) / 3;
                break;
            case 4:
                a = (_jumpEndY - (2 * _jumpMidY)) / 8;
                b = (_jumpMidY - (4 * a)) / 2;
                break;
            case 5:
                a = ((2 * _jumpEndY) - (4 * _jumpMidY)) / 25;
                b = (_jumpEndY - (25 * a)) / 5;
                break;
            default: // jumpTileCount 2
                a = (_jumpEndY - (2 * _jumpMidY)) / 2;
                b = _jumpMidY - a;
                break;
        }

        return (a * x * x) + (b * x);
    }

    private void ChangeMoveSpeed(KoreographyEvent evt)
    {
        if (evt.HasFloatPayload())
        {
            _characterPosition = transform.position;
            MoveSpeed = evt.GetFloatValue();
            _checkPointCurrentBeatTime = (float)Koreographer.Instance.GetMusicBeatTime();
            //_rewindTime.RecordCheckPoint(_characterPosition);
            Debug.Log(_checkPointCurrentBeatTime);
        }
        if (evt.HasTextPayload())
        {
            if (evt.GetTextValue() == "Stop")
            {
                _canGroundCheck = false;
                PlayerStatus.Instance.ChangeStatus(CharacterStatus.Idle);
            }
        }
    }

    public void RewindPosition()
    {
        RaycastHit2D positionCheckHit = Physics2D.Raycast(_characterPosition, Vector2.down, 1000f, _tileLayer);
        float y = 0f;
        // 땅 위에 있을 때
        if (positionCheckHit)
        {
            y = positionCheckHit.point.y + _positionYOffset;
        }
        
        _characterPosition = new Vector3(_characterPosition.x, y, 0f);
        transform.position = _characterPosition;
        _previousBeatTime = 0;
        _currentBeatTime = _checkPointCurrentBeatTime;

        //StartCoroutine(Rewind(y));
        
        //_previousBeatTime = _checkPointCurrentBeatTime;
    }
    
    public IEnumerator Rewind(float y)
    {
        _rewindTime.isRecord = false;
        while (_rewindTime.rewindPos.Count != 0)
        {
            transform.position = Vector3.Lerp(_rewindTime.rewindPos[0], transform.position, Time.deltaTime);
            //transform.position = _rewindTime.rewindPos[0];
            _rewindTime.rewindPos.RemoveAt(0);
            yield return null;
        }
        _rewindTime.StopRewind();
        _characterPosition = new Vector3(_characterPosition.x, y, 0f);
        transform.position = _characterPosition;
        _previousBeatTime = 0;
        _currentBeatTime = _checkPointCurrentBeatTime;
    }

    private IEnumerator SetPos(Vector3 position)
    {
        while (transform.position != position)
        {
            yield return null;
        }
    }
}
