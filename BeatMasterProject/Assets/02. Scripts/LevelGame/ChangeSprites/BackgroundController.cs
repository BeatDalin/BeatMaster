using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class BackgroundController : MonoBehaviour
{
    [SerializeField] private float _moveXAmount = 0.01f;
    [SerializeField] private float _modSpeedY = 5f;
    // Stage1_Level1을 기준으로 정함
    [SerializeField] private Vector3 _defaultScale = new Vector3(5.5f, 5.5f, 1);
    [SerializeField] private float _defaultSpeed = 2f;
    
    private SpriteRenderer[] _mySpriteRenderers;
    private CameraController _cameraController;
    private CharacterMovement _characterMovement;
    private CinemachineVirtualCamera _characterCamera;
    private CinemachineBrain _mainCamBrain;
    private float _heightTimer;
    private float _prevSpeed;
    private static readonly int ScrollAmount = Shader.PropertyToID("_ScrollAmount");

    // TODO
    // 추후 해상도에 따라 스케일 조정 등을 생각 해봐야할 듯
    
    private void Awake()
    {
        Init();
    }

    void Init()
    {
        _cameraController = FindObjectOfType<CameraController>();
        _characterMovement = FindObjectOfType<CharacterMovement>();
        _mainCamBrain = GameObject.FindWithTag("Main").GetComponent<CinemachineBrain>();
        _characterCamera = _cameraController.GetComponent<CinemachineVirtualCamera>();
        _mySpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void SetOffsetSize(Sprite backgroundSprite)
    {
        Vector2 defaultSize = new Vector2(backgroundSprite.texture.width, backgroundSprite.texture.height);
        Vector2 totalSize = new Vector2(defaultSize.x * _defaultScale.x, defaultSize.y * _defaultScale.y);
        
        int currentWidth = _mySpriteRenderers[0].sprite.texture.width;
        int currenthHeight = _mySpriteRenderers[0].sprite.texture.height;
        transform.localScale = new Vector2(totalSize.x / currentWidth, totalSize.y / currenthHeight) *
                               (_characterMovement.MoveSpeed / _defaultSpeed);
    }

    private void Start()
    {
        StartCoroutine(CoMoveBackgroundOffset());
    }

    private void LateUpdate()
    {
        MoveBackground();
    }

    public void SetBackgroundSize()
    {
        // lerp 만들기z
        StartCoroutine(CoSetBackgroundSize());
    }

    private IEnumerator CoSetBackgroundSize()
    {
        bool isIncreasing = _cameraController.ToOrthoSize > _cameraController.FromOrthoSize;
        float standardSize = isIncreasing
            ? _cameraController.ToOrthoSize - _cameraController.FromOrthoSize
            : _cameraController.FromOrthoSize - _cameraController.ToOrthoSize;
        
        float ratio = 0;
        float modRatio = _cameraController.ToOrthoSize / _cameraController.FromOrthoSize;
        Vector3 originalScale = transform.localScale;
        
        while (ratio < 1)
        {
            yield return null;
            // 4 -> 6
            // 6   4.1
            // 1.9
            // 0.1 목표
            // 6 -> 4
            // 6  5.9
            // 0.1
            // 0.1 목표
            // 3.9
            float deltaSize = _cameraController.ToOrthoSize - _characterCamera.m_Lens.OrthographicSize;
            float totalIncrement = isIncreasing ? standardSize - deltaSize : standardSize + deltaSize;
            ratio = totalIncrement / standardSize;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * modRatio, ratio);
        }
    }

    private IEnumerator CoMoveBackgroundOffset()
    {
        while (!SoundManager.instance.musicPlayer.IsPlaying)
        {
            yield return null;
        }

        for (int i = 1; i < _mySpriteRenderers.Length; i++)
        {
            float scrollX = _mySpriteRenderers[i].material.GetFloat(ScrollAmount);
            scrollX += i * _moveXAmount * Time.deltaTime * _characterMovement.MoveSpeed;
            _mySpriteRenderers[i].material.SetFloat(ScrollAmount, scrollX);
        }

        yield return null;
        StartCoroutine(CoMoveBackgroundOffset());
    }

    private void MoveBackground()
    {
        Vector3 tempVec = transform.position;
        
        float posY = Mathf.Lerp(tempVec.y, _mainCamBrain.transform.position.y, _modSpeedY * Time.deltaTime);
        Vector3 goalVec = new Vector3(_characterMovement.transform.position.x, posY, tempVec.z);
        
        transform.position = goalVec;
    }

    public void SetBackgroundSprite(ChangingResources changingResources)
    {
        int length = changingResources.Backgrounds.Length;
        if (length == 0)
        {
            Debug.LogError("Backgrounds 리소스 길이가 0입니다");
            return;
        }

        // 0번째 인덱스는 안 움직이는 배경이 들어갈 것임 ==> Sprite-Default Material로 함
        _mySpriteRenderers[0].sprite = changingResources.Backgrounds[0];

        if (length == 1)
        {
            _mySpriteRenderers[0].material = changingResources.BackgroundMaterials[0];
        }
        else
        {
            for (int i = 1; i < changingResources.Backgrounds.Length; i++)
            {
                _mySpriteRenderers[i].sprite = changingResources.Backgrounds[i];
                _mySpriteRenderers[i - 1].material = changingResources.BackgroundMaterials[i - 1];
            }

            // 바뀔 배경보다 내부 스프라이트가 더 많을 때
            for (int i = changingResources.Backgrounds.Length; i < _mySpriteRenderers.Length; i++)
            {
                _mySpriteRenderers[i].sprite = null;
            }
        }
    }


    
}
