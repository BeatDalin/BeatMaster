using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FallingCorgiController : ObjectPooling
{
    [SerializeField] private Sprite[] _corgiSprites;
    [SerializeField] private float _minPosYAmount = 1.1f;
    [SerializeField] private float _maxPosYAmount = 1.3f;
    [SerializeField] private float _minPosXAmount = 0.5f;
    [SerializeField] private float _maxPosXAmount = 1.1f;
    [SerializeField] private float _minRotation = 30f;
    [SerializeField] private float _maxRotation = 60f;
    [SerializeField] private float _minDealy = 0.5f;
    [SerializeField] private float _maxDelay = 1.5f;
    [SerializeField] private int _minLayer = -9;
    [SerializeField] private int _maxLayer = -5;

    private Camera _defaultCamera;
    private Coroutine _currentCoroutine;
    private List<GameObject> _currentList = new List<GameObject>();

    protected override void Init()
    {
        _defaultCamera = GameObject.FindWithTag("Main").GetComponent<Camera>();

        for (int i = 0; i < initCount; i++)
        {
            CreateNewObject().GetComponent<FallingCorgi>().SetPoolComponent(ReturnObject);
        }
    }

    public void InstantiateFallingCorgi()
    {
        _currentCoroutine = StartCoroutine(CoInstantiateCorgi());
    }

    public void StopFallingCorgi()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }

        if (_currentList.Count > 0)
        {
            DestroyObjects();
        }
    }

    public int GetSpritesLength()
    {
        return _corgiSprites.Length;
    }

    private IEnumerator CoInstantiateCorgi()
    {
        float posX = Random.Range(_minPosXAmount, _maxPosXAmount);
        float posY = Random.Range(_minPosYAmount, _maxPosYAmount);

        Vector3 afterPos = _defaultCamera.ViewportToWorldPoint(new Vector3(posX, posY, 0f));
        afterPos.z = 0f;
        float randomRotation = Random.Range(_minRotation, _maxRotation);
        
        GameObject go = GetObject(afterPos);
        go.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);
        _currentList.Add(go);
        
        SpriteRenderer[] spriteRenderer = go.GetComponentsInChildren<SpriteRenderer>();
        int layer = Random.Range(_minLayer, _maxLayer);
        foreach (var renderer in spriteRenderer)
        {
            renderer.sortingOrder = layer;
        }
        
        // 0번 부스터
        // 1번 코기
        int index = Random.Range(0, _corgiSprites.Length);
        spriteRenderer[1].sprite = _corgiSprites[index];
        
        float delay = Random.Range(_minDealy, _maxDelay);

        yield return new WaitForSeconds(delay);

        _currentCoroutine = StartCoroutine(CoInstantiateCorgi());
    }

    private void DestroyObjects()
    {
        foreach (var go in _currentList)
        {
            if (go.activeSelf)
            {
                ReturnObject(go);
            }
        }
        
        _currentList.Clear();
    }
}
