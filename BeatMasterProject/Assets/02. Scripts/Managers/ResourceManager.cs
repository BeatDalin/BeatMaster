using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class ResourceManager : MonoBehaviour
{
    public static ResourceManager instance;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    // Load 제네릭 사용자 지정 함수 정의
    // where T : Object => 부모 클래스가 Object인 타입만 받을 수 있도록 제약 걸음
    // Resources.Load<T>(path) => Resources폴더를 시작위치로 한 "path"에 해당하는 T 타입의 에셋을 불러오고 리턴
    public T Load<T>(string path) where T : Object
    {
        return Resources.Load<T>(path);
    }

    // Instantiate 사용자 지정 함수 정의
    public GameObject Instantiate(string path, Transform parent = null)
    {
        // Load를 사용해 prefab에 path에 해당하는 GameObject 타입의 에셋 할당
        GameObject prefab = Load<GameObject>($"Prefabs/{path}");

        if (prefab == null)
        {
            return null;
        }

        // Object.Instantiate라고 부른 이유는
        // 그냥 Instantiate 라고 명시하면 지금 정의하고 있는
        // 이 사용자 지정 함수 Instantiate 라고 인식되서 재귀호출 되므로.
        return Object.Instantiate(prefab, parent);
    }

    // Destory 사용자 지정 함수 정의
    public void Destory(GameObject go)
    {
        if (go == null)
        {
            return;
        }

        Object.Destroy(go);
    }
}