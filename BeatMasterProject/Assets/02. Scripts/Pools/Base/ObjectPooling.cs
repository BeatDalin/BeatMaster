using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public abstract class ObjectPooling : Singleton<ObjectPooling>
{ 
    [SerializeField] protected GameObject poolingPrefab;

    protected Queue<GameObject> poolingObjectQueue = new Queue<GameObject>();
    
    [SerializeField] protected int initCount;

    protected GameObject CreateNewObject()
    {
        var newObj = Instantiate(poolingPrefab);
        newObj.transform.SetParent(transform);
        newObj.SetActive(false);
        return newObj;
    }
    
    public virtual GameObject GetObject(Vector3 touchPos)
    {
        var obj = poolingObjectQueue.Dequeue();
        obj.transform.SetParent(null);
        obj.transform.position = touchPos;
        obj.SetActive(true);
        return obj;
    }

    public virtual void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        poolingObjectQueue.Enqueue(obj);
    }
}
