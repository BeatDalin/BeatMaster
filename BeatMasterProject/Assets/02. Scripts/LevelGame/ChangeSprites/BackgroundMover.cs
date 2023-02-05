using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class BackgroundMover : MonoBehaviour
{
    [SerializeField] private float moveXAmount = 0.01f;
    private Renderer _myRenderer;
    private Material _myMaterial;
    private Vector2 _currentUV;
    
    // TODO
    // 추후 해상도에 따라 스케일 조정 등을 생각 해봐야할 듯
    
    private void Awake()
    {
        _myRenderer = GetComponent<Renderer>();
        _myMaterial = _myRenderer.material;
        _currentUV = _myMaterial.mainTextureOffset;
    }

    private void Update()
    {
        MoveBackground();
    }

    private void MoveBackground()
    {
        // 캐릭터 속도와 맞출 필요성이 있어보임
        Vector2 tempUV = _currentUV;
        tempUV.x += moveXAmount * Time.deltaTime;
        _currentUV = tempUV;
        _myMaterial.mainTextureOffset = _currentUV;
    }

    public void SetMaterial(Material material)
    {
        _myRenderer.material = material;
        _myMaterial = material;
        // 자연스럽게 교체하기 위해 초기화
        _myMaterial.mainTextureOffset = _currentUV;
    }

    
}
