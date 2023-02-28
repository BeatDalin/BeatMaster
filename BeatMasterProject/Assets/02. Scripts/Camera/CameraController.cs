using System.Collections;
using UnityEngine;
using Cinemachine;
using SonicBloom.Koreo;

public class CameraController : MonoBehaviour
{
    private CharacterMovement _characterMovement;
    private CinemachineVirtualCamera[] _virtualCameras;
    private ResourcesChanger _resourcesChanger;

    public float FromOrthoSize { get => _fromOrthoSize; private set => _fromOrthoSize = value; }
    public float ToOrthoSize { get => _toOrthoSize; private set => _toOrthoSize = value; }
    
    [SerializeField] private Vector3 _offset = new Vector3(0f, 2f, -10f);
    [SerializeField] [EventID] private string _speedEventID;
    [SerializeField] private float _minOrthoSize = 4.1f;
    [SerializeField] private float _maxOrthoSize = 5.7f;
    [SerializeField] private AnimationCurve _lerpCurve;
    private float _fromOrthoSize;
    private float _toOrthoSize;
    private float _prevCharacterSpeed;

    private void Start()
    {
        _characterMovement = FindObjectOfType<CharacterMovement>();
        _resourcesChanger = FindObjectOfType<ResourcesChanger>();
        _virtualCameras = FindObjectsOfType<CinemachineVirtualCamera>();
        foreach (var virtualCamera in _virtualCameras)
        {
            virtualCamera.transform.position = virtualCamera.Follow.position + _offset;
            virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(_minOrthoSize, _maxOrthoSize, (_characterMovement.MoveSpeed - 1.5f) / 2f);
        }
        _prevCharacterSpeed = _characterMovement.MoveSpeed;

        Koreographer.Instance.RegisterForEvents(_speedEventID, ChangeOrthoSize);
    }

    private void ChangeOrthoSize(KoreographyEvent evt)
    {
        if (!evt.HasFloatPayload())
        {
            return;
        }

        float newCharacterSpeed = evt.GetFloatValue();

        if (_prevCharacterSpeed == newCharacterSpeed)
        {
            return;
        }
        
        _fromOrthoSize = Mathf.Lerp(_minOrthoSize, _maxOrthoSize, (_prevCharacterSpeed - 1.5f) / 2f);
        _toOrthoSize = Mathf.Lerp(_minOrthoSize, _maxOrthoSize, (newCharacterSpeed - 1.5f) / 2f);
        
        StartCoroutine(CoChangeOrthoSize(_fromOrthoSize, _toOrthoSize));
        _resourcesChanger.OnSpeedChanged(newCharacterSpeed);
        _prevCharacterSpeed = newCharacterSpeed;
    }
    
    private IEnumerator CoChangeOrthoSize(float from, float to)
    {
        float time = 0f;

        while (time <= 0.7f)
        {
            foreach (var virtualCamera in _virtualCameras)
            {
                virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(from, to, _lerpCurve.Evaluate(time));
            }

            time += Time.deltaTime;
            
            yield return null;
        }

        foreach (var virtualCamera in _virtualCameras)
        {
            virtualCamera.m_Lens.OrthographicSize = to;
        }
    }
}