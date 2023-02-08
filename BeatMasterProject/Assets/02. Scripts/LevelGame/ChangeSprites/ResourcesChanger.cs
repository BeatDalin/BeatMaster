using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Serialization;

public class ResourcesChanger : MonoBehaviour
{
    [SerializeField] private ChangingResources[] _changingResources;
    private BackgroundMover _backgroundMover;
    private CameraController _cameraController;
    private Volume _volume;
    private int _volumeIndex;
    private float _defaultSpeed;
    
    private void Awake()
    {
        _backgroundMover = FindObjectOfType<BackgroundMover>();
        _cameraController = FindObjectOfType<CameraController>();
        _volume = FindObjectOfType<Volume>();
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
    
    private void ChangePostProcessing(float speed)
    {
        if (_defaultSpeed.Equals(speed))
        {
            _volume.profile = null;
            return;
        }
         
        switch (SceneLoadManager.Instance.Scene)
        {
            case SceneLoadManager.SceneType.Level1:
                // 포스트 프로세싱
                _volumeIndex %= _changingResources[0].ChangingProfiles.Length;
                _volume.profile = _changingResources[0].ChangingProfiles[_volumeIndex];
                _volumeIndex++;
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
