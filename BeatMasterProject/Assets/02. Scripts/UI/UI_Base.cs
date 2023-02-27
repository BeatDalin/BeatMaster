using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

public class UI_Base : MonoBehaviour
{
    // _objects : Key는 Type, Value는 오브젝트들이 담긴 배열인 Dictionary
    // 유니티 씬 상에 존재하는 오브젝트들을 로드하여 이 곳에 바인딩하여 보관
    // UI_Button에서 UI_Button 캔버스의 구성 오브젝트들의 이름을 Enum별로 분류해 정의
    // Enum Type을 Key로 하여 Enum에 담긴 이름들에 해당하는 실제 오브젝트들을 배열로 담아 Value로 할 것이다.
    // ex) Buttons가 Key라면 버튼 UI 오브젝트들이 담긴 배열이 Value
    // UI_Buttons는 UI_Base를 상속받는다. 즉, 캔버스 UI 프리팹들에 붙을 스크립트는 모두 UI_Base를 상속받게 할 것이므로
    // 캔버스 프리팹들 각각 본인들의 _objects에서 자신들을 구성하는 오브젝트들을 바인딩하여 담게 된다.
    // UnityEngine.Object는 모든 오브젝트와 컴포넌트들의 조상이라 UnityEngine.Object로 모든 오브젝트, 컴포넌트들을 업캐스팅 하는게 가능하다.
    
    protected Dictionary<Type, UnityEngine.Object[]> _objects = new Dictionary<Type, UnityEngine.Object[]>();

    //public abstract void Init();

    // Bind() : UI_Base를 상속받는 모든 캔버스 UI 
    protected void Bind<T>(Type type) where T : UnityEngine.Object
    {
        string[] names = Enum.GetNames(type);
        UnityEngine.Object[] objects = new UnityEngine.Object[name.Length];
        _objects.Add(typeof(T), objects);   // Dictionary에 추가
        
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
        
        //Unity
    }



}
