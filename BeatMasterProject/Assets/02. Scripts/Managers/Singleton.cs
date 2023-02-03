using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T _instance;

    public static T Instance
    {
        get => _instance;
    }

    private void Awake()
    {
        if (!_instance)
        {
            _instance = GetComponent<T>();
        }
        Init();
    }

    public abstract void Init();
}
