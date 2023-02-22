using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorController : MonoBehaviour
{
    [SerializeField] private Transform[] _indicatorsTransforms;
    [SerializeField] private float _waitTime;
    private void Awake()
    {
        
    }

    private IEnumerator CoMoveTrans()
    {
        yield break;
    }

    void Update()
    {
        
    }
}
