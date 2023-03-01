using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

// 클래스와 메서드들이 static인 것에 주목 => 확장 메서드
public static class Extension
{
    // 확장 메서드
    // 매개변수가 없는 함수이다.
    // GameObject 파라미터에서 호출할 수 있게 되었다.
    // 마치 GameObject의 메서드인 것처럼 사용할 수 있게 됨!
    public static T GetOrAddComponent<T>(this GameObject go) where T : UnityEngine.Component
    {
        return Util.GetOrAddComponent<T>(go);
    }

    // 확장 메서드
    // 매개변수 action, Define
    // GameObject 파라미터에서 호출할 수 있게 되었다.
    // 마치 GameObject의 메서드인 것처럼 사용할 수 있게 됨!
    public static void AddUIEvent(this GameObject go, Action<PointerEventData> action,
        Define.UIEvent type = Define.UIEvent.Click)
    {
        UI_Base.AddUIEvent(go, action, type);
    }
}