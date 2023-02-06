using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BackgroundMover : MonoBehaviour
{
    [SerializeField] private float _moveXAmount = 0.01f;
    private Renderer _myRenderer;
    private Material _myMaterial;
    private Vector2 _currentUV;
    private CharacterMovement _characterMovement;
    
    // TODO
    // 추후 해상도에 따라 스케일 조정 등을 생각 해봐야할 듯
    
    private void Awake()
    {
        _myRenderer = GetComponent<Renderer>();
        _characterMovement = FindObjectOfType<CharacterMovement>();
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
        tempVec.x = _characterMovement.transform.position.x;
        transform.position = tempVec;
    }

    public void SetMaterial(Material material)
    {
        _myRenderer.material = material;
        _myMaterial = material;
        // 자연스럽게 교체하기 위해 초기화
        _myMaterial.mainTextureOffset = _currentUV;
    }

    
    
}
