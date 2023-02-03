using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : Singleton<ObjectPooling>
{
    [SerializeField] private GameObject _poolingPrefab;

    private Queue<GameObject> _pollingObjectQueue = new Queue<GameObject>();
    
    public int initCount;
    
    public override void Init()
    {
        for (int i = 0; i < initCount; i++)
        {
            _pollingObjectQueue.Enqueue(CreateNewObject());
        }
    }

    private GameObject CreateNewObject()
    {
        var newObj = Instantiate(_poolingPrefab);
        newObj.transform.SetParent(transform);
        newObj.SetActive(false);
        return newObj;
    }
    
    public GameObject GetObject(Vector3 touchPos)
    {
        if(_pollingObjectQueue.Count > 0)
        {
            var obj = _pollingObjectQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.transform.position = touchPos;
            obj.SetActive(true);
            return obj;
        }
        
        var newObj = CreateNewObject();
        newObj.gameObject.SetActive(true);
        newObj.transform.position = touchPos;
        newObj.transform.SetParent(null);
        return newObj;
    }

    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        _pollingObjectQueue.Enqueue(obj);
    }
}
