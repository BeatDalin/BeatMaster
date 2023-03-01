using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class FallingCorgi : MonoBehaviour
{
    [SerializeField] private float _minSpeed = 3f;
    [SerializeField] private float _maxSpeed = 5f;
    [SerializeField] private int _minLayer = -10;
    [SerializeField] private int _maxLayer = -5;

    private Camera _defaultCam;
    private float _speed;
    private Action<GameObject> _returnAction;
    private SpriteRenderer[] _spriteRenderers;

    private void Awake()
    {
        _defaultCam = GameObject.FindWithTag("Main").GetComponent<Camera>();
        _speed = Random.Range(_minSpeed, _maxSpeed);
    }

    public void SetPoolComponent(Action<GameObject> action)
    {
        _returnAction = action;
    }

    void Update()
    {
        Vector3 pos = _defaultCam.WorldToViewportPoint(transform.position);
        pos.z = 0f;
        if (pos.x < -1f || pos.y < -1f)
        {
            _returnAction?.Invoke(gameObject);
        }
        
        transform.Translate(transform.right * (-1f * (Time.deltaTime * _speed)));        
    }

    public void SetRendererComponent()
    {
        _spriteRenderers = new SpriteRenderer[GetComponentsInChildren<SpriteRenderer>().Length];
        _spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
    }

    public void SetRendererSprite(Sprite sprite)
    {
        int layer = Random.Range(_minLayer, _maxLayer);
        
        foreach (var spriteRenderer in _spriteRenderers)
        {
            spriteRenderer.sortingOrder = layer;
            Color color = spriteRenderer.color;
            color.a = 1f;
            spriteRenderer.color = color;
        }
        
        _spriteRenderers[1].sprite = sprite;
    }

    public void DecreaseAlpha()
    {
        StartCoroutine(CoDecreaseAlpha());
    }

    private IEnumerator CoDecreaseAlpha()
    {
        float ratio = 0f;
        while (ratio < 1f)
        {
            ratio += Time.deltaTime;
            foreach (var spriteRenderer in _spriteRenderers)
            {
                Color color = spriteRenderer.color;
                // 점점 사라져야 하므로 1-ratio
                color.a = 1f - ratio;
                spriteRenderer.color = color;
            }

            yield return null;
        }

        _returnAction.Invoke(gameObject);
    }
}
