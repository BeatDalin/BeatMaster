using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class FallingCorgiController : MonoBehaviour
{
    [SerializeField] private Sprite[] _corgiSprites;
    [SerializeField] private GameObject _corgiprefab;
    [SerializeField] private float _minPosAmount = 1.1f;
    [SerializeField] private float _maxPosAmount = 1.3f;
    [SerializeField] private float _minRotation = 30f;
    [SerializeField] private float _maxRotation = 60f;
    [SerializeField] private float _minDealy = 0.5f;
    [SerializeField] private float _maxDelay = 1.5f;
    [SerializeField] private int _minLayer = -9;
    [SerializeField] private int _maxLayer = -5;

    private Camera _defaultCamera;
    private Coroutine _currentCoroutine;

    private void Awake()
    {
        _defaultCamera = GameObject.FindWithTag("Main").GetComponent<Camera>();
    }

    public void InstantiateFallingCorgi()
    {
        StartCoroutine(CoInstantiateCorgi());
    }

    public void StopFallingCorgi()
    {
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }
    }

    private IEnumerator CoInstantiateCorgi()
    {
        float posX = Random.Range(_minPosAmount, _maxPosAmount);
        float posY = Random.Range(_minPosAmount, _maxPosAmount);

        Vector3 afterPos = _defaultCamera.ViewportToWorldPoint(new Vector3(posX, posY, 0f));
        afterPos.z = 0f;
        float randomRotation = Random.Range(_minRotation, _maxRotation);
        GameObject go = Instantiate(_corgiprefab, afterPos, Quaternion.Euler(0f, 0f, randomRotation));
        go.transform.SetParent(transform);
        
        SpriteRenderer[] spriteRenderer = go.GetComponentsInChildren<SpriteRenderer>();
        int layer = Random.Range(_minLayer, _maxLayer);
        foreach (var renderer in spriteRenderer)
        {
            renderer.sortingOrder = layer;
        }
        
        // 0번 코기
        // 1번 부스터
        int index = Random.Range(0, _corgiSprites.Length);
        spriteRenderer[0].sprite = _corgiSprites[index];
        
        float delay = Random.Range(_minDealy, _maxDelay);

        yield return new WaitForSeconds(delay);

        _currentCoroutine = StartCoroutine(CoInstantiateCorgi());
    }
}
