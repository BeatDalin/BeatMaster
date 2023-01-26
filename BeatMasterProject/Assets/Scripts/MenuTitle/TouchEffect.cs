using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchEffect : MonoBehaviour
{
    private Vector2 _direction;
    private SpriteRenderer _sprite;

    public float moveSpeed = 0.1f;

    public float minSize = 0.1f;
    public float maxSize = 0.3f;

    public float sizeSpeed = 1f;
    public float colorSpeed = 5f;

    public Color[] colors;
    // Start is called before the first frame update
    void Start()
    {
        _sprite = GetComponent<SpriteRenderer>();
        _direction = new Vector2(Random.Range(-0.1f, 0.1f), Random.Range(-0.1f, 0.1f));
        float size = Random.Range(minSize, maxSize);

        transform.localScale = new Vector2(size, size);

        _sprite.color = colors[Random.Range(0, colors.Length)];
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(_direction * moveSpeed);

        transform.localScale = Vector2.Lerp(transform.localScale, Vector2.zero, Time.deltaTime * sizeSpeed);

        Color color = _sprite.color;

        color.a = Mathf.Lerp(_sprite.color.a, 0, Time.deltaTime * colorSpeed);

        _sprite.color = color;

        if (_sprite.color.a < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}
