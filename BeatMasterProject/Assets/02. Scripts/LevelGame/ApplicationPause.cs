using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationPause : MonoBehaviour
{
    private bool _isPause;
    private Game _game;
    
    // Start is called before the first frame update
    void Start()
    {
        _game = FindObjectOfType<Game>();
    }

    private void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
        {
            _isPause = true;
            _game.PauseGame();  // 게임 일시 정지
        }
        else
        {
            if (_isPause)
            {
                _isPause = false;
                _game.ContinueGame();   // 게임 재개
            }
        }
    }
}
