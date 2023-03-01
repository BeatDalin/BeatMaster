using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;
using UnityEngine.UI;

/// UI_Base : 모든 UI의 조상. 모든 UI들이 가지고 있는 공통적인 부분들
public abstract class UI_Base : MonoBehaviour
{
    // _objects : Key => Type, Value => 오브젝트들이 담긴 배열(Dictionary)  
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    public abstract void Init();

    #region Bind

    // Bind : UI 오브젝트 이름으로 찾아 바인딩해주기
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);

        // objects : _objects Dictionary에 Value로 담기 위한 배열
        UnityEngine.Object[] objects = new UnityEngine.Object[name.Length];
        _objects.Add(typeof(T), objects); // Dictionary에 추가


        // T에 속하는 오브젝트들을 Dictionary의 Value인 objects 배열의 원소들에 하나씩 추가
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

    #endregion


    #region Get

    // Get : : UI 오브젝트 가져오기
    protected T Get<T>(int idx) where T : UnityEngine.Object
    {
        UnityEngine.Object[] objects = null;

        if (_objects.TryGetValue(typeof(T), out objects) == false)
        {
            return null;
        }

        return objects[idx] as T;
    }

    #endregion


    #region AddUIEvent

    // AddUIEvent : UI 오브젝트에 이벤트 등록하기
    // go 오브젝트에 UI_EventHandler를 붙여 go 오브젝트가 이벤트 콜백을 받을 수 있게 함
    // UI_EventHandler에 정의되어 있는 이벤트들이 발생하면, action에 등록된 것들이 실행되도록 한다.
    public static void AddUIEvent(GameObject go, Action<PointerEventData> action,
        Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            // 클릭 이벤트일 때
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                evt.OnClickHandler += action; // 액션 실행
                break;

            // 드래그 이벤트일 때
            case Define.UIEvent.Drag:
                evt.OnDragHandler -= action;
                evt.OnDragHandler += action; // 액션 실행
                break;
        }
    }

    #endregion


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
}