using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchEffectPool : ObjectPooling
{
    protected override void Init()
    {
        DontDestroyOnLoad(this);
        for (int i = 0; i < initCount; i++)
        {
            CreateNewObject();
        }
    }
}
