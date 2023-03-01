using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SonicBloom.Koreo;
using UnityEngine;
using UnityEngine.UI;

public enum TitleButtonName
{
    Play = 0,
    Menu,
    Store,
    Gpgs,
    Announce,
}
public enum MenuButtonName // 메뉴 버튼들
{
    Setting = 0,
    Quit
}

public class MenuTitleButton : MonoBehaviour
{
    
    [EventID] public string eventID;

    [SerializeField] private DOTweenAnimation[] _doTweenAnimations;

    [Header("========= Buttons =========")] 
    [SerializeField] private Button[] _titleButtons;    // 플레이, 메뉴, 상점 버튼
    [SerializeField] private Button[] _menuButtons;     // 세팅, 내정보, 게임 종료 버튼


    [Header("========= Panels =========")] 
    [SerializeField] private GameObject _menuGroupPanel; // Menu Group
    [SerializeField] private GameObject _storePanel; // Store Panel
    [SerializeField] private GameObject _gpgsPanel;  // Google Game Service Panel
    [SerializeField] private GameObject _announcePanel;  // Announce Panel
    [SerializeField] private GameObject[] _menuPanels;  // Menu Panels

    [SerializeField] private float _fadeTime = 1f; // Panel Fade 타임

    
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
            //UIManager.instance.ClosePopUp();
        }
    }

    /// <summary>
    /// 각 버튼에 클릭 리스너를 달아주는 함수
    /// </summary>
    private void AddClickListener()
    {
        #region Title 씬의 버튼들 (Play, Menu, Store)
        
        _titleButtons[(int)TitleButtonName.Play].onClick.AddListener(() =>
        {
            #if UNITY_ANDROID && !UNITY_EDITOR
            ExcuteVibration.Instance.Touch();
            #endif
            SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.LevelSelect);
        });  // Play 버튼
        _titleButtons[(int)TitleButtonName.Menu].onClick.AddListener(() =>
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            ExcuteVibration.Instance.Touch();
#endif
            OpenMenu(_menuGroupPanel); 
        });     // Menu 버튼
        _titleButtons[(int)TitleButtonName.Store].onClick.AddListener(() =>
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            ExcuteVibration.Instance.Touch();
#endif
            
            UIManager.instance.OpenPanel(_storePanel); 
        });       // Store 버튼
        _titleButtons[(int)TitleButtonName.Gpgs].onClick.AddListener(() =>
        {
#if UNITY_ANDROID && !UNITY_EDITOR
            ExcuteVibration.Instance.Touch();
#endif
            UIManager.instance.OpenPanel(_gpgsPanel);
        });

        _titleButtons[(int)TitleButtonName.Announce].onClick.AddListener(() => { 
            #if UNITY_ANDROID && !UNITY_EDITOR
            ExcuteVibration.Instance.Touch();
            #endif
            UIManager.instance.OpenPanel(_announcePanel); 
            });

        #endregion

        #region MenuGroup의 버튼들 (Settings, MyInfo, Quit)

        // Menu - Settings 버튼 (Setting, MyInfo, Quit)
        _menuButtons[(int)MenuButtonName.Setting].onClick.AddListener(() => { UIManager.instance.OpenPanel(_menuPanels[(int)MenuButtonName.Setting]); });
        _menuButtons[(int)MenuButtonName.Quit].onClick.AddListener(() => { UIManager.instance.OpenPanel(_menuPanels[(int)MenuButtonName.Quit]); });
        
        #endregion
        
    }
    public void OpenMenu(GameObject panelName)
    {
        SoundManager.instance.PlaySFX("Touch");

        if (!panelName.activeSelf)
        {
            panelName.SetActive(true);
        }
        else
        {   
            panelName.SetActive(false);
        }
    }
    public void CloseMenu(GameObject panelName)
    {
        panelName.SetActive(false);
    }
    
    public void OpenPanel(GameObject panelName)
    {
        if (!panelName.activeSelf)
        {
            SoundManager.instance.PlaySFX("Touch");
            CloseMenu(_menuGroupPanel);

            foreach (var titleButton in _titleButtons)
            {
                ActivateButton(titleButton,false);
            }
            
            panelName.SetActive(true);
            panelName.GetComponent<RectTransform>().localPosition = new Vector3(Screen.width, 0, 0);
            
        }
    }
    public void ClosePanel(GameObject panelName)
    {
        SoundManager.instance.PlaySFX("Touch");

        panelName.GetComponent<RectTransform>().DOLocalMove(new Vector3(Screen.width, 0, 0), 0.4f).onComplete += () =>
        {
            panelName.SetActive(false);

            foreach (var titleButton in _titleButtons)
            {
                ActivateButton(titleButton,true);
            }
        };
    }

    // 다른 UI가 활성/비활성화 되었을 때, 다른 버튼 클릭(터치) 가능/불가능 하도록!
    public void ActivateButton(Button buttonName, bool isInteractable)
    {
        buttonName.interactable = isInteractable;
    }    
    private void ChangeScale(KoreographyEvent evt)
    {
        for (int i = 0; i < _doTweenAnimations.Length; i++)
        {
            _doTweenAnimations[i].DORestart();

        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
    
}