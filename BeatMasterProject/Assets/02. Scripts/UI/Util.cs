using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Util
{

    // 컴포넌트 없는 GameObject 오브젝트만(go) 넘겨 받을 예정인 public static GameObject FindChild
    // static이라 외부에서 Util.FindChild로 호출 가능
    // 컴포넌트 없는 빈 오브젝트만 이 함수에서 넘길 것이라서 Generic으로 구현할 필요 X
    // 빈 오브젝트를 포함한 오브젝트는 Transform 컴포넌트를 가지므로 이 Transform 컴포넌트를 가진 오브젝트를 찾아오도록 FindChild<Transform>을 호출한다.
    public static GameObject FindChild(GameObject go, string name = null, bool recursive = false)
    {
        Transform transform = FindChild<Transform>(go, name, recursive);
        
        if (transform == null)
        {
            return null;
        }
        return transform.gameObject;
    }
    
    // go : 오브젝트의 모든 자식들 중 T Component를 가지며, name과 이름이 일치하는 오브젝트를 찾아 return한다.
    public static T FindChild<T>(GameObject go, string name = null, bool recursive = false) where T : UnityEngine.Object
    {
        if (go == null)
        {
            return null;
        }

        // recursive를 false로 받았다면 go 의 직속 자식들 중에서만 T Component를 가진 자식 찾음
        // GetChild(int) 함수를 통해 직속 자식을 Transform 타입으로 리턴받을 수 있음.
        // 몇 번째 자식인지 index를 파라미터로 넘김
        if (recursive == false)
        {
            for (int i = 0; i < go.transform.childCount; i++)
            {
                Transform transform = go.transform.GetChild(i);

                // 만약 name == null이면(호출 시, 안넘겨줬다면) 그냥 바로 T Component에 해당되는 것만 찾으면 된다.
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
        // recursive를 true로 받았다면 go의 손자들까지 recursive하게 모든 자식들 중에서 T Component를 가진 자식을 찾음
        // GetComponentsInChildren함수를 통해 T Component를 가진 모든 자식들 검사!
        else
        {
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
}
