using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraResolution : MonoBehaviour
{
    //[SerializeField] private Camera _camera;

    // 1920 x 1080
    [SerializeField] private int _setWidth = 1920;    // 사용자 설정 너비(가로)
    [SerializeField] private int _setHeight = 1080;   // 사용자 설정 높이(세로)
    [SerializeField] private int _deviceWidth;  // 디바이스 너비 저장
    [SerializeField] private int _deviceHeight; // 디바이스 높이 저장
    
    private void Awake()
    {
        //_camera = GetComponent<Camera>();

        
        //CameraSetResolution();
        
    }

    private void Start()
    {
        Camera camera = GetComponent<Camera>();
        Rect rect = camera.rect;
        float scaleheight = ((float)Screen.width / Screen.height) / ((float)16 / 9); // (가로 / 세로)
        float scalewidth = 1f / scaleheight;
        if (scaleheight < 1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleheight) / 2f;
        }
        else
        {
            rect.width = scalewidth;
            rect.x = (1f - scalewidth) / 2f;
        }
        camera.rect = rect;
    }
    //void OnPreCull() => GL.Clear(true, true, Color.black);
    
    
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
