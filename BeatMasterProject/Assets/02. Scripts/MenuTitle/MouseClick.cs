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

    private bool _isChangeScene;

    private string _currentScene = "MenuTitle";

    // Update is called once per frame
    void Update()
    {
        if (!_currentScene.Equals(SceneManager.GetActiveScene().name))
        {
            _isChangeScene = false;
            _currentScene = SceneManager.GetActiveScene().name;
        }
        
        if (Input.GetMouseButton(0) && _spawnTime >= defaultTime)
        {
            InitTouchEffect();
            _spawnTime = 0f;
        }

        if (SceneManager.GetActiveScene().name.Equals(_currentScene) && !_isChangeScene)
        {
            _isChangeScene = true;
            
            if (SceneManager.GetActiveScene().name.Equals("MenuLevelSelect"))
            {
                _levelMenuManager = FindObjectOfType<LevelMenuManager>();
            }
        }

        _spawnTime += Time.deltaTime;
    }

    private void InitTouchEffect()
    {
        
        if (SceneManager.GetActiveScene().name.Equals("MenuLevelSelect"))
        {
            Vector3 pos = _levelMenuManager.effectRayPoint.GetPoint(10f);
            
            ObjectPooling.Instance.GetObject(pos);
        }
        else
        {
            Vector3 mouse = Input.mousePosition;

            mouse.z = 20f;
            
            Vector3 mPosition = Camera.main.ScreenToWorldPoint(mouse);
            
            ObjectPooling.Instance.GetObject(mPosition);
        }
    }
}