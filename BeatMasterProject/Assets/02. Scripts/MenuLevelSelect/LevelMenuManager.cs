using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject _cam;
    
    public Ray effectRayPoint;
    
    private Camera _mainCam;
    
    [SerializeField] private GameObject _settingPopUp;
    [SerializeField] private GameObject _levelPanel;

    private void Start() 
    {
        _mainCam = _cam.GetComponent<Camera>();
        StartCoroutine("CoZoomInCam");
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

    IEnumerator CoZoomInCam()
    {
        yield return new WaitForSeconds(3f);

        for (int i = 0; i < 200; i++)
        {
            if (_mainCam.orthographicSize > 5)
            {
                _mainCam.orthographicSize -= 0.08f;
            }
            
            yield return new WaitForSeconds(0.01f);
            if (_mainCam.orthographicSize <= 5)
            {
                yield return new WaitForSeconds(1f);
                _levelPanel.SetActive(true);
            }
        }
    }
}
