using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseClick : MonoBehaviour
{
    [SerializeField] private GameObject _touchEffect;

    public float defaultTime = 0.05f;
    
    private float _spawnTime;
    
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
        Vector3 mPosition = Camera.main.WorldToScreenPoint(Input.mousePosition);

        mPosition.z = 20f;

        Instantiate(_touchEffect, mPosition, Quaternion.identity);
        
    }
}
