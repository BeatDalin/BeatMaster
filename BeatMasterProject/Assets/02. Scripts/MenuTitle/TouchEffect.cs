using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class TouchEffect : MonoBehaviour
{
    private Vector2 _direction;
    private SpriteRenderer _sprite;

    [SerializeField] private float _moveSpeed = 0.1f;
    [SerializeField] private float _minSize = 0.1f;
    [SerializeField] private float _maxSize = 0.3f;
    [SerializeField] private float _sizeSpeed = 1f;
    [SerializeField] private float _colorSpeed = 5f;

    [SerializeField] private Color[] _colors;

    [SerializeField] private float othgraphicSize = 3f;

    // Start is called before the first frame update
    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();

        InitEffect();
    }

    // Update is called once per frame
    void Update()
    {
        ChangeEffectScale();
        
        transform.Translate(_direction * _moveSpeed);

        transform.localScale = Vector2.Lerp(transform.localScale, Vector2.zero, Time.deltaTime * _sizeSpeed);

        Color color = _sprite.color;

        color.a = Mathf.Lerp(_sprite.color.a, 0, Time.deltaTime * _colorSpeed);

        _sprite.color = color;

        if (_sprite.color.a <= 0.1f)
        {
            InitEffect();

            ObjectPooling.Instance.ReturnObject(gameObject);
        }
    }

    private void ChangeEffectScale()
    {
        if (othgraphicSize != Camera.main.orthographicSize)
        {
            othgraphicSize = Camera.main.orthographicSize;
            float temp = 3 / othgraphicSize;
            _minSize = 1 - temp;
            _maxSize = _minSize + 0.3f;

            _minSize /= 2f;
            _maxSize /= 2f;
        }
        
    }

    private void InitEffect()
    {
        _direction = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));

        float size = Random.Range(_minSize, _maxSize);

        transform.localScale = new Vector2(size, size);

        _sprite.color = _colors[Random.Range(0, _colors.Length)];
    }
}