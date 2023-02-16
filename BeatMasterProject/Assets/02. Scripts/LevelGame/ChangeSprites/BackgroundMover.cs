using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class BackgroundMover : MonoBehaviour
{
    [SerializeField] private float _moveXAmount = 0.01f;
    [SerializeField] private float _heightLerp = 0.05f;
    [SerializeField] private float _ModSpeedY = 5f;
    private Renderer _myRenderer;
    private Material _myMaterial;
    private CameraController _cameraController;
    private CharacterMovement _characterMovement;
    private CinemachineVirtualCamera _characterCamera;
    private CinemachineBrain _mainCamBrain;
    private Vector2 _currentUV;
    private float _heightTimer;
    private float _prevSpeed;
    
    // TODO
    // 추후 해상도에 따라 스케일 조정 등을 생각 해봐야할 듯
    
    private void Awake()
    {
        _cameraController = FindObjectOfType<CameraController>();
        _myRenderer = GetComponent<Renderer>();
        _characterMovement = FindObjectOfType<CharacterMovement>();
        _mainCamBrain = GameObject.FindWithTag("Main").GetComponent<CinemachineBrain>();
        _characterCamera = _cameraController.GetComponent<CinemachineVirtualCamera>();
        _myMaterial = _myRenderer.material;
        _currentUV = _myMaterial.mainTextureOffset;
    }

    private void Update()
    {
        MoveBackgroundOffset();
    }

    private void LateUpdate()
    {
        MoveBackground();
    }
    
    public void SetBackgroundSize(float speed)
    {
        // lerp 만들기
        StartCoroutine(CoSetBackgroundSize(speed));
        _prevSpeed = speed;
    }

    private IEnumerator CoSetBackgroundSize(float speed)
    {
        float goalSize = speed < _prevSpeed ? _cameraController.MinOrthoSize : _cameraController.MaxOrthoSize;
        float modRatio = speed < _prevSpeed
            ? goalSize / _cameraController.MaxOrthoSize
            : goalSize / _cameraController.MinOrthoSize;
        float standardSize = goalSize - _characterCamera.m_Lens.OrthographicSize;
        float ratio = 0;
        Vector3 originalScale = transform.localScale;
        
        while (ratio < 1)
        {
            yield return new WaitForEndOfFrame();
            float deltaSize = goalSize - _characterCamera.m_Lens.OrthographicSize;
            float totalIncrement = standardSize - deltaSize;
            
            ratio = totalIncrement / standardSize;
            transform.localScale = Vector3.Lerp(originalScale, originalScale * modRatio, ratio);
        }
    }
    
    private void MoveBackgroundOffset()
    {
        Vector2 tempUV = _currentUV;
        tempUV.x += _moveXAmount * Time.deltaTime * _characterMovement.MoveSpeed;
        _currentUV = tempUV;
        _myMaterial.mainTextureOffset = _currentUV;
    }

    private void MoveBackground()
    {
        Vector3 tempVec = transform.position;
        
        float posY = Mathf.Lerp(tempVec.y, _mainCamBrain.transform.position.y, _ModSpeedY * Time.deltaTime);
        Vector3 goalVec = new Vector3(_characterMovement.transform.position.x, posY, tempVec.z);
        
        transform.position = goalVec;
    }

}
