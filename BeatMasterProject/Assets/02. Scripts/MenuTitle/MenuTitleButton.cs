using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SonicBloom.Koreo;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public enum MenuButtonName // Title 씬의 버튼들
{
    Play = 0,
    Setting,
    Store,
}

public enum SettingButtonName // Setting Panel의 버튼들
{
    Sound = 0,
    Quality,
    Leave
}

public class MenuTitleButton : MonoBehaviour
{
    [EventID] public string eventID;

    [SerializeField] private DOTweenAnimation[] _doTweenAnimations;

    [Header("==== Title Buttons ====")] 
    [SerializeField] private Button[] _menuButtons; // <Title - Menu> Buttons

    [Header("==== Title Panel ====")]
    [SerializeField] private GameObject _settingPanel; // <Panel> Settings
    [SerializeField] private GameObject _storePanel; // <Panel> Store

    [Header("==== Setting Panel Buttons ====")] 
    [SerializeField] private Button[] _settingButtons; // <Title - Setting> Buttons

    [Header("==== Setting Panel ====")] 
    [SerializeField] private GameObject _soundSettingPanel; // <Panel> SoundSetting\
    [SerializeField] private GameObject _qualitySettingPanel; // <Panel> QualitySetting
    [SerializeField] private GameObject _leavePanel; // <Panel> LeavePopUp

    [Header("==== Close Button ====")] 
    [SerializeField] private Button _closeButton;

    [SerializeField] private float _fadeTime = 1f; // Panel Fade 타임

    private int _objectIdx = 0;

    private void Start()
    {
        // Event Register
        Koreographer.Instance.RegisterForEvents("Title_Track", ChangeScale);

        AddClickListener();

        if (!SoundManager.instance.musicPlayer.IsPlaying)
        {
            SoundManager.instance.musicPlayer.Play();
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.ClosePopUp();
        }
    }

    /// <summary>
    /// 각 버튼에 클릭 리스너를 달아주는 함수
    /// </summary>
    private void AddClickListener()
    {
        #region Menu Buttons (StartGame, Store, Setting)

        // <Menu - Button> StartGame
        _menuButtons[(int)MenuButtonName.Play].onClick.AddListener(() =>
        {
            SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.LevelSelect);
        });
        // <Menu - Button> Store
        _menuButtons[(int)MenuButtonName.Store].onClick.AddListener(() => { OpenPanelFadeIn(_storePanel); });
        // <Menu - Button> Setting
        _menuButtons[(int)MenuButtonName.Setting].onClick.AddListener(() => { OpenPanelFadeIn(_settingPanel); });

        #endregion

        #region Setting Buttons (Sound, Quality, Leave)

        // // <Setting - Button> Sound Settings
        // _settingButtons[(int)SettingButtonName.Sound].onClick.AddListener(() => { OpenPopUp("SoundSetting"); });
        // // <Setting - Button> Quality Settings
        // _settingButtons[(int)SettingButtonName.Quality].onClick
        //     .AddListener(() => { OpenPopUp("QualitySetting"); });
        // // <Setting - Button> Leave
        // _settingButtons[(int)SettingButtonName.Leave].onClick.AddListener(() => { OpenPopUp("Leave"); });

        #endregion
        
    }

    public void OpenPanelFadeIn(GameObject panelName)
    {
        panelName.SetActive(true);
        panelName.GetComponent<RectTransform>().localPosition = new Vector3(Screen.width, 0, 0);
    }

    public void ClosePanelFadeOut(GameObject panelName)
    {
        panelName.GetComponent<RectTransform>().DOLocalMove(new Vector3(Screen.width, 0, 0), 0.4f).onComplete += () =>
        {
            panelName.SetActive(false);
        };
    }


    // private void OpenPopUp(string popUpName)
    // {
    //     SoundManager.instance.PlaySFX("Touch");
    //
    //     switch (popUpName)
    //     {
    //         case "StartGame":
    //             if (!_playPanel.activeSelf)
    //             {
    //                 _playPanel.SetActive(true);
    //             }
    //
    //             break;
    //         case "Store":
    //             if (!_storePanel.activeSelf)
    //             {
    //                 _storePanel.SetActive(true);
    //             }
    //
    //             break;
    //         case "Setting":
    //             if (!_settingPanel.activeSelf)
    //             {
    //                 _settingPanel.SetActive(true);
    //             }
    //
    //             //UIManager.instance.OpenPopUp(_settingPopUp);
    //             break;
    //         case "SoundSetting":
    //             //UIManager.instance.OpenPopUp(_soundSettingPopUp);
    //             break;
    //         case "QualitySetting":
    //             //UIManager.instance.OpenPopUp(_qualitySettingPopUp);
    //             break;
    //         case "Leave":
    //             //UIManager.instance.OpenPopUp(_leavePopUp);
    //             break;
    //     }
    // }

    private void ChangeScale(KoreographyEvent evt)
    {
        for (int i = 0; i < _doTweenAnimations.Length; i++)
        {
            _doTweenAnimations[i].DORewind();
            _doTweenAnimations[i].DOPlay();
        }
    }
}