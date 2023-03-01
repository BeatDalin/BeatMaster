using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Popup : UI_Base
{
    // UI Canvas 세팅(오브젝트에 Canvas 컴포넌트 sorting order 세팅)
    public override void Init()
    {
        // UI Manager의 SetCanvas() 호출(팝업 UI니까 캔버스들을 Sorting해야함)
        UIManager.instance.SetCanvas(gameObject, true);
    }

    // UI 닫기(캔버스 프리팹 파괴)
    // 팝업이니까 고정 캔버스(Scene)와 다르게 닫는게 필요함
    public virtual void ClosePopupUI()
    {
        // UI Manager의 ClosePopupUI() 호출
        UIManager.instance.ClosePopupUI(this);
    }
}