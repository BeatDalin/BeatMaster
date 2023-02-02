using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using SonicBloom.Koreo;

public class CameraController : MonoBehaviour
{
    private CinemachineBrain _brain;
    private CinemachineVirtualCamera _virtualCamera;

    [SerializeField] private Vector3 _offset = new Vector3(0, 3, -10);
    [SerializeField] private float _minOrthoSize = 7f;
    [SerializeField] private float _maxOrthoSize = 10f;
    [SerializeField] [EventID] private string _speedEventID;
    private float _prevCharacterSpeed;

    private void Start()
    {
        _brain = GetComponent<CinemachineBrain>();
        _virtualCamera = _brain.ActiveVirtualCamera.VirtualCameraGameObject.GetComponent<CinemachineVirtualCamera>();
        _virtualCamera.transform.position = _virtualCamera.Follow.position + _offset;
        _virtualCamera.m_Lens.OrthographicSize = _minOrthoSize;
        //_prevCharacterSpeed = _virtualCamera.Follow.GetComponent<CharacterMovement>().moveSpeed;

        Koreographer.Instance.RegisterForEvents(_speedEventID, ChangeOrthoSize);
    }

    private void ChangeOrthoSize(KoreographyEvent evt)
    {
        if (evt.StartSample == 0)
        {
            _prevCharacterSpeed = evt.GetFloatValue();

            return;
        }

        float newCharacterSpeed = evt.GetFloatValue();

        if (_prevCharacterSpeed < newCharacterSpeed)
        {
            StartCoroutine(CoChangeOrthoSize(_minOrthoSize, _maxOrthoSize));
        }
        else if (_prevCharacterSpeed > newCharacterSpeed)
        {
            StartCoroutine(CoChangeOrthoSize(_maxOrthoSize, _minOrthoSize));
        }

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
