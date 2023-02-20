using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    [SerializeField] private Camera _camera;
    [SerializeField] private Rect _rect;
    [SerializeField] private float _scaleHeight;
    [SerializeField] private float _scaleWidth;
    
    
    private void Awake()
    {
        CameraSetResolution();
    }
    private void CameraSetResolution()
    {
        _camera = GetComponent<Camera>();
        _rect = _camera.rect;
        _scaleHeight = ((float)Screen.width / Screen.height) / ((float)16 / 9); // (가로 / 세로)
        _scaleWidth = 1f / _scaleHeight;
        if (_scaleHeight < 1)
        {
            _rect.height = _scaleHeight;
            _rect.y = (1f - _scaleHeight) / 2f;
        }
        else
        {
            _rect.width = _scaleWidth;
            _rect.x = (1f - _scaleWidth) / 2f;
        }
        _camera.rect = _rect;
    }
    void OnPreCull() => GL.Clear(true, true, Color.black);
    
    
    
    
    
    // private void CameraSetResolution()
    // {
    //     _deviceWidth = Screen.width;
    //     _deviceHeight = Screen.height;
    //
    //     Debug.Log("기본해상도" + _deviceWidth +" "+ _deviceHeight);
    //     
    //     //Screen.SetResolution(_setWidth, (int)(((float)_deviceHeight / _deviceWidth) * _setWidth), true);
    //
    //     // 기기의 해상도 비가 더 큰 경우
    //     if ((float)_setWidth  < (float)_deviceWidth )
    //     {
    //         // 새로운 너비 
    //         float newWidth = ((float)_setWidth / _setHeight) / ((float)_deviceWidth / _deviceHeight);
    //
    //         Debug.Log("바뀐 해상도 : " + newWidth);
    //         // 새로운 Rect 적용
    //         _camera.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
    //     }
    //     
    //     if ((float)_setHeight  < (float)_deviceHeight )
    //     {
    //         // 새로운 너비 
    //         float newHeight = ((float)_deviceWidth / _deviceHeight) / ((float)_setWidth / _setHeight);
    //
    //         Debug.Log("바뀐 해상도 : " + newHeight);
    //         // 새로운 Rect 적용
    //         _camera.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
    //     }
    //     
    //     // 게임의 해상도 비가 더 큰 경우
    //     // else
    //     // {
    //     //     // 새로운 높이
    //     //     float newHeight = ((float)_deviceWidth / _deviceHeight) / ((float)_setWidth / _setHeight);
    //     //
    //     //     Debug.Log("바뀐 해상도 : " + newHeight);
    //     //     // 새로운 Rect 적용
    //     //     _camera.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
    //     // }
    // }

    // OnPreCull() : 카메라가 씬을 추려내기 전에 호출
    //private void OnPreCull() => GL.Clear(true, true, Color.black);
}
