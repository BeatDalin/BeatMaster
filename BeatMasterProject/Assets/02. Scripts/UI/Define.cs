using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 이벤트도 종류별로 묶어서 정리
public class Define : MonoBehaviour
{
    public enum UIEvent
    {
        Click,
        Drag,
    }

    public enum MouseEvent
    {
        Press,
        Click
    }

    public enum CameraMode
    {
        Quaterview
    }
}
