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
public abstract class GameUI : MonoBehaviour
{
    [Header("Game")]
    [SerializeField] protected Game game;
    
    [Header("Result UI")] 
    [SerializeField] protected GameObject finalPanel;
    [SerializeField] protected Text finalFast;
    [SerializeField] protected Text finalPerfect;
    [SerializeField] protected Text finalSlow;
    [SerializeField] protected GameObject[] star;
    [SerializeField] protected GameObject starPrefab;
    [SerializeField] protected RectTransform target;
    [SerializeField] protected GameObject startPos;
    [SerializeField] protected Color successColor;
    private float _delay = 0f;
    [SerializeField] protected Button goLevelAfterGameBtn;
    
    [Header("Result Visualize")]
    [SerializeField] protected RectTransform maskImage;
    [SerializeField] protected RectTransform outLine;
    [SerializeField] private Image _outLineColor;
    [SerializeField] private Color _perfectColor;
    [SerializeField] private Color _fastColor;
    [SerializeField] private Color _slowColor;
    [SerializeField] private Color _failColor;
    
    [Header("Time Count UI")]
    [SerializeField] public GameObject timePanel;
    [SerializeField] public Text timeCount;
    
    [Header("Pause UI")]
    [SerializeField] protected GameObject pausePanel;
    [SerializeField] protected Button continueBtn;
    [SerializeField] protected Button restartBtn;
    [SerializeField] protected Button goSettingsBtn;
    [SerializeField] protected Button goLevelMenuBtn;
    
    [Header("Settings UI")]
    [SerializeField] protected GameObject settingsPanel;
    [SerializeField] protected Button settingsCloseBtn;

    #region Abstract Function
    public abstract void UpdateText(TextType type, int number);
    #endregion

    
    protected virtual void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && game.curState == GameState.Play)
        {
            OpenPause();
        }
        else if(Input.GetKeyDown(KeyCode.Escape) && game.curState == GameState.Pause && UIManager.instance.popUpStack.Count == 1)
        {
            UIManager.instance.ClosePopUp();
            game.ContinueGame();
        }
    }

    protected void OpenPause()
    {
        UIManager.instance.OpenPopUp(pausePanel);
        game.PauseGame();
    }

    public virtual void InitUI()
    {
        // panels
        timePanel.SetActive(true);
        finalPanel.SetActive(false);
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        // outline
        outLine.sizeDelta = new Vector2(Screen.width, Screen.height);
        maskImage.sizeDelta = new Vector2(Screen.width - 60f, Screen.height - 60f);
        // star
        foreach (var s in star)
        {
            s.SetActive(false);
        }
        // Button Events
        continueBtn.onClick.AddListener(() =>
        {
            UIManager.instance.ClosePopUp();
            game.ContinueGame();
        });
        goLevelMenuBtn.onClick.AddListener(() => SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.MenuLevelSelect));
        //settings
        goSettingsBtn.onClick.AddListener(() => UIManager.instance.OpenPopUp(settingsPanel));
        settingsCloseBtn.onClick.AddListener(() => { UIManager.instance.ClosePopUp(); });
        
        goLevelAfterGameBtn.onClick.AddListener(() => SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.MenuLevelSelect));
    }

    public void ChangeOutLineColor(BeatResult result)
    {
        if (result == BeatResult.Perfect)
        {
            _outLineColor.DOColor(_perfectColor, 0.1f).onComplete += () =>
            {
                _outLineColor.DOColor(Color.white, 0.1f);
            };
        }
        else if (result == BeatResult.Fast)
        {
            _outLineColor.DOColor(_fastColor, 0.1f).onComplete += () =>
            {
                _outLineColor.DOColor(Color.white, 0.1f);
            };
        }
        else if (result == BeatResult.Slow)
        {
            _outLineColor.DOColor(_slowColor, 0.1f).onComplete += () =>
            {
                _outLineColor.DOColor(Color.white, 0.1f);
            };
        }
        else
        {
            _outLineColor.DOColor(_failColor, 0.1f).onComplete += () =>
            {
                _outLineColor.DOColor(Color.white, 0.1f);
            };
        }
    }
    
    public void ShowFinalResult(int[] finalResultSummary, int total, int stageIdx, int levelIdx)
    {
        finalPanel.SetActive(true);
        
        finalFast.DOCounter(0, finalResultSummary[1], 1).onComplete += () =>
        {
            finalFast.text = $"{finalResultSummary[1]}/{total}";
            
            finalSlow.DOCounter(0, finalResultSummary[3], 1).onComplete += () =>
            {
                finalSlow.text = $"{finalResultSummary[3]}/{total}";
                
                finalPerfect.DOCounter(0, finalResultSummary[2], 1).onComplete += () =>
                {
                    finalPerfect.text = $"{finalResultSummary[2]}/{total}";

                    for (int i = 0; i < 30; i++)
                    {
                        GameObject g = Instantiate(starPrefab);
                        g.SetActive(true);
                        
                        g.transform.SetParent(startPos.transform);
                        g.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

                        g.transform.localScale = new Vector3(0, 0, 0);
            
                        g.transform.DOScale(new Vector3(1, 1, 1), 1f).SetDelay(_delay).SetEase(Ease.OutBack);
            
                        g.GetComponent<RectTransform>()
                            .DOAnchorPos(target.anchoredPosition, 1f)
                            .SetDelay(_delay + 0.5f)
                            .SetEase(Ease.InBack).onComplete += () =>
                        {
                            g.transform.DOScale(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.Flash);
                        };

                        _delay += 0.2f;
                    }
                    
                    ShowStar(DataCenter.Instance.GetLevelData(stageIdx, levelIdx).star);
                };
            };
        };
    }
    
    
    public void ShowStar(int starCount)
    {
        if (starCount == 3)
        {
            star[0].SetActive(true);
            star[0].GetComponent<Image>().color = successColor;
            star[0].transform.DORotate(new Vector3(0, 180, 0), 0.5f).onComplete += () =>
            {
                star[1].SetActive(true);
                star[1].GetComponent<Image>().color = successColor;
                star[1].transform.DORotate(new Vector3(0, 180, 0), 0.5f).onComplete += () =>
                {
                    star[2].SetActive(true);
                    star[2].GetComponent<Image>().color = successColor;
                    star[2].transform.DORotate(new Vector3(0, 180, 0), 0.5f);
                };
            };
        }
        else if (starCount == 2)
        {
            star[0].SetActive(true);
            star[0].GetComponent<Image>().color = successColor;
            star[0].transform.DORotate(new Vector3(0, 180, 0), 0.5f).onComplete += () =>
            {
                star[1].SetActive(true);
                star[1].GetComponent<Image>().color = successColor;
                star[1].transform.DORotate(new Vector3(0, 180, 0), 0.5f).onComplete += () =>
                {
                    star[2].SetActive(true);
                    star[2].transform.DORotate(new Vector3(0, 180, 0), 0.5f);
                };
            };
        }
        else
        {
            star[0].SetActive(true);
            star[0].GetComponent<Image>().color = successColor;
            star[0].transform.DORotate(new Vector3(0, 180, 0), 0.5f).onComplete += () =>
            {
                star[1].SetActive(true);
                star[1].transform.DORotate(new Vector3(0, 180, 0), 0.5f).onComplete += () =>
                {
                    star[2].SetActive(true);
                    star[2].transform.DORotate(new Vector3(0, 180, 0), 0.5f);
                };
            };
        }
    }
}
