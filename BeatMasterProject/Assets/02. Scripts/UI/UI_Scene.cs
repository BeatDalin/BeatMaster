using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Scene : UI_Base
{
    
    // UI 캔버스 세팅(오브젝트에 Canvas 컴포넌트 sorting order 세팅)
    public override void Init()
    {
        // UIManager의 SetCanvas 호출
        // 고정 UI니까 sorting할 필요가 없으므로 false 전달
        UIManager.instance.SetCanvas(gameObject,false);
    }
}
