using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SetVolume : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;
    
    public void SetBGMVolume(float sliderValue)
    {
        _mixer.SetFloat("BGMVolume", Mathf.Log10(sliderValue) * 20);
    }
    public void SetSFXVolume(float sliderValue)
    {
        _mixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
    }
}
