using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class Note : MonoBehaviour
{
    private RewindTime _rewindTime;
    private CharacterMovement _characterMovement;
    public string beatResult;
    private Game _game;
    private bool _isPlay = true;
    private bool _isRewindParticle;
    private WaitForSeconds _waitForSecondsEnumerator;

    [SerializeField] private ParticleSystem[] _particleSystems;
    [SerializeField] private ParticleSystemReverseSimulation[] _particleSystemReverseSimulations;

    private void Awake()
    {
        _waitForSecondsEnumerator = new WaitForSeconds(0.1f); 
        _particleSystems = new ParticleSystem[4];
        _rewindTime = FindObjectOfType<RewindTime>();
        _characterMovement = FindObjectOfType<CharacterMovement>();
        
        for (int i = 0; i < transform.GetChild(1).childCount; i++)
        {
            _particleSystems[i] = transform.GetChild(1).GetChild(i).GetComponent<ParticleSystem>();
        }
        _particleSystemReverseSimulations = transform.GetChild(1).GetComponentsInChildren<ParticleSystemReverseSimulation>();
    }

    private void Update()
    {
        if (_rewindTime.isRewind)
        {
            if (!_isRewindParticle)
            {
                if (Mathf.Abs(_characterMovement.transform.position.x - transform.position.x) <= 1f)
                {
                    _isRewindParticle = true;
                    StartCoroutine(CoRewindParticle(beatResult)); //파티클 되돌리는 함수 실행
                }
            }
        }

        if (!string.IsNullOrEmpty(beatResult) && _isPlay)
        {
            _isRewindParticle = false;
            _isPlay = false;
            ChangeOutLineColor(beatResult); //파티클 재생함수 실행
        }
    }

    public void ChangeOutLineColor(string result)
    {
        switch (result)
        {
            case "Perfect":
                StartCoroutine(CoStartParticle());
                break;

            case "Fast":
                _particleSystems[1].Play();
                break;

            case "Slow":
                _particleSystems[2].Play();
                break;

            case "Fail":
                _particleSystems[3].Play();
                break;
        }
    }

    private IEnumerator CoStartParticle()
    {
        int i = 0;
        while (i != _particleSystems.Length)
        {
            _particleSystems[i].Play();
            yield return _waitForSecondsEnumerator;
            i++;
        }
    }

    public IEnumerator CoRewindParticle(string result)
    {
        int idx = _particleSystems.Length - 1;

        switch (result)
        {
            case "Perfect":
                while (idx != -1)
                {
                    _particleSystemReverseSimulations[idx].enabled = true;
                    yield return _waitForSecondsEnumerator;
                    idx--;
                }

                break;

            case "Fast":
                _particleSystemReverseSimulations[1].enabled = true;
                yield return _waitForSecondsEnumerator;
                break;

            case "Slow":
                _particleSystemReverseSimulations[2].enabled = true;
                yield return _waitForSecondsEnumerator;
                break;
        }
        
        yield return _waitForSecondsEnumerator;
        
        for (int i = _particleSystems.Length - 1; i > -1; i--)
        {
            _particleSystemReverseSimulations[i].enabled = false;
        }
        
        _isPlay = true;
        beatResult = "";
    }
}