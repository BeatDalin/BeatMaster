using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

public class ResourcesChanger : MonoBehaviour
{
    public float DefaultSpeed { get; private set; }
    [SerializeField] private ChangingResources[] _changingResources;
    [SerializeField] private AnimationCurve _hueCurve;
    [SerializeField] private float _lerpTime = 1f;
    private BackgroundMover _backgroundMover;
    private Volume _volume;
    private ColorAdjustments _colorAdjustments;
    private int _materialIndex;
    private string _sceneName;
    private ChangingResources _currentResource;

    private int _hueIndex;
    private int _satIndex;
    private readonly int MAX_HUE = 360;
    private readonly int MAX_SAT = 100;
    
    
    private void Awake()
    {
        _backgroundMover = FindObjectOfType<BackgroundMover>();
        _volume = FindObjectOfType<Volume>();
        _volume.profile.TryGet(typeof(ColorAdjustments), out _colorAdjustments);
        Init();
    }

    private void Init()
    {
        // TODO Stage()_Level()로 바꾸기
        _sceneName = SceneLoadManager.Instance.Scene.ToString();
    }

    public void OnSpeedChanged(float speed)
    {
        ChangePostProcessing(speed);
        _backgroundMover.SetBackgroundSize(speed);
    }
    
    private void ChangePostProcessing(float speed)
    {
        foreach (var changingResource in _changingResources)
        {
            if (changingResource.name == _sceneName)
            {
                _currentResource = changingResource;
                break;
            }
        }

        StartCoroutine(CoHueShift(speed));
    }

    private IEnumerator CoHueShift(float speed)
    {
        float timer = 0f;
        float h, s, v;
        
        Color.RGBToHSV(_colorAdjustments.colorFilter.value, out h, out s, out v);

        if (DefaultSpeed.Equals(speed))
        {
            while (_lerpTime > timer)
            {
                timer += Time.deltaTime;
                timer = _lerpTime < timer ? _lerpTime : timer; 
                float ratio = timer / _lerpTime;
                
                // 여기선 그냥 값이
                s = Mathf.Lerp(s, 0, _hueCurve.Evaluate(ratio));
                _colorAdjustments.colorFilter.value = Color.HSVToRGB(h, s, v);
                yield return new WaitForEndOfFrame();
            }
        }
        else
        {
            while (_lerpTime > timer)
            {
                timer += Time.deltaTime;
                timer = _lerpTime < timer ? _lerpTime : timer; 
                float ratio = timer / _lerpTime;
                
                // changingResources의 값은 ratio 값이 아닌 일반 HSV 값이므로 최대 값으로 나누어 주어야 된다.
                // 360이 최대 값
                h = Mathf.Lerp(h * MAX_HUE, _currentResource.HueValues[_hueIndex],
                        _hueCurve.Evaluate(ratio)) /
                    MAX_HUE;
                // 100이 최대 값
                s = Mathf.Lerp(s * MAX_SAT, _currentResource.SatValues[_satIndex], _hueCurve.Evaluate(ratio)) /
                    MAX_SAT;
                    _colorAdjustments.colorFilter.value = Color.HSVToRGB(h, s, v);
                yield return new WaitForEndOfFrame();
            }    
        }
        
        _hueIndex++;
        _satIndex++;
    }

    public void SetDefaultSpeed(float moveSpeed)
    {
        DefaultSpeed = moveSpeed;
    }

    public void ResetPostProcessing()
    {
        _hueIndex = _satIndex = 0;
    }
}
