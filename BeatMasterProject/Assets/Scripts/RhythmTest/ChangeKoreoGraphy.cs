using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo.Players;
using UnityEngine;
using UnityEngine.SceneManagement;
using SonicBloom.Koreo;


public class ChangeKoreoGraphy : MonoBehaviour
{
    [SerializeField] private string _currentScene = "";

    private SimpleMusicPlayer _musicPlayer;
    public Koreography koreo;

    private void Awake()
    {
        _musicPlayer = GetComponent<SimpleMusicPlayer>();
        _currentScene = SceneManager.GetActiveScene().name;

        ChangeKoreo();
    }

    private void ChangeKoreo()
    {
        switch (_currentScene)
        {
            case "MenuTitle":
            {
                koreo = Resources.Load<Koreography>("KoreoGraphys/MenuBGM");
                _musicPlayer.LoadSong(koreo, 0, false);
                break;
            }
            case "MenuLevelSelect":
            {
                koreo = Resources.Load<Koreography>("KoreoGraphys/MenuLevelBGM");
                _musicPlayer.LoadSong(koreo, 0, true);
                break;
            }

            case "LevelGame":
            {
                koreo = Resources.Load<Koreography>("KoreoGraphys/Level1");
                _musicPlayer.LoadSong(koreo, 0, false);
                break;
            }

        }
    }
}