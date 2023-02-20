using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseClick : MonoBehaviour
{
    public float defaultTime = 0.01f;
    
    private float _spawnTime;

    private LevelMenuManager _levelMenuManager;

    private TouchEffectPool _touchEffectPool;

    private bool _isChangeScene;

    private string _currentScene = "MenuTitle";
    
    [SerializeField] private Game _game;

    private void Awake()
    {
        _touchEffectPool = FindObjectOfType<TouchEffectPool>();
    }

    // Update is called once per frame
    void Update()
    {
        // if (!_currentScene.Equals(SceneManager.GetActiveScene().name))
        // {
        //     _isChangeScene = false;
        //     _currentScene = SceneManager.GetActiveScene().name;
        // }

        if (SceneManager.GetActiveScene().name.Equals("Level1"))
        {
            if (_game == null)
            {
                _game = FindObjectOfType<Game>();
            }
        }

        if (_game != null)
        {
            if (!_game.curState.Equals(GameState.Play))
            {
                if (Input.GetMouseButton(0) && _spawnTime >= defaultTime)
                {
                    InitTouchEffect();
                    _spawnTime = 0f;
                }
            }
        }
        else
        {
            if (Input.GetMouseButton(0) && _spawnTime >= defaultTime)
            {
                InitTouchEffect();
                _spawnTime = 0f;
            }
        }

        // if (SceneManager.GetActiveScene().name.Equals(_currentScene) && !_isChangeScene)
        // {
        //     _isChangeScene = true;
        //     
        //     if (SceneManager.GetActiveScene().name.Equals("MenuLevelSelect"))
        //     {
        //         _levelMenuManager = FindObjectOfType<LevelMenuManager>();
        //     }
        // }

        _spawnTime += Time.deltaTime;
    }

    private void InitTouchEffect()
    {
        
        Vector3 mouse = Input.mousePosition;

        mouse.z = 20f;
            
        Vector3 mPosition = Camera.main.ScreenToWorldPoint(mouse);
            
        _touchEffectPool.GetObject(mPosition);
    }
}