using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UICamera : MonoBehaviour
{
    [SerializeField] private Camera _mainCamera;

    private Camera uiCamera;

    private void Start()
    {
        uiCamera = GetComponent<Camera>();
    }

    private void Update()
    {
        if (_mainCamera.orthographicSize != uiCamera.orthographicSize)
        {
            uiCamera.orthographicSize = _mainCamera.orthographicSize;
        }
    }
}
