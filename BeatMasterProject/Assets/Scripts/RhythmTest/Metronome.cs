using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Metronome : MonoBehaviour
{
    public int bpm;
    public int signatureTop = 4;
    public int signatureBottom = 4;
    public AudioClip tickSound;
    public AudioSource audioSource;

    private float _tickInterval;
    private float _nextTick;


    private void Start()
    {
        _tickInterval = 60f / bpm * signatureBottom / signatureTop;
        _nextTick = Time.time + _tickInterval;
    }

    private void Update()
    {
        Metro();
    }

    private void Metro()
    {
        if (Time.time > _nextTick)
        {
            
            audioSource.PlayOneShot(tickSound);
            _nextTick = Time.time + _tickInterval;

        }
    }
    
}




