using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SonicBloom.Koreo;
using UnityEngine;
using UnityEngine.UI;

public enum MenuButtonName
{
    StartGame = 0,
    Setting,
    Store,
    Quit
}

public enum SettingButtonName
{
    QualitySetting,
    SoundSetting,
    LanguageSetting
}

public class MenuTitleButton : MonoBehaviour
{
    [EventID] public string eventID;

    [SerializeField] private DOTweenAnimation[] _doTweenAnimations;
    [SerializeField] private Button[] _menuButtons; // <Title - Menu> Buttons
    [SerializeField] private Button[] _settingButtons; // <Title - Setting> Buttons

    [SerializeField] private GameObject _startGamePopUp; // <Panel> Start Game
    [SerializeField] private GameObject _settingPopUp; // <Panel> Settings
    [SerializeField] private GameObject _storePopUp; // <Panel> Store

    [SerializeField] private GameObject _qualitySettingPopUp; // <Panel> QualitySetting
    [SerializeField] private GameObject _soundSettingPopUp; // <Panel> SoundSetting
    [SerializeField] private GameObject _languageSettingPopUp; // <Panel> LanguagePanel


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
    /// 눌린것을 표현하기 위해서 DOScale을 사용하고 완료되면 DORewind로 원래 Scale로 돌아오게함
    /// </summary>
    private void AddClickListener()
    {
        #region Menu Buttons (StartGame, Store, Setting, Quit)

        // <Menu - Button> StartGame
        _menuButtons[(int)MenuButtonName.StartGame].onClick.AddListener(() =>
        {
            OpenPopUp("StartGame");
            #region legacy

            // if (_menuButtons[(int)MenuButtonName.StartGame].transform.localScale == new Vector3(1, 1, 1))
            // {
            //     _menuButtons[(int)MenuButtonName.StartGame].transform.DOScale(new Vector3(0.9f, 0.9f, 0), 0.1f)
            //         .onComplete += () =>
            //     {
            //         _menuButtons[(int)MenuButtonName.StartGame].transform.DORewind();
            //         OpenPopUp("StartGame");
            //     };
            // }
            // else
            // {
            //     OpenPopUp("StartGame");
            // }

            #endregion
        });

        // <Menu - Button> Store
        _menuButtons[(int)MenuButtonName.Store].onClick.AddListener(() =>
        {
            OpenPopUp("Store");
            
            // if (_menuButtons[(int)MenuButtonName.Store].transform.localScale == new Vector3(1, 1, 1))
            // {
            //     _menuButtons[(int)MenuButtonName.Store].transform.DOScale(new Vector3(0.9f, 0.9f, 0), 0.1f).onComplete +=
            //         () =>
            //         {
            //             _menuButtons[(int)MenuButtonName.Store].transform.DORewind();
            //             OpenPopUp("Store");
            //         };
            // }
            // else
            // {
            //     OpenPopUp("Store");
            // }
        });

        // <Menu - Button> Setting
        _menuButtons[(int)MenuButtonName.Setting].onClick.AddListener(() =>
        {
            OpenPopUp("Setting");

            // if (_menuButtons[(int)MenuButtonName.Setting].transform.localScale == new Vector3(1, 1, 1))
            // {
            //     _menuButtons[(int)MenuButtonName.Setting].transform.DOScale(new Vector3(0.9f, 0.9f, 0), 0.1f).onComplete +=
            //         () =>
            //         {
            //             _menuButtons[(int)MenuButtonName.Setting].transform.DORewind();
            //             OpenPopUp("Setting");
            //         };
            // }
            // else
            // {
            //     OpenPopUp("Setting");
            // }
        });

        // <Menu - Button> Quit
        // _menuButtons[(int)MenuButtonName.Quit].onClick.AddListener(() =>
        // {
        //     if (_menuButtons[(int)MenuButtonName.Quit].transform.localScale == new Vector3(1, 1, 1))
        //     {
        //         _menuButtons[(int)MenuButtonName.Quit].transform.DOScale(new Vector3(0.9f, 0.9f, 0), 0.1f).onComplete +=
        //             () =>
        //             {
        //                 _menuButtons[(int)MenuButtonName.Quit].transform.DORewind();
        //                 OpenPopUp("Quit");
        //             };
        //     }
        //     else
        //     {
        //         OpenPopUp("Quit");
        //     }
        // });

        #endregion

        #region Setting Buttons (Quality, Sound, Language Settings)

        // <Setting - Button> Quality Settings
        _settingButtons[(int)SettingButtonName.QualitySetting].onClick.AddListener(() =>
        {
            if (_settingButtons[(int)SettingButtonName.QualitySetting].transform.localScale == new Vector3(1, 1, 1))
            {
                _settingButtons[(int)SettingButtonName.QualitySetting].transform
                        .DOScale(new Vector3(0.9f, 0.9f, 0), 0.1f)
                        .onComplete +=
                    () =>
                    {
                        _settingButtons[(int)SettingButtonName.QualitySetting].transform.DORewind();
                        OpenPopUp("QualitySetting");
                    };
            }
            else
            {
                OpenPopUp("QualitySetting");
            }
        });
        // <Setting - Button> Sound Settings
        _settingButtons[(int)SettingButtonName.SoundSetting].onClick.AddListener(() =>
        {
            if (_settingButtons[(int)SettingButtonName.SoundSetting].transform.localScale == new Vector3(1, 1, 1))
            {
                _settingButtons[(int)SettingButtonName.SoundSetting].transform.DOScale(new Vector3(0.9f, 0.9f, 0), 0.1f)
                        .onComplete +=
                    () =>
                    {
                        _settingButtons[(int)SettingButtonName.SoundSetting].transform.DORewind();
                        OpenPopUp("SoundSetting");
                    };
            }
            else
            {
                OpenPopUp("SoundSetting");
            }
        });
        // <Setting - Button> Quality Settings
        _settingButtons[(int)SettingButtonName.LanguageSetting].onClick.AddListener(() =>
        {
            if (_settingButtons[(int)SettingButtonName.LanguageSetting].transform.localScale == new Vector3(1, 1, 1))
            {
                _settingButtons[(int)SettingButtonName.LanguageSetting].transform
                        .DOScale(new Vector3(0.9f, 0.9f, 0), 0.1f)
                        .onComplete +=
                    () =>
                    {
                        _settingButtons[(int)SettingButtonName.LanguageSetting].transform.DORewind();
                        OpenPopUp("LanguageSetting");
                    };
            }
            else
            {
                OpenPopUp("LanguageSetting");
            }
        });

        #endregion
    }


    private void OpenPopUp(string popUpName)
    {
        SoundManager.instance.PlaySFX("Touch");

        switch (popUpName)
        {
            case "StartGame":
                UIManager.instance.OpenPopUp(_startGamePopUp);
                break;
            case "Store":
                UIManager.instance.OpenPopUp(_storePopUp);
                break;
            case "Setting":
                UIManager.instance.OpenPopUp(_settingPopUp);
                break;
            case "Quit":
                Application.Quit();
                break;

            case "QualitySetting":
                UIManager.instance.OpenPopUp(_qualitySettingPopUp);
                break;
            case "SoundSetting":
                UIManager.instance.OpenPopUp(_soundSettingPopUp);
                break;
            case "LanguageSetting":
                UIManager.instance.OpenPopUp(_languageSettingPopUp);
                break;
        }
    }

    private void ChangeScale(KoreographyEvent evt)
    {
        for (int i = 0; i < _doTweenAnimations.Length; i++)
        {
            _doTweenAnimations[i].DORewind();
            _doTweenAnimations[i].DOPlay();
        }
    }
}