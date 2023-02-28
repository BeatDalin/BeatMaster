using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Util : MonoBehaviour
{
    #region T GetorAddComponent<T>

    // GetOrAddComponent : go 오브젝트의 T 컴포넌트를 GetComponent한다.
    //  T 컴포넌트가 없다면 AddComponent 해준 후 가져온다.

    public static T GetOrAddComponent<T>(GameObject go) where T : UnityEngine.Component
    {
        T component = go.GetComponent<T>();

        if (component == null)
        {
            component = go.AddComponent<T>();
        }

        return component;
    }

    #endregion


    #region GameObject FindChild

    // 컴포넌트 없는 GameObject만 넘겨 받음
    // static이라 외부에서 그냥 Util.FindChild 호출 가능!
    // 빈 오브젝트를 포함한 모든 오브젝트는 Transform 컴포넌트를 가지므로
    // Transform 컴포넌트를 가진 오브젝트를 찾아오도록 FindChild<Transform>를 호출
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);

        if (transform == null)
        {
            return null;
        }

        return transform.gameObject;
    }

    #endregion


    #region T FindChild<T>

    // FindChild : go 오브젝트의 모든 자식들 중 T 컴포넌트를 가지며, name과 일치하는 오브젝트를 찾아 리턴

    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
        {
            return null;
        }

        // go의 직속 자식들만 T 컴포넌트를 가진 자식 찾음
        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);

                // name이 null이면(호출 시 안넘겨줬다면) 바로 그냥 T 컴포넌트에 해당 되는 것만 찾으면 된다.
                if (string.IsNullOrEmpty(name) || transform.name == name)
                {
                    T component = transform.GetComponent<T>();

                    if (component != null)
                    {
                        return component;
                    }
                }
            }
        }

        // recursive == true일 때, go의 후손들까지 recursive하게 모든 자식들 중에서 T 컴포넌트를 가진 자식 찾음
        else
        {
            // T 컴포넌트를 가진 모든 자식들 검사
            foreach (T component in go.GetComponentsInChildren<T>())
            {
                if (string.IsNullOrEmpty(name) || component.name == name)
                {
                    return component;
                }
            }
        }

        return null;
    }

    #endregion
}