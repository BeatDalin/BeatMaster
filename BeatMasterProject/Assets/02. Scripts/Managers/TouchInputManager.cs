using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class TouchInputManager : MonoBehaviour
{
    [Header("Screen Width")]
    private int _centerOfScreen;
    [Header("Touch")]
    public Touch touch;
    private Touch _leftTouch;
    private Touch _leftLongTouch;
    private Touch _leftTouchEnd; 
    private Touch _rightTouch;

    private void Start()
    {
        _centerOfScreen = Screen.width / 2;
    }

    // private void Update()
    // {
    //     // CheckTouch();
    // }
    
    private void CheckTouch()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                if (!EventSystem.current.IsPointerOverGameObject(i))
                {
                    touch = Input.GetTouch(i);
                    if (touch.position.x <= _centerOfScreen)
                    {
                        // Left Half of the Screen
                    }
                    else
                    {
                        // Right Half of the Screen
                    }
                }
            }
        }
    }

    public bool CheckLeftTouch()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                _leftTouch = Input.GetTouch(i);
                if (_leftTouch.position.x <= _centerOfScreen)
                {
                    if (_leftTouch.phase == TouchPhase.Began)
                    {
                        // Simple One Touch
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool CheckLeftTouching()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                _leftTouch = Input.GetTouch(i);
                if (_leftTouch.position.x <= _centerOfScreen)
                {
                    if (_leftTouch.phase == TouchPhase.Stationary || _leftTouch.phase == TouchPhase.Moved)
                    {
                        // Touching....
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool CheckLeftTouchEnd()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                _leftTouch = Input.GetTouch(i);
                if (_leftTouch.position.x <= _centerOfScreen)
                {
                    if (_leftTouch.phase == TouchPhase.Ended)
                    {
                        // Touch End
                        return true;
                    }
                }
            }
        }
        return false;
    }
    public bool CheckRightTouch()
    {
        if (Input.touchCount > 0)
        {
            for (int i = 0; i < Input.touchCount; i++)
            {
                _rightTouch = Input.GetTouch(i);
                if (_rightTouch.position.x > _centerOfScreen)
                {
                    if (_rightTouch.phase == TouchPhase.Began)
                    {
                        // Simple One Touch
                        return true;
                    }
                }
            }
        }
        return false;
    }
}
