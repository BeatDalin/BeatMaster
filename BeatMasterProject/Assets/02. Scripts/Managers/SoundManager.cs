using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

[Serializable]
public class Sound
{
    public string name; // 곡 제목
    public AudioClip clip; // 곡
}

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    [Header("Clips")]
    [SerializeField] private Sound[] _bgm; // BGM 클립들
    [SerializeField] private Sound[] _sfx; // SFX 클립들

    [Header("Audio")]
    [SerializeField] private AudioSource _bgmPlayer; // BGM 플레이어
    [SerializeField] private AudioSource[] _sfxPlayer; // SFX 플레이어. 여러 개 재생될 수 있게 배열로 선언
    [SerializeField] private AudioMixer _mixer;

    [Header("Koreography")]
    public Koreography playingKoreo;
    public SimpleMusicPlayer musicPlayer;
    public string clipName;

    public Koreography[] koreographies; // An array of Koreography assets

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

        musicPlayer = GetComponentInChildren<SimpleMusicPlayer>();
        musicPlayer.TryGetComponent(typeof(AudioSource), out Component audioSource);
        if (audioSource)
        {
            _bgmPlayer = audioSource as AudioSource;
        }
    }

    private void Start()
    {
        CheckAudioSetting();
    }

    private void CheckAudioSetting()
    {
        if (PlayerPrefs.HasKey("Music"))
        {
            _mixer.SetFloat("BGMVolume", Mathf.Log10(PlayerPrefs.GetFloat("Music")) * 20);
        }
        else
        {
            _mixer.SetFloat("BGMVolume", 0);
        }

        if (PlayerPrefs.HasKey("Sfx"))
        {
            _mixer.SetFloat("SFXVolume", Mathf.Log10(PlayerPrefs.GetFloat("Sfx")) * 20);

        }
        else
        {
            _mixer.SetFloat("SFXVolume", 0);
        }

        if (PlayerPrefs.HasKey("Vibrator"))
        {
            float v_temp = PlayerPrefs.GetFloat("Vibrator");
            ExcuteVibration.Instance.vibrationPower = (int)(v_temp * 100f);            
        }
        else
        {
            ExcuteVibration.Instance.vibrationPower = 100;
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

    public void PlayBGM(bool wannaPlay, int sampleTime = 0)
    {
        musicPlayer.LoadSong(playingKoreo, sampleTime, false);
        if (wannaPlay)
        {
            musicPlayer.Play();
        }
        else
        {
            musicPlayer.Pause();
        }
    }
    
    // SFX
    public void PlaySFX(string sfxName)
    {
        for (int i = 0; i < _sfx.Length; i++)
        {
            if (sfxName == _sfx[i].name) // SFX 배열에서 이름이 같은 곡 검사
            {
                for (int x = 0; x < _sfxPlayer.Length; x++)
                {
                    if (!_sfxPlayer[x].isPlaying) // 재생 중이지 않은 sfxPlayer 검사
                    {
                        _sfxPlayer[x].clip = _sfx[i].clip;
                        _sfxPlayer[x].volume = 0.5f;
                        _sfxPlayer[x].PlayOneShot(_sfxPlayer[x].clip);
                        
                        return;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Clears Koreo EventRegister, Unloads previous Koreography, Set new Koreography and MusicPlayer for next scene. 
    /// </summary>
    /// <param name="currentScene">Enum SceneType</param>
    public void ChangeKoreo(SceneLoadManager.SceneType currentScene)
    {
        Koreographer.Instance.ClearEventRegister(); // Clear Events before moving scenes!!!
        // Unload Koreography before moving scene
        if (Koreographer.Instance.GetNumLoadedKoreography() > 0)
        {
            Koreographer.Instance.UnloadKoreography(playingKoreo);
        }

        _bgmPlayer.loop = false;
        switch (currentScene)
        {
            case SceneLoadManager.SceneType.Title: // Title 씬
            {
                playingKoreo = koreographies[0];
                // playingKoreo = Resources.Load<Koreography>("KoreoGraphys/Title");
                musicPlayer.LoadSong(playingKoreo, 0, false);
                _bgmPlayer.loop = true;
                break;
            }
            case SceneLoadManager.SceneType.LevelSelect: // LevelSelect 씬
            {
                playingKoreo = koreographies[1];
                // playingKoreo = Resources.Load<Koreography>("KoreoGraphys/LevelSelect");
                musicPlayer.LoadSong(playingKoreo, 0, true);
                _bgmPlayer.loop = true;
                break;
            }
            case SceneLoadManager.SceneType.Stage1_Level1: // Forest
            {
                playingKoreo = koreographies[2];
                // playingKoreo = Resources.Load<Koreography>("KoreoGraphys/Level1");
                musicPlayer.LoadSong(playingKoreo, 0, false);
                break;
            }
            
            case SceneLoadManager.SceneType.Stage2_Level1: // City
            {
                playingKoreo = koreographies[3];
                // playingKoreo = Resources.Load<Koreography>("KoreoGraphys/Level1");
                musicPlayer.LoadSong(playingKoreo, 0, false);
                break;
            }
            
            case SceneLoadManager.SceneType.Stage3_Level1: // Desert
            {
                playingKoreo = koreographies[4];
                // playingKoreo = Resources.Load<Koreography>("KoreoGraphys/Level1");
                musicPlayer.LoadSong(playingKoreo, 0, false);
                break;
            }
            
            case SceneLoadManager.SceneType.Stage4_Level1: // Glacier
            {
                playingKoreo = koreographies[5];
                // playingKoreo = Resources.Load<Koreography>("KoreoGraphys/Level1");
                musicPlayer.LoadSong(playingKoreo, 0, false);
                break;
            }
        }
        // Load next scene's Koreography
        Koreographer.Instance.LoadKoreography(playingKoreo);
        clipName = musicPlayer.GetCurrentClipName();

    }
}