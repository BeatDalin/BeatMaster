using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SonicBloom.Koreo;

[RequireComponent(typeof(Rigidbody2D))]
public class CharacterMovement : MonoBehaviour
{
    private Rigidbody2D _rigidbody;

    [Header("Music")]
    public string musicName;
    public float beatTimeScale;
    public float gravityScale;
    public float startGravityAccel;
    private float _gravityAccel;

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
        _rigidbody = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        GetInput();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void GetInput()
    {
        // ���� �Է�
        if (Input.GetButtonDown("Jump") && _canJump)
        {
            if (++_jumpCount >= _maxJumpCount)
            {
                _canJump = false;
            }

            _jumpMidY = _jumpHeight;
            _jumpStartPosition = transform.position;
            _isJumping = true;
            _canGroundCheck = false;

            Invoke("GroundCheckOn", 0.1f);

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

    // ĳ������ �������� �����ϴ� �޼ҵ�
    // ĳ������ x���� �뷡�� ��Ʈ�� ���߾� �ڵ����� �����ǰ�, y���� ĳ������ �ൿ�̳� ���ǿ� ���� ����
    private void Move()
    {
        float x = Koreographer.GetBeatTime(musicName, 1) * beatTimeScale;
        float y = 0f;

        // ���� ���� �ƴ� �� ĳ������ y�� ����
        RaycastHit2D positionCheckHit = Physics2D.Raycast(_rayOriginPoint.position, Vector2.down, -_rayOriginPoint.localPosition.y + _rayDistanceOffset, _tileLayer);

        // �� ���� ���� ��
        if (positionCheckHit)
        {
            y = positionCheckHit.point.y + _positionOffsetY;
        }
        else
        {
            // ���� ���� �ƴϰ� �� �ؿ� �ƹ��͵� ���� ��
            if (!_isJumping)
            {
                _canJump = false;
                _gravityAccel += Time.fixedDeltaTime;
                y = _rigidbody.position.y + Physics2D.gravity.y * gravityScale * _gravityAccel;
            }
        }

        // �����ϰ� ���� �ٽ� ���� �ٴٶ����� üũ
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

        // ���� ���� �� ĳ���� y�� ����
        if (_isJumping)
        {
            y = GetJumpingY(_rigidbody.position.x - _jumpStartPosition.x, _jumpTileCount) + _jumpStartPosition.y;
        }

        // ���������� ���� x, y�� ĳ���� �̵�
        _rigidbody.MovePosition(new Vector2(x, y));
    }

    // ���� �� ĳ������ y���� ����ϴ� �޼ҵ�
    // �����Լ� �������� ����(y = ax^2 + bx)
    // jumpTileCount�� x�� �� ĭ��ŭ�� �������� ����
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
}
