using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    [SerializeField] private Camera _camera;

    // 1920 x 1080
    [SerializeField] private int _setWidth = 1920;    // 사용자 설정 너비(가로)
    [SerializeField] private int _setHeight = 1080;   // 사용자 설정 높이(세로)
    [SerializeField] private int _deviceWidth;  // 디바이스 너비 저장
    [SerializeField] private int _deviceHeight; // 디바이스 높이 저장
    
    private void Awake()
    {
        _camera = GetComponent<Camera>();

        
        CameraSetResolution();
        
    }

    private void CameraSetResolution()
    {
        _deviceWidth = Screen.width;
        _deviceHeight = Screen.height;
        
        Screen.SetResolution(_setWidth, (int)(((float)_deviceHeight / _deviceWidth) * _setWidth), true);

        // 기기의 해상도 비가 더 큰 경우
        if ((float)_setWidth / _setHeight < (float)_deviceWidth / _deviceHeight)
        {
            // 새로운 너비 
            float newWidth = ((float)_setWidth / _setHeight) / ((float)_deviceWidth / _deviceHeight);
            // 새로운 Rect 적용
            _camera.rect = new Rect((1f - newWidth) / 2f, 0f, newWidth, 1f);
        }
        
        // 게임의 해상도 비가 더 큰 경우
        else
        {
            // 새로운 높이
            float newHeight = ((float)_deviceWidth / _deviceHeight) / ((float)_setWidth / _setHeight);
            // 새로운 Rect 적용
            _camera.rect = new Rect(0f, (1f - newHeight) / 2f, 1f, newHeight);
        }
    }

    // OnPreCull() : 카메라가 씬을 추려내기 전에 호출
    private void OnPreCull() => GL.Clear(true, true, Color.black);
}