using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Note : MonoBehaviour
{
    private RewindTime _rewindTime;
    private CharacterMovement _characterMovement;
    public string _beatResult;
    private Game _game;
    private bool isPlay = true;
    private bool isRewindParticle;
    public int currentIdx;

    private bool waitResult;

    [SerializeField] private ParticleSystem[] _particleSystems;
    [SerializeField] private ParticleSystemReverseSimulation[] _particleSystemReverseSimulations;

    private void Awake()
    {
        _particleSystems = new ParticleSystem[4];
        _rewindTime = FindObjectOfType<RewindTime>();
        _characterMovement = FindObjectOfType<CharacterMovement>();
        _particleSystems[0] = transform.GetChild(1).GetChild(0).GetComponent<ParticleSystem>();
        _particleSystems[1] = transform.GetChild(1).GetChild(1).GetComponent<ParticleSystem>();
        _particleSystems[2] = transform.GetChild(1).GetChild(2).GetComponent<ParticleSystem>();
        _particleSystems[3] = transform.GetChild(1).GetChild(3).GetComponent<ParticleSystem>();
        _particleSystemReverseSimulations = transform.GetChild(1).GetComponentsInChildren<ParticleSystemReverseSimulation>();
    }

    private void Update()
    {
        if (_rewindTime.isRewind)
        {
            if (!isRewindParticle)
            {
                Debug.Log("파티클 기다리는중");
                if (Mathf.Abs(_characterMovement.transform.position.x - transform.position.x) <= 1f)
                {
                    Debug.Log("되돌리기 파티클 재생");
                    isRewindParticle = true;
                    StartCoroutine(RewindParticle(_beatResult)); //파티클 되돌리는 함수 실행
                }
            }
        }

        if (_beatResult != "" && isPlay)
        {
            isRewindParticle = false;
            isPlay = false;
            ChangeOutLineColor(_beatResult); //파티클 재생함수 실행
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
        while (i != _particleSystems.Length - 1)
        {
            _particleSystems[i].Play();
            yield return new WaitForSeconds(0.1f);
            i++;
        }
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
        isPlay = true;
        _beatResult = "";
    }
}