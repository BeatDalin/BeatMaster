using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ResourcesChanger : MonoBehaviour
{
    private float _defaultSpeed;
    [SerializeField] private ChangingResources[] _changingResources;
    [SerializeField] private AnimationCurve _hueCurve;
    [SerializeField] private float _lerpTime = 1f;
    private BackgroundController _backgroundController;
    private Volume _volume;
    private ColorAdjustments _colorAdjustments;
    private int _materialIndex;
    private string _sceneName;
    private ChangingResources _currentResource;
    private CharacterMovement _characterMovement;

    private int _hueIndex;
    private int _satIndex;
    private readonly int MAX_HUE = 360;
    private readonly int MAX_SAT = 100;
    
    
    private void Awake()
    {
        _backgroundController = FindObjectOfType<BackgroundController>();
        _volume = FindObjectOfType<Volume>();
        _volume.profile.TryGet(typeof(ColorAdjustments), out _colorAdjustments);
        _characterMovement = FindObjectOfType<CharacterMovement>();
        Init();
    }

    private void Init()
    {
        // TODO Stage()_Level()로 바꾸기
        _sceneName = SceneLoadManager.Instance.Scene.ToString();
        SetCurrentResource();
        _backgroundController.SetOffsetSize(_changingResources[0].Backgrounds[0]);
        SetDefaultSpeed();
    }

    private void SetCurrentResource()
    {
        foreach (var changingResource in _changingResources)
        {
            if (changingResource.name == _sceneName)
            {
                _currentResource = changingResource;
                break;
            }
        }

        ChangeBackground();
    }
    
    private void ChangeBackground()
    {
        _backgroundController.SetBackgroundSprite(_currentResource);
    }

    public void OnSpeedChanged(float speed)
    {
        ChangePostProcessing(speed);
        _backgroundController.SetBackgroundSize();
    }
    
    private void ChangePostProcessing(float speed)
    {
        StartCoroutine(CoHueShift(speed));
    }

    private IEnumerator CoHueShift(float speed)
    {
        float timer = 0f;
        float h, s, v;
        
        Color.RGBToHSV(_colorAdjustments.colorFilter.value, out h, out s, out v);

        if (_defaultSpeed.Equals(speed))
        {
            while (_lerpTime > timer)
            {
                timer += Time.deltaTime;
                timer = _lerpTime < timer ? _lerpTime : timer; 
                float ratio = timer / _lerpTime;
                
                // 여기선 그냥 값이
                s = Mathf.Lerp(s, 0, _hueCurve.Evaluate(ratio));
                _colorAdjustments.colorFilter.value = Color.HSVToRGB(h, s, v);
                yield return null;
            }
        }
        else
        {
            Debug.Log(_hueIndex);
            Debug.Log(_currentResource);
            Debug.Log(_currentResource.HueValues[_hueIndex]); 
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
                yield return null;
            }
            _hueIndex++;
            _satIndex++;
        }

    }

    public void SetDefaultSpeed()
    {
        _defaultSpeed = _characterMovement.MoveSpeed;
    }

    public void ResetPostProcessing()
    {
        _hueIndex = _satIndex = 0;
    }
}
