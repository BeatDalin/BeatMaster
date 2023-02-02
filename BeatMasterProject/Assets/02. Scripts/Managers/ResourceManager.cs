using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : Singleton<ResourceManager>
{
    public GameObject LoadingUI { get; private set; }

    // 씬보다 얘를 먼저 호출해야 됨
    // Global은 여기서 Load
    public void Init()
    {
        LoadingUI = Resources.Load<GameObject>($"UI/LoadingCanvas");
    }
}
