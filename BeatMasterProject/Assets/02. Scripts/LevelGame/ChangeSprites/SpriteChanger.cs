using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;
using UnityEngine.Serialization;

public class SpriteChanger : MonoBehaviour
{
    [SerializeField] private ChangingResources[] _changingResources;
    private BackgroundMover _backgroundMover;
    private CameraController _cameraController;
    private CharacterMovement _characterMovement;
    private SpriteRenderer _characterRenderer;
    private int _materialIndex;
    private int _spriteIndex;    
    
    // TODO
    // 추후에 적, 주변 환경 등 1가지 이상으로 여러가지가 달라진다면 (적, 타일, {이펙트도 여기..??} 등)
    // enum과 딕셔너리를 사용하는 방법으로 구현하는 것을 생각해보았는데
    // SerializeField로는 어려우니 Resource.Load를 사용해야 될 것 같다.
    // => ResourceManager 고려
    /*public enum ChangingSprites
    {
        
    }*/
    
    // 배경은 Quad로 만들어졌으니 따로 Material 변수로 작성하는 것은 맞는 것 같다.
    // => SerializedField 가능
    
    // TODO
    // NormalGame도 등록할 이벤트들을 폴더별로 나누어 다 불러오든지 enum으로 나누어 씬 별로 불러와서 등록하던지 해야할 듯 싶다.
    // -> 상의해보기
    // NormalGame에서 성공했는지 여부도 받아야 함
    // -> 지은님과 상의
    // Rewind시 다시 바꾸는 로직도 생각해봐야 함
    // 기능 명세서와 클래스 다이어그램, 지라까지 싹 갈아 엎는 거 상의
    
    
    private void Awake()
    {
        _characterMovement = FindObjectOfType<CharacterMovement>();
        _characterRenderer = _characterMovement.GetComponent<SpriteRenderer>();
        _backgroundMover = FindObjectOfType<BackgroundMover>();
        _cameraController = FindObjectOfType<CameraController>();
    }

    public void Init()
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

    private void ResetMaterialsOffset(Material[] materials)
    {
        // BackgroundMover에서 이동한 결과에 의해 원본이 훼손되는 것을 막기위함
        foreach (var material in materials)
        {
            material.mainTextureOffset = Vector2.zero;
        }
    }

    public void OnSpeedChanged()
    {
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
                Material changingMaterial = _changingResources[0].ChangingMaterials[_materialIndex];
                _backgroundMover.SetMaterial(changingMaterial);
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

    public void OnLongPressed()
    {
        // 플레이어 스프라이트 변경
        switch (SceneLoadManager.Instance.Scene)
        {
            case SceneLoadManager.SceneType.Level1:
                // 스프라이트 교체
                _spriteIndex++;
                _spriteIndex %= _changingResources[0].ChangingSprites.Length;
                Sprite changingSprite = _changingResources[0].ChangingSprites[_spriteIndex];
                SetSprite(changingSprite);
                break;
            case SceneLoadManager.SceneType.Level2:
                break;
            case SceneLoadManager.SceneType.Level3:
                break;
            case SceneLoadManager.SceneType.Level4:
                break;
        }
    }
    
    private void SetSprite(Sprite sprite)
    {
        _characterRenderer.sprite = sprite;
    }
}
