using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputManager : MonoBehaviour
{
    // 화면의 중앙 지점 ( width / 2 )
    public int centerOfScreen;
    
    public Touch touchs;
    private Game _game;
    
    private void Start()
    {
        centerOfScreen = Screen.width / 2;
        _game = FindObjectOfType<Game>();
    }

    private void Update()
    {
        TouchInput();
    }

    public void TouchInput()
    {
        if (_game.curState != GameState.Play)
        {
            return;
        }
        // 터치가 1개 이상이면
        if (Input.touchCount > 0)
        {
            //Input.touchCount를 통해 입력받은 숫자만큼 반복
            for (int i = 0; i < Input.touchCount; i++)
            {
                // UI 클릭 시 터치 이벤트 발생 X
                if (!EventSystem.current.IsPointerOverGameObject(i))
                {
                    touchs = Input.GetTouch(i);
                }
            }
        }
    }
}

