using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

enum ButtonName
{
    Stage = 0,
    Store,
    Setting,
    ShutDown
}

public class MenuTitleButton : MonoBehaviour
{
    [SerializeField] private DOTweenAnimation[] _doTweenAnimations;
    [SerializeField] private Button[] _buttons;
    
    [SerializeField] private GameObject _settingPopUp;
    [SerializeField] private GameObject _storePopUp;
    [SerializeField] private CanvasGroup _loadingPanelGroup;

    [SerializeField] private SimpleMusicPlayer _simpleMusicPlayer;

    private int _objectIdx = 0;

    private void Awake()
    {
        Koreographer.Instance.GetKoreographyAtIndex(0);
    }

    private void Start()
    {
        Koreographer.Instance.RegisterForEvents("MenuBGMTrack", ChangeScale);

        _buttons[(int)ButtonName.Stage].onClick.AddListener(() => SceneMoveBtn("Stage"));
        _buttons[(int)ButtonName.Store].onClick.AddListener(() => OpenPopUp("Store"));
        _buttons[(int)ButtonName.Setting].onClick.AddListener(() => OpenPopUp("Setting"));
        _buttons[(int)ButtonName.ShutDown].onClick.AddListener(() => OpenPopUp("ShutDown"));
    }

    private void Update()
    {
        if (_loadingPanelGroup.alpha == 0)
        {
            if (!_simpleMusicPlayer.IsPlaying)
            {
                _simpleMusicPlayer.Play();
            }
        }
    }

    private void OpenPopUp(string PopUpName)
    {
        SoundManager.instance.PlaySFX("Touch");

        switch (PopUpName)
        {
            case "Store":
                _storePopUp.SetActive(true);
                _settingPopUp.GetComponent<RectTransform>().localPosition = new Vector3(Screen.width, 0, 0);
                
                UIManager.instance.popUpStack.Push(_settingPopUp);
                break;
            
            case "Setting":
                _settingPopUp.SetActive(true);
                _settingPopUp.GetComponent<RectTransform>().localPosition = new Vector3(Screen.width, 0, 0);
                
                UIManager.instance.popUpStack.Push(_settingPopUp);
                break;
            
            case "ShutDown":
                Application.Quit();
                break;
        }
    }

    private void SceneMoveBtn(string SceneName)
    {
        SoundManager.instance.PlaySFX("Touch");
        
        //SceneManager.LoadScene(SceneName); 씬 로드(씬매니저 없어서 임시로 해둠)
    }

    
    private void ChangeScale(KoreographyEvent evt)
    {
        if (_objectIdx == _doTweenAnimations.Length)
        {
            _objectIdx = 0;
        }

        if (_objectIdx == 0)
        {
            _doTweenAnimations[_doTweenAnimations.Length - 1].DORewind();
            _doTweenAnimations[_objectIdx].DOPlay();
            _objectIdx++;
        }
        else
        {
            _doTweenAnimations[_objectIdx - 1].DORewind();
            _doTweenAnimations[_objectIdx].DOPlay();
            _objectIdx++;
        }
    }
}