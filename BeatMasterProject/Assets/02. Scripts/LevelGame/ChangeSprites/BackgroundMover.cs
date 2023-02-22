using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class BackgroundMover : MonoBehaviour
{
    [SerializeField] private float _moveXAmount = 0.01f;
    [SerializeField] private float _modSpeedY = 5f;
    private SpriteRenderer[] _mySpriteRenderers;
    private CameraController _cameraController;
    private CharacterMovement _characterMovement;
    private CinemachineVirtualCamera _characterCamera;
    private CinemachineBrain _mainCamBrain;
    private float _heightTimer;
    private float _prevSpeed;
    private float _scrollX;
    private static readonly int ScrollAmount = Shader.PropertyToID("_ScrollAmount");

    // TODO
    // 추후 해상도에 따라 스케일 조정 등을 생각 해봐야할 듯
    
    private void Awake()
    {
        _cameraController = FindObjectOfType<CameraController>();
        _characterMovement = FindObjectOfType<CharacterMovement>();
        _mainCamBrain = GameObject.FindWithTag("Main").GetComponent<CinemachineBrain>();
        _characterCamera = _cameraController.GetComponent<CinemachineVirtualCamera>();
        
        _mySpriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    private void Update()
    {
        MoveBackgroundOffset();
    }

    private void LateUpdate()
    {
        MoveBackground();
    }
    
    public void SetBackgroundSize()
    {
        // lerp 만들기
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
            yield return new WaitForEndOfFrame();
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
            Debug.Log(originalScale * modRatio);
            transform.localScale = Vector3.Lerp(originalScale, originalScale * modRatio, ratio);
        }
    }

    private void MoveBackgroundOffset()
    {
        if (_characterMovement.transform.position.x == 0)
        {
            return;
        }

        for (int i = 0; i < _mySpriteRenderers.Length; i++)
        {
            _scrollX += _moveXAmount * Time.deltaTime * _characterMovement.MoveSpeed;
            _mySpriteRenderers[i].material.SetFloat(ScrollAmount, _scrollX);
        }

    }

    private void MoveBackground()
    {
        Vector3 tempVec = transform.position;
        
        float posY = Mathf.Lerp(tempVec.y, _mainCamBrain.transform.position.y, _modSpeedY * Time.deltaTime);
        Vector3 goalVec = new Vector3(_characterMovement.transform.position.x, posY, tempVec.z);
        
        transform.position = goalVec;
    }

}
