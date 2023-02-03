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
    [SerializeField] private GameObject _stagePopUp;
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

        AddClickListener();
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
        _buttons[(int)ButtonName.Stage].onClick.AddListener(() =>
        {
            if (_buttons[(int)ButtonName.Stage].transform.localScale == new Vector3(1, 1, 1))
            {
                _buttons[(int)ButtonName.Stage].transform.DOScale(new Vector3(0.9f, 0.9f, 0), 0.1f).onComplete += () =>
                {
                    _buttons[(int)ButtonName.Stage].transform.DORewind();
                    OpenPopUp("Stage");
                };
            }
            else
            {
                OpenPopUp("Stage");            
            }
        });
        
        _buttons[(int)ButtonName.Store].onClick.AddListener(() =>
        {
            if (_buttons[(int)ButtonName.Store].transform.localScale == new Vector3(1, 1, 1))
            {
                _buttons[(int)ButtonName.Store].transform.DOScale(new Vector3(0.9f, 0.9f, 0), 0.1f).onComplete += () =>
                {
                    _buttons[(int)ButtonName.Store].transform.DORewind();
                    OpenPopUp("Store");
                };
            }
            else
            {
                OpenPopUp("Store");
            }
        });
        
        _buttons[(int)ButtonName.Setting].onClick.AddListener(() =>
        {
            if (_buttons[(int)ButtonName.Setting].transform.localScale == new Vector3(1, 1, 1))
            {
                _buttons[(int)ButtonName.Setting].transform.DOScale(new Vector3(0.9f, 0.9f, 0), 0.1f).onComplete += () =>
                {
                    _buttons[(int)ButtonName.Setting].transform.DORewind();
                    OpenPopUp("Setting");
                };
            }
            else
            {
                OpenPopUp("Setting");
            }
        });
        
        _buttons[(int)ButtonName.ShutDown].onClick.AddListener(() =>
        {
            if (_buttons[(int)ButtonName.ShutDown].transform.localScale == new Vector3(1, 1, 1))
            {
                _buttons[(int)ButtonName.ShutDown].transform.DOScale(new Vector3(0.9f, 0.9f, 0), 0.1f).onComplete += () =>
                {
                    _buttons[(int)ButtonName.ShutDown].transform.DORewind();
                    OpenPopUp("ShutDown");
                };
            }
            else
            {
                OpenPopUp("ShutDown");
            }
        });
    }

    private void OpenPopUp(string popUpName)
    {
        SoundManager.instance.PlaySFX("Touch");

        switch (popUpName)
        {
            case "Stage":
                UIManager.instance.OpenPopUp(_stagePopUp);
                break;
            
            case "Store":
                UIManager.instance.OpenPopUp(_storePopUp);
                break;
            
            case "Setting":
                UIManager.instance.OpenPopUp(_settingPopUp);
                break;
            
            case "ShutDown":
                Application.Quit();
                break;
        }
    }

    private void SceneMoveBtn(string sceneName)
    {
        SoundManager.instance.PlaySFX("Touch");
        
        //temp
        if (sceneName == "Stage")
        {
            SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.MenuLevelSelect);
        }
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