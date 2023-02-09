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
    private Renderer _backgroundRenderer;
    private Material _backgroundMaterial;
    private CameraController _cameraController;
    private int _materialIndex;
    private float _defaultSpeed;
    
    private void Awake()
    {
        _backgroundMover = FindObjectOfType<BackgroundMover>();
        _backgroundRenderer = _backgroundMover.GetComponent<Renderer>();
        _backgroundMaterial = _backgroundRenderer.material;
        _cameraController = FindObjectOfType<CameraController>();
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
                ResetMaterialsOffset(_changingResources[0].ChangingMaterials);
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
            _backgroundMover.SetMaterial(_backgroundMaterial);
            return;
        }
         
        switch (SceneLoadManager.Instance.Scene)
        {
            case SceneLoadManager.SceneType.Level1:
                // 스프라이트 교체
                _materialIndex %= _changingResources[0].ChangingMaterials.Length;
                Material changingMaterial = _changingResources[0].ChangingMaterials[_materialIndex];
                _backgroundMover.SetMaterial(changingMaterial);
                _materialIndex++;
                break;
            case SceneLoadManager.SceneType.Level2:
                break;
            case SceneLoadManager.SceneType.Level3:
                break;
            case SceneLoadManager.SceneType.Level4:
                break;
        }
    }
    
    private void ResetMaterialsOffset(Material[] materials)
    {
        // BackgroundMover에서 이동한 결과에 의해 원본이 훼손되는 것을 막기위함
        _backgroundMaterial.mainTextureOffset = Vector2.zero;
        foreach (var material in materials)
        {
            material.mainTextureOffset = Vector2.zero;
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
