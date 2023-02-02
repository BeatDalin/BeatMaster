using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MouseClick : MonoBehaviour
{
    [SerializeField] private GameObject _touchEffect;

    public float defaultTime = 0.01f;
    
    private float _spawnTime;

    private LevelMenuManager _levelMenuManager;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name.Equals("MenuLevelSelect"))
        {
            _levelMenuManager = FindObjectOfType<LevelMenuManager>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && _spawnTime >= defaultTime)
        {
            InitTouchEffect();
            _spawnTime = 0f;
        }

        _spawnTime += Time.deltaTime;
    }

    private void InitTouchEffect()
    {
        
        if (SceneManager.GetActiveScene().name.Equals("MenuLevelSelect"))
        {
            Vector3 pos = _levelMenuManager.effectRayPoint.GetPoint(10f);
            
            ObjectPooling.GetObject(pos);
        }
        else
        {
            Vector3 mouse = Input.mousePosition;

            mouse.z = 20f;
            
            Vector3 mPosition = Camera.main.ScreenToWorldPoint(mouse);
            
            ObjectPooling.GetObject(mPosition);
        }
        
        
        //Instantiate(_touchEffect, mPosition, Quaternion.identity);
    }
}