using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomScrollRect : ScrollRect
{
    public void SetHorizontalNormalizedPosition(float value) { SetNormalizedPosition(value, 0); }
}
