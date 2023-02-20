using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ObjectPooling : MonoBehaviour
{
    [SerializeField] protected GameObject poolingPrefab;

    protected Queue<GameObject> poolingObjectQueue = new Queue<GameObject>();
    
    [SerializeField] protected int initCount;

    private void Awake()
    {
        Init();
    }

    protected abstract void Init();

    protected GameObject CreateNewObject()
    {
        var newObj = Instantiate(poolingPrefab);
        poolingObjectQueue.Enqueue(newObj);
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

    public GameObject GetObject()
    {
        var obj = poolingObjectQueue.Dequeue();
        obj.transform.SetParent(null);
        obj.SetActive(true);
        return obj;
    }

    public virtual void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(transform);
        poolingObjectQueue.Enqueue(obj);
        Debug.Log(poolingObjectQueue.Count); 
    }
}
