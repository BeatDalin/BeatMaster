using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RepeatBackGround : MonoBehaviour
{
    [SerializeField] [Range(1f, 20f)] private float _speed = 1f;
    [SerializeField] private float _posValue;
    private Vector2 _startPos;
    private float _newPos;
    
    void Start()
    {
        _startPos = transform.position;
    }
    void Update()
    {
        RepeatBG();
    }

    private void RepeatBG()
    {
        _newPos = Mathf.Repeat(Time.time * _speed, _posValue);
        transform.position = (_startPos + Vector2.right * _newPos);
    }
    
}
