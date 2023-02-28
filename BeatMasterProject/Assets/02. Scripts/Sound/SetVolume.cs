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
        CheckPlayerPrefs();
    }

    private void CheckPlayerPrefs()
    {
        if (PlayerPrefs.HasKey("Music"))
        {
            _musicVolume.value = PlayerPrefs.GetFloat("Music");
        }
        else
        {
            _musicVolume.value = GetBgmVolume();
            PlayerPrefs.SetFloat("Music", _musicVolume.value);
        }

        if (PlayerPrefs.HasKey("Sfx"))
        {
            _sfxVolume.value = PlayerPrefs.GetFloat("Sfx");
        }
        else
        {
            _sfxVolume.value = GetSfxVolume();
            PlayerPrefs.SetFloat("Sfx", _sfxVolume.value);
        }

        if (PlayerPrefs.HasKey("Vibrator"))
        {
            _vibationPower.value = PlayerPrefs.GetFloat("Vibrator");
        }
        else
        {
            _vibationPower.value = GetVibrationPower();
            PlayerPrefs.GetFloat("Vibrator", _vibationPower.value);
        }
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
    
    public int GetVibrationPower()
    {
        return ExcuteVibration.Instance.vibrationPower;
    }

    private void OnDisable()
    {
        PlayerPrefs.SetFloat("Music", GetBgmVolume());
        Debug.Log($"Music {PlayerPrefs.GetFloat("Music")}");
        PlayerPrefs.SetFloat("Sfx", GetSfxVolume());
        Debug.Log($"Sfx {PlayerPrefs.GetFloat("Sfx")}");
        PlayerPrefs.SetFloat("Vibrator", _vibationPower.value);
        Debug.Log($"Vibrator {PlayerPrefs.GetFloat("Vibrator")}");
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
        ExcuteVibration.Instance.vibrationPower = (int)(_vibationPower.value * 100f);
    }
}
