using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _cam;
    
    public Ray effectRayPoint;
    
    private Camera _mainCam;
    
    [SerializeField] private GameObject _settingPopUp;

    private void Start() 
    {
        _mainCam = _cam.GetComponent<Camera>();
    }
    
    private void Update()
    {
        // mouse Point effect
        if (Input.GetMouseButton(0))
        {
            effectRayPoint = _mainCam.ScreenPointToRay(Input.mousePosition);
        }

        // Setting PopUp
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SoundManager.instance.PlaySFX("Touch");
            if (!_settingPopUp.activeSelf)
            {
                UIManager.instance.OpenPopUp(_settingPopUp);
            }
            else
            {
                UIManager.instance.ClosePopUp();
            }
        }
    }
}
