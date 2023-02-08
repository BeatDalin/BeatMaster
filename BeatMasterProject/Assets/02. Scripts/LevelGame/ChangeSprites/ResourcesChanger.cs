using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;
using UnityEngine.Serialization;

public class ResourcesChanger : MonoBehaviour
{
    [SerializeField] private ChangingResources[] _changingResources;
    private BackgroundMover _backgroundMover;
    private CameraController _cameraController;
    private Material _backgroundMaterial;
    private int _materialIndex;
    private bool _isSpeedUp;    
    
    private void Awake()
    {
        _backgroundMover = FindObjectOfType<BackgroundMover>();
        _cameraController = FindObjectOfType<CameraController>();
        _backgroundMaterial = _backgroundMover.GetComponent<Renderer>().material;
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
                ResetTexturesOffset(_changingResources[0].ChangingMaterials);
                break;
            case SceneLoadManager.SceneType.Level2:
                break;
            case SceneLoadManager.SceneType.Level3:
                break;
            case SceneLoadManager.SceneType.Level4:
                break;
        }
    }

    private void ResetTexturesOffset(Material[] materials)
    {
        // BackgroundMover에서 이동한 결과에 의해 원본이 훼손되는 것을 막기위함
        _backgroundMaterial.mainTextureOffset = Vector2.zero;
        foreach (var material in materials)
        {
            material.mainTextureOffset = Vector2.zero;
        }
    }

    public void OnSpeedChanged()
    {
        _isSpeedUp = !_isSpeedUp;
        ChangeBackgroundMaterial();
        SetBackgroundSize();
    }
    
    private void ChangeBackgroundMaterial()
    {
        switch (SceneLoadManager.Instance.Scene)
        {
            case SceneLoadManager.SceneType.Level1:
                // 스프라이트 교체
                _materialIndex++;
                _materialIndex %= _changingResources[0].ChangingMaterials.Length;
                Material changingMaterial = _isSpeedUp? _changingResources[0].ChangingMaterials[_materialIndex] : _backgroundMaterial;
                _backgroundMover.SetTextureOffset(changingMaterial);
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
}
