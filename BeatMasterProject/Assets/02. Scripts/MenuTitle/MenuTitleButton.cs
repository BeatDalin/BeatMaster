using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using TMPro;
using UnityEngine;
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
    [EventID] public string eventID;

    [SerializeField] private DOTweenAnimation[] _doTweenAnimations;
    [SerializeField] private Button[] _buttons;

    [SerializeField] private GameObject _settingPopUp;
    [SerializeField] private GameObject _storePopUp;
    [SerializeField] private GameObject _stagePopUp;
    [SerializeField] private CanvasGroup _loadingPanelGroup;

    private int _objectIdx = 0;


    private void Awake()
    {
    }

    private void Start()
    {
        AddClickListener();
        Koreographer.Instance.RegisterForEvents(eventID, ChangeScale);
    }

    private void Update()
    {
        if (_loadingPanelGroup.alpha == 0)
        {
            if (!SoundManager.instance.musicPlayer.IsPlaying)
            {
                SoundManager.instance.musicPlayer.Play();
                _loadingPanelGroup.gameObject.SetActive(false);
            }
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            UIManager.instance.ClosePopUp();
        }
    }

    private void ChangeScale(KoreographyEvent evt)
    {
        if (_doTweenAnimations[0] == null)
        {
            _doTweenAnimations[0] = GameObject.Find("TitleText").GetComponent<DOTweenAnimation>();
            _doTweenAnimations[1] = GameObject.Find("StageBtn").GetComponent<DOTweenAnimation>();
            _doTweenAnimations[2] = GameObject.Find("StoreBtn").GetComponent<DOTweenAnimation>();
            _doTweenAnimations[3] = GameObject.Find("SettingBtn").GetComponent<DOTweenAnimation>();
            _doTweenAnimations[4] = GameObject.Find("ShotdownBtn").GetComponent<DOTweenAnimation>();

        }

        for (int i = 0; i < _doTweenAnimations.Length; i++)
        {
            _doTweenAnimations[i].DORewind();
            _doTweenAnimations[i].DOPlay();
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
                _buttons[(int)ButtonName.Setting].transform.DOScale(new Vector3(0.9f, 0.9f, 0), 0.1f).onComplete +=
                    () =>
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
                _buttons[(int)ButtonName.ShutDown].transform.DOScale(new Vector3(0.9f, 0.9f, 0), 0.1f).onComplete +=
                    () =>
                    {
                        _buttons[(int)ButtonName.ShutDown].transform.DORewind();
                        SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.Level1MonsterTest);
                    };
            }
            else
            {
                SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.Level1MonsterTest);
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
            SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.LevelSelect);
        }
    }
}