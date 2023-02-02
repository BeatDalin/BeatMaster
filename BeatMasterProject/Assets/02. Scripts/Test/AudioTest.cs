    using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    public static AudioTest instance;
    
    private AudioSource _audioSource;

    public bool isPlaying = false;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();
        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameStart();
        }
    }

    private void GameStart()
    {
        if (!_audioSource.isPlaying)
        {
            isPlaying = true;
            _audioSource.Play();
        }
    }
}
