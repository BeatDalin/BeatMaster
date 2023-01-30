using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public enum TextType
{
    Time,
    Item,
    Death,
}
public class LevelGameUI : MonoBehaviour
{
    public NormalGame levelGame;
    [Header("Base UI")]
    [SerializeField] private Text itemText;
    [SerializeField] private Text deathText;
    [Header("Result UI")] 
    [SerializeField] private GameObject _finalPanel;
    [SerializeField] private Text _finalFast;
    [SerializeField] private Text _finalPerfect;
    [SerializeField] private Text _finalSlow;
    [SerializeField] private GameObject[] _star;
    [SerializeField] private GameObject _starPrefab;
    [SerializeField] private RectTransform _target;
    [SerializeField] private GameObject _startPos;
    [SerializeField] private Color _successColor;
    [Header("Time Count UI")]
    [SerializeField] public GameObject timePanel;
    [SerializeField] public Text timeCount;
    [Header("Pause UI")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Button _continueBtn;
    [SerializeField] private Button _restartBtn;
    [SerializeField] private Button _goSettingsBtn;
    [SerializeField] private Button _goLevelMenuBtn;
    [Header("Settings UI")]
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private Button _settingsCloseBtn;

    private float delay = 0f;
    private void Awake()
    {
        levelGame = FindObjectOfType<NormalGame>();
        InitUI();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && levelGame.curState == GameState.Play)
        {
            OpenPause();
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && levelGame.curState == GameState.Pause && UIManager.instance.popUpStack.Count == 1)
        {
            UIManager.instance.ClosePopUp();
            levelGame.ContinueGame();
        }
    }

    public void InitUI()
    {
        itemText.text = "0";
        timePanel.SetActive(true);
        _finalPanel.SetActive(false);
        _pausePanel.SetActive(false);
        _settingsPanel.SetActive(false);
        // Button Events
        _continueBtn.onClick.AddListener(() =>
        {
            UIManager.instance.ClosePopUp();
            levelGame.ContinueGame();
        });
        _goLevelMenuBtn.onClick.AddListener(() => SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.MenuLevelSelect));
        //settings
        _goSettingsBtn.onClick.AddListener(() => UIManager.instance.OpenPopUp(_settingsPanel));
        _settingsCloseBtn.onClick.AddListener(() => { UIManager.instance.ClosePopUp(); });
    }

    public void UpdateText(TextType type, int number)
    {
        switch (type)
        {
            case TextType.Item:
                itemText.text = number.ToString();
                break;
            case TextType.Death:
                deathText.text = number.ToString();
                break;
            case TextType.Time:
                timeCount.text = number.ToString();
                break;
        }
    }
    
    public void ShowFinalResult(int[] finalResultSummary, int total, int stageIdx, int levelIdx)
    {
        _finalPanel.SetActive(true);
        
        _finalFast.DOCounter(0, finalResultSummary[1], 1).onComplete += () =>
        {
            _finalFast.text = $"{finalResultSummary[1]}/{total}";
            
            _finalSlow.DOCounter(0, finalResultSummary[3], 1).onComplete += () =>
            {
                _finalSlow.text = $"{finalResultSummary[3]}/{total}";
                
                _finalPerfect.DOCounter(0, finalResultSummary[2], 1).onComplete += () =>
                {
                    _finalPerfect.text = $"{finalResultSummary[2]}/{total}";

                    for (int i = 0; i < 30; i++)
                    {
                        GameObject g = Instantiate(_starPrefab);
                        g.SetActive(true);
                        
                        g.transform.SetParent(_startPos.transform);
                        g.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

                        g.transform.localScale = new Vector3(0, 0, 0);
            
                        g.transform.DOScale(new Vector3(1, 1, 1), 1f).SetDelay(delay).SetEase(Ease.OutBack);
            
                        g.GetComponent<RectTransform>()
                            .DOAnchorPos(_target.anchoredPosition, 1f)
                            .SetDelay(delay + 0.5f)
                            .SetEase(Ease.InBack).onComplete += () =>
                        {
                            g.transform.DOScale(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.Flash);
                        };

                        delay += 0.2f;
                    }
                    
                    ShowStar(DataCenter.Instance.GetLevelData(stageIdx, levelIdx).star);
                };
            };
        };
        //_finalFast.text = $"{finalResultSummary[1]}/{total}";
        //_finalPerfect.DOCounter(0, finalResultSummary[2], 1);
        //_finalPerfect.text = $"{finalResultSummary[2]}/{total}";
        //_finalSlow.DOCounter(0, finalResultSummary[3], 1);
        //_finalSlow.text = $"{finalResultSummary[3]}/{total}";
        
    }

    private void OpenPause()
    {
        UIManager.instance.OpenPopUp(_pausePanel);
        levelGame.PauseGame();
    }
    
    public void ShowStar(int starCount)
    {
        if (starCount == 3)
        {
            _star[0].SetActive(true);
            _star[0].GetComponent<Image>().color = _successColor;
            _star[0].transform.DORotate(new Vector3(0, 180, 0), 0.5f).onComplete += () =>
            {
                _star[1].SetActive(true);
                _star[1].GetComponent<Image>().color = _successColor;
                _star[1].transform.DORotate(new Vector3(0, 180, 0), 0.5f).onComplete += () =>
                {
                    _star[2].SetActive(true);
                    _star[2].GetComponent<Image>().color = _successColor;
                    _star[2].transform.DORotate(new Vector3(0, 180, 0), 0.5f);
                };
            };
        }
        else if (starCount == 2)
        {
            _star[0].SetActive(true);
            _star[0].GetComponent<Image>().color = _successColor;
            _star[0].transform.DORotate(new Vector3(0, 180, 0), 0.5f).onComplete += () =>
            {
                _star[1].SetActive(true);
                _star[1].GetComponent<Image>().color = _successColor;
                _star[1].transform.DORotate(new Vector3(0, 180, 0), 0.5f).onComplete += () =>
                {
                    _star[2].SetActive(true);
                    _star[2].transform.DORotate(new Vector3(0, 180, 0), 0.5f);
                };
            };
        }
        else
        {
            _star[0].SetActive(true);
            _star[0].GetComponent<Image>().color = _successColor;
            _star[0].transform.DORotate(new Vector3(0, 180, 0), 0.5f).onComplete += () =>
            {
                _star[1].SetActive(true);
                _star[1].transform.DORotate(new Vector3(0, 180, 0), 0.5f).onComplete += () =>
                {
                    _star[2].SetActive(true);
                    _star[2].transform.DORotate(new Vector3(0, 180, 0), 0.5f);
                };
            };
        }
        
    }

    
}
