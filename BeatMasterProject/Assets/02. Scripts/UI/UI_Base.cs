using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

public class UI_Base : MonoBehaviour
{
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        
        UnityEngine.Object[] objects = new UnityEngine.Object[name.Length];
        _objects.Add(typeof(T), objects); // Dictionary에 추가

        for (int i = 0; i < names.Length; i++)
        {
            if (typeof(T) == typeof(GameObject))
            {
                objects[i] = Util.FindChild(gameObject, names[i], true);
            }
            else
            {
                objects[i] = Util.FindChild<T>(gameObject, names[i], true);
            }
        }
    }

    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;

        if (_objects.TryGetValue(typeof(T), out objects) == false)
        {
            return null;
        }

        return objects[idx] as T;
    }
    
    // 오브젝트 가져오기
    protected GameObject GetObject(int idx)
    {
        return Get<GameObject>(idx);
    } 

    // Text 가져오기
    protected Text GetText(int idx)
    {
        return Get<Text>(idx);
    }

    // Button 가져오기
    protected Button GetButton(int idx)
    {
        return Get<Button>(idx);
    } 
    
    // Image 가져오기
    protected Image GetImage(int idx)
    {
        return Get<Image>(idx);
    }

    public static void AddUIEvent(
        GameObject go,
        Action<PointerEventData> action,
        Define.UIEvent type = Define.UIEvent.Click)
    {
        
    }
}