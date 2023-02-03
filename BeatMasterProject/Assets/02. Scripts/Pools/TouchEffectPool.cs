using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchEffectPool : ObjectPooling
{
    public override void Init()
    {
        for (int i = 0; i < initCount; i++)
        {
            pollingObjectQueue.Enqueue(CreateNewObject());
        }
    }
}
