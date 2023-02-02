using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    private Game _game;
    private Rigidbody2D _rigidbody;

    [Header("Music")]
    [EventID] public string speedEventID;
    public float moveSpeed;
    public float gravityScale;
    public float startGravityAccel;
    private float _gravityAccel;
    private float _previousBeatTime = 0;

    [Header("Jump")]
    [SerializeField] private float _jumpHeight = 3f;
    [SerializeField] private int _jumpTileCount = 2;
    [SerializeField] private float _jumpGapRate = 0.25f;
    private const int _maxJumpCount = 2;
    private int _jumpCount;
    private Vector2 _jumpStartPosition;
    private float _jumpEndY;
    private float _jumpMidY;
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
        //_game = FindObjectOfType<Game>();
        _rigidbody = GetComponent<Rigidbody2D>();

        Koreographer.Instance.RegisterForEvents(speedEventID, ChangeMoveSpeed);
        //SoundManager.instance.PlayBGM(false);
    }

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        //if (_game.curState == GameState.Play)
        //{
            Move();
        //}
    }

    private void GetInput()
    {
        // 점프 입력
        if (Input.GetKeyDown(KeyCode.LeftArrow) && _canJump)
        {
            if (++_jumpCount >= _maxJumpCount)
            {
                _canJump = false;
            }

            _jumpMidY = _jumpHeight;
            _jumpStartPosition = transform.position;
            _isJumping = true;
            _canGroundCheck = false;

            Invoke("GroundCheckOn", 0.2f);

            RaycastHit2D jumpEndCheckHit = Physics2D.Raycast(new Vector2(_jumpStartPosition.x + _jumpTileCount, 100f), Vector2.down, 1000, _tileLayer);
            
            if (jumpEndCheckHit)
            {
                float yGap = jumpEndCheckHit.point.y - _jumpStartPosition.y;

                _jumpMidY += yGap * _jumpGapRate;
            }
        }
    }

    private void GroundCheckOn()
    {
        _canGroundCheck = true;
    }

    // 캐릭터의 움직임을 결정하는 메소드
    // 캐릭터의 x값은 노래에 맞추어 결정되고, y값은 캐릭터의 행동이나 조건에 따라 결정
    private void Move()
    {
        float currentBeatTime = (float)Koreographer.Instance.GetMusicBeatTime();
        float x = transform.position.x + (currentBeatTime - _previousBeatTime) * moveSpeed;
        //x = transform.position.x + (float)(Koreographer.Instance.GetMusicBPM() / 60 * moveSpeed * Time.fixedDeltaTime) ;
        float y = 0f;
        _previousBeatTime = currentBeatTime;

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

    // 점프 시 캐릭터의 y값을 계산하는 메소드
    // 이차함수 포물선을 따름(y = ax^2 + bx)
    // jumpTileCount로 x로 몇 칸만큼을 점프할지 지정
    private float GetJumpingY(float x, int jumpTileCount)
    {
        float a;
        float b;

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
            // default = jumpTileCount 1
            default:
                a = 2 * _jumpEndY - 4 * _jumpMidY;
                b = _jumpEndY - a;
                break;
        }

        return (a * x * x) + (b * x);
    }

    private void ChangeMoveSpeed(KoreographyEvent evt)
    {
        moveSpeed = evt.GetFloatValue();
    }
}
