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
        Vector3 mouse = Input.mousePosition;

        if (SceneManager.GetActiveScene().name.Equals("MenuLevelSelect"))
        {
            mouse.y = 2f;
        }
        else
        {
            mouse.z = 20f;
        }
        
        Vector3 mPosition = Camera.main.ScreenToWorldPoint(mouse);
        
        if (SceneManager.GetActiveScene().name.Equals("MenuLevelSelect"))
        {
            mPosition = _levelMenuManager.ray.origin;
        }
        
        Debug.Log(mPosition);
        //mPosition.z = 20f;

        ObjectPooling.GetObject(mPosition);
        //Instantiate(_touchEffect, mPosition, Quaternion.identity);
    }
}