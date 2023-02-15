using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputManager : MonoBehaviour
{

    // 화면의 중앙 지점 ( width / 2 )
    private int _centerOfScreen;
    private Touch _touchs;
    
    public bool isJumpTouch;    // 점프
    public bool isAttackTouch;  // 공격

    private Game _game;
    
    private void Start()
    {
        _game = FindObjectOfType<Game>();
        _centerOfScreen = (Screen.width / 2);
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
                    _touchs = Input.GetTouch(i);

                    // 터치 시작 시
                    if (_touchs.phase == TouchPhase.Began)
                    {
                        // 화면의 중앙 지점 왼쪽 터치 시
                        if (_touchs.position.x < _centerOfScreen)
                        {
                            isJumpTouch = true;
                            Debug.Log("isJump Began= " + isJumpTouch);
                            break;
                        }
                        if (_touchs.position.x > _centerOfScreen)
                        {
                            isAttackTouch = true;
                            Debug.Log("isAttack Began= " + isAttackTouch);
                            break;
                        }
                    }
                    else
                    {
                        if (_touchs.position.x < _centerOfScreen)
                        {
                            isJumpTouch = false;
                            Debug.Log("isJump = " + isJumpTouch);
                            break;
                        }
                        if (_touchs.position.x > _centerOfScreen)
                        {
                            isAttackTouch = false;
                            Debug.Log("isAttack = " + isAttackTouch);

                            break;
                        }
                    }
                }
            }
        }
    }
}

