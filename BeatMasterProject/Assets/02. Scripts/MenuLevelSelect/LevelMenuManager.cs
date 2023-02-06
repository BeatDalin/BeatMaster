using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _waypointIndicator;
    [SerializeField] private GameObject _cam;
    [SerializeField] private GameObject _camPosOrigin;
    
    [SerializeField] private int _fovOrigin = 30;
    [SerializeField] private int _fovMax = 36;
    [SerializeField] private int _fovMin = 10;
    
    [SerializeField] private float _xPosMax = 6f;
    [SerializeField] private float _xPosMin = 2f;

    [SerializeField] private float _zPosMax = -6.5f;
    [SerializeField] private float _zPosMin = -15f;

    public Ray effectRayPoint;
    
    private Camera _mainCam;
    
    private Vector2 _touchPoint;

    private WaypointMover _waypointMover;
    
    private float _zoomSpeed = 10f;
    private float _dragSpeed = 10f;

    private void Start() 
    {
        // Init Main Camera's Pos, FOV
        _mainCam = _cam.GetComponent<Camera>();
        _mainCam.transform.position = _camPosOrigin.transform.position;
        _mainCam.fieldOfView = _fovOrigin;

        _waypointMover = FindObjectOfType<WaypointMover>();
    }

    private void Update()
    {
        SelectLevel();
        Zoom();
        MoveCamByDrag();
    }

    /// <summary>
    /// LvFlat 오브젝트 터치 시 GameScene으로 이동
    /// </summary>
    private void SelectLevel()
    {
    #if UNITY_ANDROID

        if(Input.touchCount == 1)
        {
            _touchPoint = new Vector2(Input.GetTouch(0).position.x, Input.GetTouch(0).position.y);
            
            Ray ray = _mainCam.ScreenPointToRay(_touchPoint);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                //Load GameScene
            }
        }

    #else
        if (Input.GetMouseButtonDown(0))
        {
            _touchPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);

            Ray ray = _mainCam.ScreenPointToRay(_touchPoint);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                // Load GameScene
                //SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.LevelGame);
                // 추후 레벨 별로 구분해야 함

                if (hit.collider.name.Equals("BossGame"))
                {
                    _waypointMover.UpdateWaypointPosition();
                }
                else
                {
                    SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.Level1);
                }
            }
        }

        if (Input.GetMouseButton(0))
        {
            effectRayPoint = _mainCam.ScreenPointToRay(Input.mousePosition);
        }

#endif
    }

    /// <summary>
    /// Zoom In, Out
    /// 모바일 핀치, StandAlone Mouse Wheel
    /// </summary>
    private void Zoom()
    {
    #if UNITY_ANDROID

        if(Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0); // 첫 번째 손가락 좌표
            Touch touchOne = Input.GetTOuch(1); // 두 번째 손가락 좌표

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevTouchDeltaMag = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float touchDeltaMag = (touchZero.position - touchOne.position).magnitude;

            float distance = (prevTouchDeltaMag - touchDeltaMag) * _zoomSpeed;
            if (distance != 0)
            {
                _mainCam.fieldOfView += distance;
                _mainCam.fieldOfView = Mathf.Clamp(_mainCam.fieldOfView, _fovMin, _fovMax); // 줌 최대, 최소 범위
            }
        }
    }

    #else
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            float distance = Input.GetAxis("Mouse ScrollWheel") * -1 * _zoomSpeed;
            if (distance != 0)
            {
                var fieldOfView = _mainCam.fieldOfView;
                fieldOfView += distance;
                //_mainCam.fieldOfView = fieldOfView;
                _mainCam.fieldOfView = Mathf.Clamp(fieldOfView, _fovMin, _fovMax); // 줌 최대, 최소 범위
            }
        }
    }
    #endif

    private void MoveCamByDrag()
    {
#if UNITY_ANDROID

        if(Input.GetTouch(0).phase == TouchPhase.Moved) // 바른 조건인지 확실치 않음! 테스트 필
        {
            Vector3 goalPos = _mainCam.ScreenToViewportPoint((Vector2)Input.GetTouch(0).position - _touchPoint);

            goalPos.z = goalPos.y;
            goalPos.y = .0f;

            Vector3 camMove = goalPos * (Time.deltaTime * _dragSpeed);

            float y = cam.transform.position.y;

            cam.transform.Translate(camMove);

            // y축 값 고정
            Vector3 position = cam.transform.position;
            position = new Vector3(position.x, y, position.z);
            cam.transform.position = ClampPoisition(position);
        }

#else
        if (Input.GetMouseButton(0))
        {
            Vector3 goalPos = _mainCam.ScreenToViewportPoint((Vector2)Input.mousePosition - _touchPoint);

            goalPos.z = goalPos.y;
            goalPos.y = .0f;

            Vector3 camMove = goalPos * (Time.deltaTime * _dragSpeed);

            float y = _cam.transform.position.y;

            _cam.transform.Translate(camMove);

            // y축 값 고정
            Vector3 position = _cam.transform.position;
            position = new Vector3(position.x, y, position.z);
            _cam.transform.position = ClampPoisition(position);
        }
        
#endif
    }

    /// <summary>
    /// Vector3 값의 최댓, 최솟값을 제한하는 함수
    /// </summary>
    /// <param name="position"></param> 제한할 Vector3
    /// <returns></returns> 제한된 Vector3
    private Vector3 ClampPoisition(Vector3 position)
    {
        return new Vector3(
            Mathf.Clamp(position.x, _xPosMin, _xPosMax), 
            position.y, 
            Mathf.Clamp(position.z, _zPosMin, _zPosMax));
    }
}
