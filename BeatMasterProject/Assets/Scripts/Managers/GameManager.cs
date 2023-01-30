using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : Singleton<GameManager>
{
    private void Awake()
    {
        ResourceManager.Instance.Init();
        SceneLoadManager.Instance.Init();
    }

}
