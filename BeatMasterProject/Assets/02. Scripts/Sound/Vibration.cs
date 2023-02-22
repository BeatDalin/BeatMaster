using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vibration : MonoBehaviour
{
#if UNITY_ANDROID && !UNITY_EDITOR
    public static AndroidJavaClass AndroidPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
    public static AndroidJavaObject AndroidcurrentActivity = AndroidPlayer.GetStatic<AndroidJavaObject>("currentActivity");
    public static AndroidJavaObject AndroidVibrator = AndroidcurrentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
#endif
    public static void Vibrate()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate");
#else
        Handheld.Vibrate();
#endif
    }
 
    public static void Vibrate(long milliseconds)
    {
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate", milliseconds);
#else
        Handheld.Vibrate();
#endif
    }
    public static void Vibrate(long[] pattern, int repeat)
    {
 
 
#if UNITY_ANDROID && !UNITY_EDITOR
        AndroidVibrator.Call("vibrate", pattern, repeat);
#else
        Handheld.Vibrate();
#endif
    }
 
    public static void Cancel()
    {
#if UNITY_ANDROID && !UNITY_EDITOR
            AndroidVibrator.Call("cancel");
#endif
    }
    
    public void OnClick1()
    {
        Vibration.Vibrate((long)5000);
    }
 
    public void OnClick2()
    {
        long[] pattern = new long[4];
        pattern[0] = 1000;
        pattern[1] = 200;
        pattern[2] = 1000;
        pattern[3] = 200;
 
 
        Vibration.Vibrate(pattern, 1);
    }
 
    public void OnClick3()
    {
        long[] pattern = new long[4];
        pattern[0] = 1000;
        pattern[1] = 5000;
        pattern[2] = 2000;
        pattern[3] = 1000;
 
 
        Vibration.Vibrate(pattern, -1);
    }
 
    public void OnClick4()
    {
        Vibration.Cancel();
    }

}
