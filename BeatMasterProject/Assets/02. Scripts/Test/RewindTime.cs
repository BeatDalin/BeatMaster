using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class RewindTime : MonoBehaviour
{
    public bool isRewind;
    public bool isRecord = true;

    public bool _rewindCheck;

    public List<Vector2> rewindPos;

    private float _time;
    private Game _game;

    private void Awake()
    {
        _game = FindObjectOfType<Game>();
    }

    private void FixedUpdate()
    {
        if (isRecord && _game.curState == GameState.Play)
        {
            Record();
        }
    }

    private void Update()
    {
        if (_game.curState == GameState.Play)
        {
            _time += Time.deltaTime;
        }
    }

    public void ClearRewindList()
    {
        rewindPos.Clear();
    }

    public void RecordCheckPoint(Vector2 pos)
    {
        rewindPos.Insert(0, pos);
    }

    private void Record()
    {
        if (_time >= 0.5f)
        {
            rewindPos.Insert(0, transform.position);
            _time = 0f;
        }
    }

    private void StartRewind()
    {
        isRewind = true;
        
    }

    public void StopRewind()
    {
        isRewind = false;
        isRecord = true;
    }
}
