using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _cam;
    
    public Ray effectRayPoint;
    
    private Camera _mainCam;
    
    private void Start() 
    {
        _mainCam = _cam.GetComponent<Camera>();
    }
    
    private void Update()
    {
        if (Input.GetMouseButton(0))
        {
            effectRayPoint = _mainCam.ScreenPointToRay(Input.mousePosition);
        }
    }
}
