using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEditorInternal;
using UnityEngine;

[System.Serializable]
public class Sound
{
    public string name;     // 곡 제목
    public AudioClip clip;  // 곡
}
public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [SerializeField] private Sound[] _bgm;   // BGM 클립들
    [SerializeField] private Sound[] _sfx;   // SFX 클립들
    
    [SerializeField] private AudioSource _bgmPlayer;    // BGM 플레이어
    [SerializeField] private AudioSource[] _sfxPlayer;  // SFX 플레이어. 여러 개 재생될 수 있게 배열로 선언

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    // BGM
    public void PlayBGM(string bgmName)
    {
        for (int i = 0; i < _bgm.Length; i++)
        {
            if (bgmName == _bgm[i].name)
            {
                _bgmPlayer.clip = _bgm[i].clip;
                _bgmPlayer.Play();
            }
        }
    }
    public void StopBGM()
    {
        _bgmPlayer.Stop();
    }
    
    // SFX
    public void PlaySFX(string sfxName)
    {
        for (int i = 0; i < _sfx.Length; i++)
        {
            if (sfxName == _sfx[i].name) // SFX 배열에서 이름이 같은 곡 검사
            {
                for (int x = 0; i < _sfxPlayer.Length; x++)
                {
                    if (!_sfxPlayer[x].isPlaying) // 재생 중이지 않은 sfxPlayer 검사
                    {
                        _sfxPlayer[x].clip = _sfx[i].clip;
                        _sfxPlayer[x].Play();
                        return;
                    }
                }
                // Debug.Log("모든 오디오 플레이어가 재생중입니다.");
            }
        }
        // Debug.Log(sfxName + "이름의 효과음이 없습니다.");
    }
}
