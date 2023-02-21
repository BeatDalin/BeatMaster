using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationPause : MonoBehaviour
{
    private bool _isPause;
    private Game _game;
    private GameUI _gameUI;
    // Start is called before the first frame update
    void Start()
    {
        _gameUI = FindObjectOfType<GameUI>();
        _game = FindObjectOfType<Game>();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            _isPause = true;
            _game.PauseGame();  // 게임 일시 정지
            _gameUI.OpenPause();
            
        }
        // else
        // {
        //     if (_isPause)
        //     {
        //         _isPause = false;
        //         _game.ContinueGame();   // 게임 재개
        //     }
        // }
    }
}
