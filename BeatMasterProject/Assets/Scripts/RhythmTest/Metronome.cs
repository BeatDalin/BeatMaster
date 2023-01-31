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

    private float tickInterval;
    private float nextTick;


    void Start()
    {
        tickInterval = 60f / bpm * signatureBottom / signatureTop;
        nextTick = Time.time + tickInterval;
    }

    void Update()
    {
        Metro();
    }

    private void Metro()
    {
        if (Time.time > nextTick)
        {
            
            audioSource.PlayOneShot(tickSound);
            nextTick = Time.time + tickInterval;

        }
    }
    
}




