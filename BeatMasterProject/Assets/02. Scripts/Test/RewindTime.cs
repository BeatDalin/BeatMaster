using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;


public class RewindTime : MonoBehaviour
{
    public bool isRewind;
    public bool isRecord = true;
    
    public List<RewindData> rewindList = new List<RewindData>();

    [SerializeField] private ParticleSystem[] _judgeParticle;
    [SerializeField] private ParticleSystemReverseSimulation[] _particleSystemReverseSimulations;

    private RewindData _rewindData;
    private int _countBtwStartEnd = 0;

    private float _time;
    private Game _game;

    private void Awake()
    {
        _game = FindObjectOfType<Game>();
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
        rewindList.Clear();
    }

    public void RecordRewindPoint(Vector2 pos, string result)
    {
        _rewindData = new RewindData();
        _rewindData.rewindPos = pos;
        _rewindData.judgeResult = result;

        rewindList.Insert(0, _rewindData);
    }

    public void RecordRewindPoint(Vector2 pos, string result, bool isStart)
    {
        _rewindData = new RewindData();
        _rewindData.rewindPos = pos;
        _rewindData.judgeResult = result;
        rewindList.Insert(0, _rewindData);
        if (!isStart)
        {
            // Long End
            rewindList[_countBtwStartEnd - 1] = _rewindData;
            _countBtwStartEnd = 0;
            return;
        }
        _countBtwStartEnd = rewindList.Count;
    }

    public void StartRewind()
    {
        isRewind = true;
        isRecord = false;
    }

    public void StopRewind()
    {
        isRewind = false;
        isRecord = true;
    }
    
    public IEnumerator RewindParticle(string result)
    {
        int idx = 0;
        
        switch (result)
        {
            case "Perfect":
                while (idx != 4)
                {
                    _particleSystemReverseSimulations[idx].enabled = true;
                    yield return new WaitForSeconds(0.1f);
                    idx++;
                }
                break;
            
            case "Fast":
                _particleSystemReverseSimulations[1].enabled = true;
                yield return new WaitForSeconds(0.2f);
                break;
            
            case "Slow":
                _particleSystemReverseSimulations[2].enabled = true;
                yield return new WaitForSeconds(0.2f);
                break;
        }

        for (int i = 0; i < _particleSystemReverseSimulations.Length - 1; i++)
        {
            _particleSystemReverseSimulations[i].enabled = false;
        }
    }
}
