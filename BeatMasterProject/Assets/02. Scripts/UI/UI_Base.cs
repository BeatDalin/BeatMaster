using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Object = UnityEngine.Object;

/// UI_Base : 모든 UI의 조상. 모든 UI들이 가지고 있는 공통적인 부분들
public abstract class UI_Base : MonoBehaviour
{
    // _objects : Key => Type, Value => 오브젝트들이 담긴 배열(Dictionary)  
    Dictionary<Type, Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();
    
    #region Bind

    // Bind : UI 오브젝트 이름으로 찾아 바인딩해주기
    protected void Bind<T>(Type type) where T : Object
    {
        string[] names = Enum.GetNames(type);

        // objects : _objects Dictionary에 Value로 담기 위한 배열
        Object[] objects = new Object[name.Length];
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
    // T 컴포넌트를 가지고 있으며(혹은 오브젝트) 파라미터로 넘긴 int(ex, Images.ItemIcon 을 int로 형변환하면 enum답게 정수가 리턴
    // 인덱스에 해당하는 오브젝트를 T 타입으로 리턴함
    
    protected T Get<T>(int idx) where T : Object
    {
        Object[] objects = null;

        // _objects Dictionary에 typeof(T) Key가 존재하면 true리턴 + objects 배열에 typeof(T) Key의 Value를 저장
        if (_objects.TryGetValue(typeof(T), out objects) == false)
        {
            return null;
        }

        return objects[idx] as T;
    }

    #endregion

    #region Get을 통해 오브젝트 가져오기


    protected Text GetText(int idx)
    {
        return Get<Text>(idx);
    }

    protected Button GetButton(int idx)
    {
        return Get<Button>(idx);
    }

    protected Image GetImage(int idx)
    {
        return Get<Image>(idx);
    }

    protected Toggle GetToggle(int idx)
    {
        return Get<Toggle>(idx);
    }

    protected GameObject GetObject(int idx)
    {
        return Get<GameObject>(idx);
    }
    

    #endregion
    
    
    
    #region AddUIEvent

    // AddUIEvent : UI 오브젝트에 이벤트 등록하기
    // go 오브젝트에 UI_EventHandler를 붙여 go 오브젝트가 이벤트 콜백을 받을 수 있게 함
    // UI_EventHandler에 정의되어 있는 이벤트들이 발생하면, action에 등록된 것들이 실행되도록 한다. (OnClickHandler, OnDragHandler)
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
    
    public static void RemoveUIEvent(GameObject go, Action<PointerEventData> action,
        Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_EventHandler evt = Util.GetOrAddComponent<UI_EventHandler>(go);

        switch (type)
        {
            case Define.UIEvent.Click:
                evt.OnClickHandler -= action;
                break;

            case Define.UIEvent.Drag:
                evt.OnDragHandler -= action;
                break;
        }
    }

    #endregion
    
}