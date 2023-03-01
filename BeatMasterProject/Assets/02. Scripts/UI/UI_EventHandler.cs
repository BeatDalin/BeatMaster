using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System;

// IPointerClickHandler, IDragHandler와 같은 인터페이스를 상속 받아 오버리이딩 하면,
// 해당 이벤트가 실행될 때 자동으로 이벤트 함수가 실행된다.
public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler
{
    // PointerEventData eventDate에 온갖 해당 이벤트와 관련 정보가 자동으로 담긴다.

    public Action<PointerEventData> OnClickHandler = null;
    public Action<PointerEventData> OnDragHandler = null;

    // 클릭 이벤트 오버라이딩
    public void OnPointerClick(PointerEventData eventData)
    {
        if (OnClickHandler != null)
        {
            OnClickHandler.Invoke(eventData); // 클릭과 관련된 액션 실행
        }
    }

    // 드래그 이벤트 오버라이딩
    public void OnDrag(PointerEventData eventData)
    {
        if (OnDragHandler != null)
        {
            OnDragHandler.Invoke(eventData); // 드래그와 관련된 액션 실행
        }
    }
}