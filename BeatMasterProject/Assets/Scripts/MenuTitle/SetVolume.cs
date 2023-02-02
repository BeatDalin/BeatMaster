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
    public void SetMenuBGMVolume(float sliderValue)
    {
        _mixer.SetFloat("MenuBGMVolume", Mathf.Log10(sliderValue) * 20);
    }
    public void SetMenuSFXVolume(float sliderValue)
    {
        _mixer.SetFloat("MenuSFXVolume", Mathf.Log10(sliderValue) * 20);
    } 
    public void SetGameBGMVolume(float sliderValue)
    {
        _mixer.SetFloat("GameBGMVolume", Mathf.Log10(sliderValue) * 20);    
    }
    public void SetGameSFXVolume(float sliderValue) 
    { 
        _mixer.SetFloat("GameSFXVolume", Mathf.Log10(sliderValue) * 20);
    }
}
