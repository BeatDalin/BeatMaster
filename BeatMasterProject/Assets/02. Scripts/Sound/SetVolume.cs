using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;
    
    // Slider 오브젝트의 최소~최대값은 0.0001 ~ 1
    // 최소값 0.0001을 넣으면 -80, 1을 넣으면 0이 나옴
    // AudioMixer의 최소~최대값은 -80~0임
    public void SetMasterVolume(float sliderValue)
    {
        _mixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20);
    }
    public void SetBGMVolume(float sliderValue)
    {
        _mixer.SetFloat("BGMVolume", Mathf.Log10(sliderValue) * 20);
    }
    public void SetSFXVolume(float sliderValue)
    {
        _mixer.SetFloat("SFXVolume", Mathf.Log10(sliderValue) * 20);
    } 
}
