using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SetVolume : MonoBehaviour
{
    [SerializeField] private AudioMixer _mixer;

    [SerializeField] private Slider _vibationPower;
    [SerializeField] private Slider _musicVolume;
    [SerializeField] private Slider _sfxVolume;
    private void OnEnable()
    {
        _musicVolume.value = GetBgmVolume();
        _sfxVolume.value = GetSfxVolume();
        _vibationPower.value = ExcuteVibration.Instance.vibrationPower;
    }

    public float GetBgmVolume()
    {
        float value;
        
        bool result =  _mixer.GetFloat("BGMVolume", out value);
        
        value = Mathf.Pow(10, value/20);
        
        if(result)
        {
            return value;
        }
        else
        {
            return 0f;
        }
    }

    public float GetSfxVolume()
    {
        float value;
        
        bool result =  _mixer.GetFloat("SFXVolume", out value);
        
        value = Mathf.Pow(10, value/20);
        
        if(result)
        {
            return value;
        }
        else
        {
            return 0f;
        }
    }

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
    public void SetVibrationPower()
    {
        ExcuteVibration.Instance.vibrationPower = (int)_vibationPower.value;
    }
    public int GetVibrationPower()
    {
        return ExcuteVibration.Instance.vibrationPower;
    }
}
