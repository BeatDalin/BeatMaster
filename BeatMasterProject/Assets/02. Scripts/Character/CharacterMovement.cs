using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;
using UnityEngine.Serialization;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    private Game _game;
    private Rigidbody2D _rigidbody;
    private ResourcesChanger _resourcesChanger;
    private Vector3 _characterPosition;

    [Header("Music")]
    [EventID] public string speedEventID;
    public float gravityScale;
    public float startGravityAccel;
    [SerializeField] private float _moveSpeed;
    public float MoveSpeed
    {
        get => _moveSpeed;
        set
        {
            if (_moveSpeed == 0)
            {
                _moveSpeed = value;
                return;
            }

            if (!_moveSpeed.Equals(value))
            {
                _moveSpeed = value;
                _resourcesChanger.OnSpeedChanged(_moveSpeed);
            }
        }
    }
    private float _gravityAccel;
    private float _previousBeatTime = 0;
    private float _currentBeatTime = 0;
    private float _checkPointCurrentBeatTime = 0f;

    [Header("Jump")]
    [SerializeField] private float _jumpHeight = 3f;
    [SerializeField] private int _jumpTileCount = 2;
    [SerializeField] private float _jumpGapRate = 0.25f;
    private const int _maxJumpCount = 1;
    private int _jumpCount;
    private Vector2 _jumpStartPosition;
    private float _jumpEndY, _jumpMidY;
    private bool _canGroundCheck = true;
    private bool _canJump = true;
    private bool _isJumping;

    [Header("Ray")]
    [SerializeField] private Transform _rayOriginPoint;
    [SerializeField] private LayerMask _tileLayer;
    [SerializeField] private float _rayDistanceOffset = 0.2f;
    [SerializeField] private float _positionOffsetY;

    private void Start()
    {
        _characterPosition = transform.position;
        _game = FindObjectOfType<Game>();
        _rigidbody = GetComponent<Rigidbody2D>();
        _resourcesChanger = FindObjectOfType<ResourcesChanger>();

        Koreographer.Instance.RegisterForEvents(speedEventID, ChangeMoveSpeed);
    }

    private void Update()
    {
        if (_game.curState.Equals(GameState.Play))
        {
            GetInput();
        }

        //Attack();

        if (_game.curState.Equals(GameState.Pause))
        {
            _currentBeatTime = _checkPointCurrentBeatTime;
            _previousBeatTime = _checkPointCurrentBeatTime;
        }
    }

    private void FixedUpdate()
    {
        if (_game.curState == GameState.Play || _game.curState == GameState.End)
        {
            Move();
        }
    }

    private void GetInput()
    {
        // 점프 입력
        if (Input.GetKeyDown(KeyCode.LeftArrow) && _canJump)
        {
            SoundManager.instance.PlaySFX("Jump");
            PlayerStatus.Instance.ChangeStatus(Status.Jump);

            if (++_jumpCount >= _maxJumpCount)
            {
                _canJump = false;
            }

            _jumpMidY = _jumpHeight;
            _jumpStartPosition = transform.position;
            _isJumping = true;
            _canGroundCheck = false;

            Invoke("GroundCheckOn", 0.2f);

            for (int i = 2; i <= 5; i++)
            {
                RaycastHit2D jumpEndCheckHit = Physics2D.Raycast(new Vector2(_jumpStartPosition.x + i, 100f), Vector2.down, 1000, _tileLayer);

                if (jumpEndCheckHit)
                {
                    _jumpTileCount = i;
                    float yGap = jumpEndCheckHit.point.y - _jumpStartPosition.y;
                    _jumpMidY += yGap * _jumpGapRate;

                    break;
                }
            }
            //RaycastHit2D jumpEndCheckHit = Physics2D.Raycast(new Vector2(_jumpStartPosition.x + _jumpTileCount, 100f), Vector2.down, 1000, _tileLayer);

            //if (jumpEndCheckHit)
            //{
            //    float yGap = jumpEndCheckHit.point.y - _jumpStartPosition.y;

            //    _jumpMidY += yGap * _jumpGapRate;
            //}
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
        _currentBeatTime = (float)Koreographer.Instance.GetMusicBeatTime();
        float x = transform.position.x + (_currentBeatTime - _previousBeatTime) * MoveSpeed;
        float y = 0f;
        _previousBeatTime = _currentBeatTime;

        // 점프 중이 아닐 때 캐릭터의 y값 설정
        RaycastHit2D positionCheckHit = Physics2D.Raycast(_rayOriginPoint.position, Vector2.down, -_rayOriginPoint.localPosition.y + _rayDistanceOffset, _tileLayer);

        // 땅 위에 있을 때
        if (positionCheckHit)
        {
            y = positionCheckHit.point.y + _positionOffsetY;
        }
        else
        {
            // 점프 중이 아니고 발 밑에 아무것도 없을 때
            if (!_isJumping)
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
                _isJumping = false;
                _canJump = true;
                _jumpCount = 0;
                _gravityAccel = startGravityAccel;
                PlayerStatus.Instance.ChangeStatus(Status.Run);
            }
        }

        // 점프 중일 때 캐릭터 y값 설정
        if (_isJumping)
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
            case 2:
                a = (_jumpEndY - (2 * _jumpMidY)) / 2;
                b = _jumpMidY - a;
                break;
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
            default: // jumpTileCount 1
                a = 2 * _jumpEndY - 4 * _jumpMidY;
                b = _jumpEndY - a;
                break;
        }

        return (a * x * x) + (b * x);
    }

    private void ChangeMoveSpeed(KoreographyEvent evt)
    {
        if (evt.HasFloatPayload())
        {
            MoveSpeed = evt.GetFloatValue();
            _checkPointCurrentBeatTime = (float)Koreographer.Instance.GetMusicBeatTime();
            _characterPosition = transform.position;
        }
        if (evt.HasTextPayload())
        {
            if (evt.GetTextValue() == "End")
            {
                _game.curState = GameState.End;
            }
            else if (evt.GetTextValue() == "Stop")
            {
                _canGroundCheck = false;
                PlayerStatus.Instance.ChangeStatus(Status.Idle);
            }
        }
    }

    public void RewindPosition()
    {
        transform.position = _characterPosition;
    }
}
