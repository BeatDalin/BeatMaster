using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMenuManager : MonoBehaviour
{
    private Vector3 _mouseDownPos;
    
    //cam variables..
    [SerializeField] private float _rotateSpeed = 500.0f;
    [SerializeField] private float _zoomSpeed = 10.0f;
    [SerializeField] private float _scrollSpeed = 2000.0f;
    [SerializeField] private float _dragSpeed = 30.0f;
    [SerializeField] private Vector3 _camPosOrigin;
    [SerializeField] private float _fovOrigin = 30f;
    [SerializeField] private GameObject cam;
    
    private Camera mainCamera;
    
    private Vector2 _clickPoint;

    void Start()
    {
        _camPosOrigin = new Vector3(2.7f, 7.47f, -10.23f);
        mainCamera = cam.GetComponent<Camera>();
        mainCamera.transform.position = _camPosOrigin;
        mainCamera.fieldOfView = _fovOrigin;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _mouseDownPos = Input.mousePosition;
            Ray ray = Camera.main.ScreenPointToRay(_mouseDownPos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log(hit.collider.name);
                //Load GameScene
            }

            else
            {
                Zoom();
                MoveCamToMousePosition();
            }
        }
    }

    void Zoom()
    {
        //왼쪽 AltKey 누르면서 마우스 스크롤하면 zoom In, out
        if (Input.GetKey(KeyCode.LeftAlt))
        {
            float distance = Input.GetAxis("Mouse ScrollWheel") * -1 * _zoomSpeed;
            if (distance != 0)
            {
                mainCamera.fieldOfView += distance;
            }
        }
    }

    void MoveCamToMousePosition()
    {
        _clickPoint = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        Vector3 _goalPos = Camera.main.ScreenToViewportPoint((Vector2)Input.mousePosition - _clickPoint);

        _goalPos.z = _goalPos.y;
        _goalPos.y = .0f;
            
        //먼 move임..? 나중에 변수명 수정해야할듯
        Vector3 move = _goalPos * (Time.deltaTime * _dragSpeed);

        float y = cam.transform.position.y;
            
        cam.transform.Translate(move);
        cam.transform.position = new Vector3(cam.transform.position.x, y, cam.transform.position.z);
    }

}
