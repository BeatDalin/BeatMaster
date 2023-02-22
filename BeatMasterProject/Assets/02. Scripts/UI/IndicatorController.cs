using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IndicatorController : MonoBehaviour
{
    private Dictionary<Transform, List<Animator>> _childrenDict = new Dictionary<Transform, List<Animator>>();
    private void Awake()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childTrans = transform.GetChild(i).transform;
            _childrenDict.Add(childTrans, new List<Animator>(childTrans.GetComponentsInChildren<Animator>()));
            
        }
        
    }

    void Update()
    {
        
    }
}
