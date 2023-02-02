using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling instance;
    
    [SerializeField] private GameObject _poolingPrefab;

    private Queue<GameObject> _pollingObjectQueue = new Queue<GameObject>();
    
    public int initCount;
    
    private void Awake()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;
        
        DontDestroyOnLoad(gameObject);

        Init(initCount);
    }
    
    private void Init(int count)
    {
        for (int i = 0; i < count; i++)
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
    
    public static GameObject GetObject(Vector3 touchPos)
    {
        if(instance._pollingObjectQueue.Count > 0)
        {
            var obj = instance._pollingObjectQueue.Dequeue();
            obj.transform.SetParent(null);
            obj.transform.position = touchPos;
            obj.SetActive(true);
            return obj;
        }
        
        var newObj = instance.CreateNewObject();
        newObj.gameObject.SetActive(true);
        newObj.transform.position = touchPos;
        newObj.transform.SetParent(null);
        return newObj;
    }

    public static void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
        obj.transform.SetParent(instance.transform);
        instance._pollingObjectQueue.Enqueue(obj);
    }
}
