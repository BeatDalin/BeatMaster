using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using UnityEngine;
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

    [SerializeField] private Sound[] _bgm; // BGM 클립들
    [SerializeField] private Sound[] _sfx; // SFX 클립들

    [SerializeField] private AudioSource _bgmPlayer; // BGM 플레이어
    [SerializeField] private AudioSource[] _sfxPlayer; // SFX 플레이어. 여러 개 재생될 수 있게 배열로 선언

    [Header("Koreography")]
    public Koreography playingKoreo;
    public SimpleMusicPlayer musicPlayer;
    public string clipName;

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
        playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);

        
        musicPlayer.TryGetComponent(typeof(AudioSource), out Component audioSource);
        if (audioSource)
        {
            _bgmPlayer = audioSource as AudioSource;
        }
    }

    private void Start()
    {
        ChangeKoreo(SceneLoadManager.Instance.Scene);
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
            }
        }
    }

    // 각 씬별로 KoreoGraphy 바꾸는 기능
    public void ChangeKoreo(Enum currentScene)
    {
        switch (currentScene)
        {
            case SceneLoadManager.SceneType.Title: // Title 씬
            {
                playingKoreo = Resources.Load<Koreography>("KoreoGraphys/Title");
                musicPlayer.LoadSong(playingKoreo, 0, false);
                clipName = musicPlayer.GetCurrentClipName();
                break;
            }
            case SceneLoadManager.SceneType.LevelSelect: // LevelSelect 씬
            {
                playingKoreo = Resources.Load<Koreography>("KoreoGraphys/LevelSelect");
                musicPlayer.LoadSong(playingKoreo, 0, true);
                clipName = musicPlayer.GetCurrentClipName();
                break;
            }
            case SceneLoadManager.SceneType.Level1: // Level1 씬
            {
                playingKoreo = Resources.Load<Koreography>("KoreoGraphys/Level1");
                musicPlayer.LoadSong(playingKoreo, 0, false);
                clipName = musicPlayer.GetCurrentClipName();
                break;
            }
            case SceneLoadManager.SceneType.Level1MonsterTest: // Level1 씬
            {
                playingKoreo = Resources.Load<Koreography>("KoreoGraphys/Level1");
                musicPlayer.LoadSong(playingKoreo, 0, false);
                clipName = musicPlayer.GetCurrentClipName();
                break;
            }
            
            
            
        }
    }
}