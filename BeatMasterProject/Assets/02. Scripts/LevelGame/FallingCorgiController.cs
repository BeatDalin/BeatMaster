using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [SerializeField] private float _maxDelay = 0.8f;
    

    private Camera _defaultCamera;
    private Coroutine _currentCoroutine;
    private List<FallingCorgi> _preList = new List<FallingCorgi>();
    private List<FallingCorgi> _currentList = new List<FallingCorgi>();

    protected override void Init()
    {
        _defaultCamera = GameObject.FindWithTag("Main").GetComponent<Camera>();

        for (int i = 0; i < initCount; i++)
        {
            FallingCorgi fallingCorgi = CreateNewObject().GetComponentInChildren<FallingCorgi>();
            _currentList.Add(fallingCorgi);
            fallingCorgi.SetPoolComponent(ReturnObject);
            fallingCorgi.SetRendererComponent();
        }
    }

    public void InstantiateFallingCorgi()
    {
        _currentCoroutine = StartCoroutine(CoInstantiateCorgi());
    }

    public void StopFallingCorgi(bool isFeverEnd)
    {
        
        if (_currentCoroutine != null)
        {
            StopCoroutine(_currentCoroutine);
        }

        if (_currentList.Count > 0)
        {
            _preList = _currentList.ToList();
            _currentList.Clear();
            
            if (isFeverEnd)
            {
                DieAwayCorgi();
            }
            else
            {
                DestroyObjects();
            }
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
        
        FallingCorgi fallingCorgi = GetObject(afterPos).GetComponent<FallingCorgi>();
        fallingCorgi.transform.rotation = Quaternion.Euler(0f, 0f, randomRotation);
        _currentList.Add(fallingCorgi);
        
        // 0번 부스터
        // 1번 코기
        int index = Random.Range(0, _corgiSprites.Length);
        
        fallingCorgi.SetRendererSprite(_corgiSprites[index]);
        
        float delay = Random.Range(_minDealy, _maxDelay);

        yield return new WaitForSeconds(delay);

        _currentCoroutine = StartCoroutine(CoInstantiateCorgi());
    }
    
    private void DieAwayCorgi()
    {
        if (_preList.Count == 0)
        {
            return;
        }
        
        foreach (var fallingCorgi in _preList)
        {
            if (fallingCorgi.gameObject.activeSelf)
            {
                fallingCorgi.DecreaseAlpha();
            }
        }
        
        _preList.Clear();
    }

    private void DestroyObjects()
    {
        foreach (var fallingCorgi in _preList)
        {
            if (fallingCorgi.gameObject.activeSelf)
            {
                ReturnObject(fallingCorgi.gameObject);
            }
        }
        
        _preList.Clear();
    }
}
