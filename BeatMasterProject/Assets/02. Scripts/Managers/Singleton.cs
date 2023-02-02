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
        get
        {
            if (!_instance)
            {
                GameObject go = new GameObject($"{typeof(T)}");
                _instance = go.AddComponent<T>();
                DontDestroyOnLoad(_instance);
            }
            return _instance;
        }
    }

}
