using System;
using UnityEngine;

public class ExcuteVibration : MonoBehaviour
{
    static ExcuteVibration instance;

    public static AndroidJavaClass unityPlayer;
    public static AndroidJavaObject vibrator;
    public static AndroidJavaObject currentActivity;
    public static AndroidJavaClass vibrationEffectClass;
    public static AndroidJavaObject context;
    public static int defaultAmplitude;

    private AndroidJavaObject vibrationLibrary;

    public int vibrationPower = 50;

    public static ExcuteVibration Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject obj = new GameObject("VibrationManager");
                instance = obj.AddComponent<ExcuteVibration>();
            }

            return instance;
        }
    }

//#if UNITY_ANDROID && !UNITY_EDITOR
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Destroy(gameObject);
        }

        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");

        if (getSDKInt() >= 26)
        {
            vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
            defaultAmplitude = vibrationEffectClass.GetStatic<int>("DEFAULT_AMPLITUDE");
        }
    }

    private void Start()
    {
        vibrationLibrary = new AndroidJavaObject("com.example.vibrationlibrary.test_String");
        context = currentActivity.Call<AndroidJavaObject>("getApplicationContext");
    }

    public void Perfect()
    {
        if (vibrationPower != 0)
        {
            int perfectPower = Mathf.Clamp(vibrationPower, 20, 40);
            Debug.Log($"perfectPower {perfectPower}");
            vibrationLibrary.CallStatic("VibratePerfect", context, perfectPower);
        }
    }

    public void Fail()
    {
        if (vibrationPower != 0)
        {
            int failPower = Mathf.Clamp(vibrationPower * 2, 40, 100);
            Debug.Log($"failPower {failPower}");
            vibrationLibrary.CallStatic("VibrateFail", context, failPower);
        }
    }

    public void FastOrSlow()
    {
        if (vibrationPower != 0)
        {
            int fastOrSlowPower = Mathf.Clamp(vibrationPower, 20, 40);
            Debug.Log($"fastOrSlow {fastOrSlowPower}");
            vibrationLibrary.CallStatic("VibrationFast", context, fastOrSlowPower);
        }
    }

    public void Touch()
    {
        if (vibrationPower != 0)
        {
            int touchPower = Math.Clamp(vibrationPower / 4, 5, 100);
            Debug.Log($"touchPower {touchPower}");
            vibrationLibrary.CallStatic("TouchVibrate", context, touchPower);
        }
    }

    static int getSDKInt()
    {
        using (var version = new AndroidJavaClass("android.os.Build$VERSION"))
        {
            return version.GetStatic<int>("SDK_INT");
        }
    }

//#endif
}