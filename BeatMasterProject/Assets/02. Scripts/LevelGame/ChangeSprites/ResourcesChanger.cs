using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ResourcesChanger : MonoBehaviour
{
    private BackgroundMover _backgroundMover;
    private CameraController _cameraController;
    private Volume _volume;
    private ColorAdjustments _colorAdjustments;
    private int _materialIndex;
    private float _defaultSpeed;
    
    private void Awake()
    {
        _backgroundMover = FindObjectOfType<BackgroundMover>();
        _cameraController = FindObjectOfType<CameraController>();
        _volume = FindObjectOfType<Volume>();
        _volume.profile.TryGet(typeof(ColorAdjustments), out _colorAdjustments);
        _volume.enabled = false;
        Init();
    }

    private void Init()
    {
        switch (SceneLoadManager.Instance.Scene)
        {
            case SceneLoadManager.SceneType.Level1:
                break;
            case SceneLoadManager.SceneType.Level2:
                break;
            case SceneLoadManager.SceneType.Level3:
                break;
            case SceneLoadManager.SceneType.Level4:
                break;
        }
    }

    private void OnDestroy()
    {
        switch (SceneLoadManager.Instance.Scene)
        {
            case SceneLoadManager.SceneType.Level1:
                break;
            case SceneLoadManager.SceneType.Level2:
                break;
            case SceneLoadManager.SceneType.Level3:
                break;
            case SceneLoadManager.SceneType.Level4:
                break;
        }
    }
    
    

    public void OnSpeedChanged(float speed)
    {
        ChangePostProcessing(speed);
        SetBackgroundSize();
    }
    
    // 색
    // -180부터 135까지 45단위로 이동
    // 빨강:-180 노랑:-135 초록:-90 청록:-45 하늘:0 파랑:45 보라:90 핑크:135   
    private void ChangePostProcessing(float speed)
    {
        if (_defaultSpeed.Equals(speed))
        {
            _volume.enabled = false;
            return;
        }

        _volume.enabled = true;
        switch (SceneLoadManager.Instance.Scene)
        {
            case SceneLoadManager.SceneType.Level1:
                _colorAdjustments.hueShift.value = 60;
                // 스프라이트 교체
                break;
            case SceneLoadManager.SceneType.Level2:
                break;
            case SceneLoadManager.SceneType.Level3:
                break;
            case SceneLoadManager.SceneType.Level4:
                break;
        }
    }
    
    private void SetBackgroundSize()
    {
        Transform backgroundTrans = _backgroundMover.transform;
        backgroundTrans.localScale = backgroundTrans.localScale.x.Equals(_cameraController.MinOrthoSize)
            ? backgroundTrans.localScale * _cameraController.MinOrthoSize / _cameraController.MaxOrthoSize
            : backgroundTrans.localScale * _cameraController.MaxOrthoSize / _cameraController.MinOrthoSize;
    }

    public void SetDefaultSpeed(float speed)
    {
        _defaultSpeed = speed;
    }
}
