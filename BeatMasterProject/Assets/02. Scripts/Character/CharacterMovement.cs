using System.Collections;
using UnityEngine;
using DG.Tweening;
using SonicBloom.Koreo;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    private Game _game;
    private ResourcesChanger _resourcesChanger;
    private Rigidbody2D _rigidbody;
    private TouchInputManager _touchInputManager;
    [SerializeField] private Vector3 _characterPosition;
    [SerializeField] private float _checkPointBeatTime;
    private float _gravityScale;
    private float _startGravityAccel;
    private float _gravityAccel;
    private float _previousBeatTime = 0;
    private float _currentBeatTime = 0;
    private bool _isFailed;

    [Header("Move")] 
    [EventID] public string speedEventID;
    [EventID] public string checkpointID;
    [SerializeField] private float _moveSpeed;
    public float MoveSpeed
    {
        get => _moveSpeed;
        set => _moveSpeed = value;
    }
    
    [Header("Jump")]
    [SerializeField] private float _jumpGapRate = 0.25f;
    [SerializeField] private float _jumpHeight = 1.3f;
    [SerializeField] private float _graphWidth = 1f;
    private int _jumpTileCount = 2;
    private const int _maxJumpCount = 1;
    private int _jumpCount;
    private Vector2 _jumpStartPosition;
    private float _jumpEndY, _jumpMidY;
    private bool _canGroundCheck = true;
    private bool _canJump = true;
    public bool isJumping;
    public bool isLongNote = false; // a variable set as true when CheckLongStart() is called

    [Header("Ray")] 
    [SerializeField] private Transform _rayOriginPoint;
    [SerializeField] private float _minRayDistance = 0.5f;
    private float _maxRayDistance;
    private float _rayDistance;
    [SerializeField] private float _positionYOffset;
    private LayerMask _tileLayer;

    [Header("Rewind")]
    public Vector3 lastPosition;
    public float lastBeatTime;
    private float rotationSpeed = 1080f;
    private RewindTime _rewindTime;
    private GameUI _gameUI;

    [Header("Character Tag")]
    private const string UnTag = "Untagged";
    private const string PlayerTag = "Player";

    private bool _isAttack;
    [SerializeField] private float _attackBeatTime;

    private ObjectGenerator _objectGenerator;
    [SerializeField] private int _rewindIdx;
    private bool _isCheckCheckPoint = true;
    
    private void Start()
    {
        Init();
    }

    private void Update()
    {
        if (SoundManager.instance.musicPlayer.IsPlaying)
        {
            GetInput();
            Move();
        }

        if (_isFailed) //실패하고 다시 시작할때
        {
            //Debug.Log("실패");
            //transform.position = _characterPosition;
            _previousBeatTime = _currentBeatTime;
            _isFailed = false;
        }
        
        if (_game.curState.Equals(GameState.Rewind))
        {
            _isFailed = true;
        }
    }

    private void FixedUpdate()
    {
        // if (SoundManager.instance.musicPlayer.IsPlaying)
        // {
        //     
        // }
    }

    private void Init()
    {
        _objectGenerator = FindObjectOfType<ObjectGenerator>();
        _gameUI = FindObjectOfType<GameUI>();
        _rewindTime = FindObjectOfType<RewindTime>();
        _game = FindObjectOfType<Game>();
        _resourcesChanger = FindObjectOfType<ResourcesChanger>();
        _touchInputManager = FindObjectOfType<TouchInputManager>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _rigidbody.bodyType = RigidbodyType2D.Kinematic;
        _rigidbody.interpolation = RigidbodyInterpolation2D.Interpolate;
        _rigidbody.constraints = RigidbodyConstraints2D.FreezeRotation;
        _tileLayer = LayerMask.GetMask("Ground");

        _characterPosition = transform.position;
        _maxRayDistance = _minRayDistance + 0.2f;
        _rayDistance = Mathf.Lerp(_minRayDistance, _maxRayDistance, (MoveSpeed - 2f) / 2f);
        
        Koreographer.Instance.RegisterForEvents(speedEventID, ChangeMoveSpeed);
        Koreographer.Instance.RegisterForEventsWithTime(checkpointID, CheckPoint);
    }

    private void GetInput()
    {
        if (_touchInputManager.CheckRightTouch())
        {
            PlayerStatus.Instance.ChangeStatus(CharacterStatus.Attack);
            SoundManager.instance.PlaySFX("Attack");
            _isAttack = true;
            _attackBeatTime = lastBeatTime;
        }
        // isLongNote prevents jumping during checking long notes
        if (!isLongNote && _touchInputManager.CheckLeftTouch() && _canJump)
        {
            Jump();
        }
        else if (!isLongNote && Input.GetKeyDown(KeyCode.LeftArrow) && _canJump)
        {
            Jump();
        }
    }

    private void Jump()
    {
        SoundManager.instance.PlaySFX("Jump");
        PlayerStatus.Instance.ChangeStatus(CharacterStatus.Jump);

        _jumpMidY = _jumpHeight;
        _jumpEndY = transform.position.y;
        _jumpStartPosition = transform.position;
        _graphWidth = 1f;
        _canJump = ++_jumpCount < _maxJumpCount;
        isJumping = true;
        _canGroundCheck = false;

        Invoke("GroundCheckOn", 0.2f);
        //Debug.DrawRay(new Vector2(_jumpStartPosition.x, 100f), Vector2.down * 1000f, Color.yellow, 10f);
        for (int i = 2; i <= 5; i++)
        {
            RaycastHit2D jumpEndCheckHit = Physics2D.Raycast(new Vector2(_jumpStartPosition.x + i, 100f), Vector2.down, 1000, _tileLayer);

            //Debug.DrawRay(new Vector2(_jumpStartPosition.x + i, 100f), Vector2.down * 1000f, Color.blue, 10f);
            if (jumpEndCheckHit)
            {
                _jumpTileCount = i;

                /// <summary>
                /// 점프하는 타일 칸 수에 따라서 점프 높이를 변경
                /// 최소 칸 수인 2칸일 때 점프 높이 1.3
                /// 최대 칸 수인 5칸일 때 점프 높이 1.9
                /// 해당 높이 사이에서 점프 칸 수에 따라 선형보간을 통해 높이 설정
                /// </summary>
                _jumpMidY = Mathf.Lerp(1.3f, 1.9f, (_jumpTileCount - 2) / 3f);
                _jumpEndY = (jumpEndCheckHit.point.y + _positionYOffset) - _jumpStartPosition.y;
                
                _jumpMidY += _jumpEndY * _jumpGapRate;

                //if (_jumpEndY >= 0)
                //{
                //    _graphWidth = Mathf.Lerp(1f, 1f, _jumpEndY);
                //}
                //else
                //{
                //    _graphWidth = Mathf.Lerp(1f, 1f, -_jumpEndY);
                //}

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
        float beatTime = (float)Koreographer.Instance.GetMusicBeatTime();

        if (beatTime != 0f)
        {
            float deltaBeatTime = beatTime - lastBeatTime;
            float deltaPosition = deltaBeatTime * MoveSpeed;
            Vector3 newPosition = lastPosition + transform.right * deltaPosition;

            float x = newPosition.x;
            float y = newPosition.y;

            // 이동한 위치 저장
            lastPosition = newPosition;
            lastBeatTime = beatTime;

            RaycastHit2D positionCheckHit = Physics2D.Raycast(_rayOriginPoint.position, Vector2.down, _rayDistance, _tileLayer);

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
                    y = _rigidbody.position.y + Physics2D.gravity.y * _gravityScale * _gravityAccel;
                }
            }

            // 점프하고 나서 다시 땅에 다다랐는지 체크
            if (_canGroundCheck)
            {
                RaycastHit2D groundCheckHit = Physics2D.Raycast(_rayOriginPoint.position, Vector2.down, _rayDistance, _tileLayer);

                if (groundCheckHit)
                {
                    isJumping = false;
                    _canJump = true;
                    _jumpCount = 0;
                    _gravityAccel = _startGravityAccel;
                    if (PlayerStatus.Instance.playerStatus != CharacterStatus.FastIdle && !_isAttack)
                    {
                        PlayerStatus.Instance.ChangeStatus(CharacterStatus.Run);
                    }
                }
            }

            if (_isAttack)
            {
                if (lastBeatTime >= _attackBeatTime + 0.7f)
                {
                    PlayerStatus.Instance.ChangeStatus(CharacterStatus.Run);
                    _isAttack = false;
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
                a = ((2f * _jumpEndY) - (4f * _jumpMidY)) / 9f;
                b = (_jumpEndY - (9f * a)) / 3f;
                break;
            case 4:
                a = (_jumpEndY - (2f * _jumpMidY)) / 8f;
                b = (_jumpMidY - (4f * a)) / 2f;
                break;
            case 5:
                a = ((2f * _jumpEndY) - (4f * _jumpMidY)) / 25f;
                b = (_jumpEndY - (25f * a)) / 5f;
                break;
            default: // jumpTileCount 2
                a = (_jumpEndY - (2f * _jumpMidY)) / 2f;
                b = _jumpMidY - a;
                break;
        }

        return (_graphWidth * a * x * x) + (b * x);
    }

    private void ChangeMoveSpeed(KoreographyEvent evt)
    {
        if (evt.HasFloatPayload())
        {
            _checkPointBeatTime = (float)Koreographer.Instance.GetMusicBeatTime();
            _rewindTime.ClearRewindList();
            MoveSpeed = evt.GetFloatValue();
            _rayDistance = Mathf.Lerp(_minRayDistance, _maxRayDistance, (MoveSpeed - 2f) / 2f);
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

    private void CheckPoint(KoreographyEvent evt, int sampleTime, int sampleDelta, DeltaSlice deltaSlice)
    {
        if (_isCheckCheckPoint && evt.GetValueOfCurveAtTime(sampleTime) < 0.9f)
        {
            _isCheckCheckPoint = false;
            _characterPosition = _objectGenerator.checkPointPos[_rewindIdx];
            _rewindIdx++;
        }
        
        if (evt.GetValueOfCurveAtTime(sampleTime) >= 1 && !_isCheckCheckPoint)
        {
            _isCheckCheckPoint = true;
        }
    }

    public void RewindPosition()
    {
        gameObject.tag = UnTag; // Set its tag as Untagged
        RaycastHit2D positionCheckHit = Physics2D.Raycast(_characterPosition, Vector2.down, 1000f, _tileLayer);
        float y = 0f;
        // 땅 위에 있을 때
        if (positionCheckHit)
        {
            y = positionCheckHit.point.y + _positionYOffset;
        }

        // _characterPosition = new Vector3(_characterPosition.x, y, 0f);
        // transform.position = _characterPosition;
        // _previousBeatTime = 0;
        // _currentBeatTime = _checkPointCurrentBeatTime;

        StartCoroutine(CoRewind(y));

        //_previousBeatTime = _checkPointCurrentBeatTime;
    }

    public IEnumerator CoRewind(float y)
    {
        float elapseTime = 0f;
        float targetTime = 0.1f;
        
        _rewindTime.StartRewind();
        
        if (_rewindTime.rewindList.Count != 0)
        {
            while (_rewindTime.rewindList.Count != 0)
            {
                elapseTime = 0f;
                Vector2 targetRewindPos = _rewindTime.rewindList[0].rewindPos;
                while (elapseTime <= targetTime)
                {
                    if (_rewindTime.rewindList.Count > 1)
                    {
                        transform.position = Vector3.Lerp(lastPosition, targetRewindPos, elapseTime / targetTime);
                        transform.Rotate(Vector3.forward * Time.fixedDeltaTime * rotationSpeed);
                        elapseTime += Time.fixedDeltaTime;
                        yield return null;
                    }
                    else
                    {
                        transform.position = Vector3.Lerp(lastPosition, _rewindTime.rewindList[0].rewindPos, elapseTime / targetTime);
                        transform.Rotate(Vector3.forward * Time.fixedDeltaTime * rotationSpeed);
                        elapseTime += Time.fixedDeltaTime;
                        yield return null;
                    }
                }
                if (Mathf.Abs(targetRewindPos.x - transform.position.x) <= 0.5f)
                {
                    _gameUI.ReverseTextColor(_rewindTime.rewindList[0].judgeResult);
                    lastPosition = targetRewindPos;
                    _rewindTime.rewindList.RemoveAt(0);
                }
            }
            elapseTime = 0f;
            targetTime = 0.3f;
            
            while (elapseTime <= targetTime)
            {
                transform.position = Vector3.Lerp(lastPosition, _characterPosition, elapseTime / targetTime);
                transform.DORotate(new Vector3(0, 0, 0), targetTime);
                elapseTime += Time.fixedDeltaTime;
                yield return null;
            }
        }
        else
        {
            while (elapseTime <= targetTime)
            {
                transform.position = Vector3.Lerp(lastPosition, _characterPosition, elapseTime / targetTime);
                transform.DORotate(new Vector3(0, 0, 0), targetTime);
                elapseTime += Time.fixedDeltaTime;
                yield return null;
            }
        }

        _rewindTime.StopRewind();
        
        _characterPosition = new Vector3(_characterPosition.x, y, 0f);
        transform.rotation = Quaternion.identity;
        transform.position = _characterPosition;
        lastPosition = _characterPosition;
        lastBeatTime = _checkPointBeatTime;

        _attackBeatTime = _checkPointBeatTime;
        _rewindIdx--;
        gameObject.tag = PlayerTag; // Back to Player Tag
    }
}