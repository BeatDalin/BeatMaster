using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibration
{
#if UNITY_ANDROID && UNITY_EDITOR
    private static AndroidJavaObject vibrator;
    
    private void Start() {
        // 네이티브 클래스 로드
        using (var unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer")) {
            using (var activity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity")) {
                vibrator = new AndroidJavaObject("com.example.vibtest", activity);
            }
        }
    }

    public void Vibrate(int duration) {
        // 네이티브 함수 호출
        vibrator.Call("vibrate", duration);
    }
#endif
}