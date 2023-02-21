using System.Collections;
using UnityEngine;
using Cinemachine;
using SonicBloom.Koreo;

public class CameraController : MonoBehaviour
{
    private CharacterMovement _characterMovement;
    private CinemachineVirtualCamera _virtualCamera;
    private ResourcesChanger _resourcesChanger;

    public float FromOrthoSize { get => _fromOrthoSize; private set => _fromOrthoSize = value; }
    public float ToOrthoSize { get => _toOrthoSize; private set => _toOrthoSize = value; }
    
    [SerializeField] private Vector3 _offset = new Vector3(0f, 2f, -10f);
    [SerializeField] [EventID] private string _speedEventID;
    private float _fromOrthoSize;
    private float _toOrthoSize;
    private float _prevCharacterSpeed;

    private void Start()
    {
        _characterMovement = FindObjectOfType<CharacterMovement>();
        _virtualCamera = transform.GetComponent<CinemachineVirtualCamera>();
        _resourcesChanger = FindObjectOfType<ResourcesChanger>();
        _virtualCamera.transform.position = _virtualCamera.Follow.position + _offset;
        _virtualCamera.m_Lens.OrthographicSize = Mathf.Lerp(3f, 7f, (_characterMovement.MoveSpeed - 1.5f) / 2f);
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
        
        _fromOrthoSize = Mathf.Lerp(3f, 7f, (_prevCharacterSpeed - 1.5f) / 2f);
        _toOrthoSize = Mathf.Lerp(3f, 7f, (newCharacterSpeed - 1.5f) / 2f);
        
        StartCoroutine(CoChangeOrthoSize(_fromOrthoSize, _toOrthoSize));
        _resourcesChanger.OnSpeedChanged(newCharacterSpeed);
        _prevCharacterSpeed = newCharacterSpeed;
    }
    
    private IEnumerator CoChangeOrthoSize(float from, float to)
    {
        float time = 0f;
        
        while (time <= 1f)
        {
            _virtualCamera.m_Lens.OrthographicSize = Mathf.SmoothStep(from, to, time);
            time += Time.deltaTime;
            
            yield return null;
        }
        
        _virtualCamera.m_Lens.OrthographicSize = to;
    }
}