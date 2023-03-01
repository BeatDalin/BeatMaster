using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public enum TextType
{
    Time,
    Item,
    Death,
}

public abstract class GameUI : MonoBehaviour
{
    [Header("Game")] protected Game game;

    [Header("Result UI")]
    [SerializeField] protected GameObject finalPanel;
    [SerializeField] protected GameObject buttonGroup;
    [SerializeField] protected Text finalFast;
    [SerializeField] protected Text finalPerfect;
    [SerializeField] protected Text finalSlow;
    [SerializeField] protected GameObject[] star;
    [SerializeField] protected GameObject starPrefab;
    [SerializeField] protected RectTransform target;
    [SerializeField] protected GameObject startPos;
    [SerializeField] protected Color successColor;
    private float _delay = 0f;
    [SerializeField] protected Button goToMenuBtn;
    [SerializeField] protected Button playAgainBtn;
    [SerializeField] protected Button showLeaderboardBtn;

    [Header("Result Visualize")]
    [SerializeField] private ParticleSystem _perfectParticle;

    [SerializeField] private ParticleSystem _fastParticle;
    [SerializeField] private ParticleSystem _slowParticle;
    [SerializeField] private ParticleSystem _failParticle;
    [SerializeField] private Text _judgeText;
    [SerializeField] private RectTransform _textStart;
    [SerializeField] private RectTransform _textEnd;
    [SerializeField] private RectTransform _textRect;

    [SerializeField] private Color _perfectColor;
    [SerializeField] private Color _fastColor;
    [SerializeField] private Color _slowColor;
    [SerializeField] private Color _failColor;

    [Header("Time Count UI")]
    public GameObject timePanel;

    [SerializeField] public Text timeCount;

    [Header("Pause UI")]
    [SerializeField] protected Button pauseBtn;
    [SerializeField] protected GameObject pausePanel;
    [SerializeField] protected Button continueBtn;
    [SerializeField] protected Button restartBtn;
    [SerializeField] protected Button goSettingsBtn;
    [SerializeField] protected Button goLevelMenuBtn;

    [Header("Settings UI")]
    [SerializeField] protected GameObject settingsPanel;

    [SerializeField] protected Button settingsCloseBtn;

    [SerializeField] private List<ParticleSystem> _particleSystemsList = new List<ParticleSystem>();


    [Header("Player Character")]
    [SerializeField] protected GameObject character;

    [Header("DOTween Animations")]
    [SerializeField] private DOTweenAnimation _pauseBtnDOT;
    #region Abstract Function

    public abstract void UpdateText(TextType type, int number);

    #endregion


    // protected virtual void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Escape) && game.curState == GameState.Play)
    //     {
    //         OpenPause();
    //     }
    //     else if (Input.GetKeyDown(KeyCode.Escape) && game.curState == GameState.Pause &&
    //              UIManager.instance.popUpStack.Count == 1)
    //     {
    //         character.SetActive(true);
    //         UIManager.instance.ClosePopUp();
    //         game.ContinueGame();
    //     }
    // }

    public void OpenPause()
    {
        if (game.curState.Equals(GameState.Play))
        {
            _pauseBtnDOT.DORestart();
            ExcuteVibration.Instance.Touch();
            SoundManager.instance.PlaySFX("Touch");
            UIManager.instance.OpenPanel(pausePanel);
            game.PauseGame();
            character.SetActive(false);
        }
    }

    public virtual void InitUI()
    {
        // panels
        timePanel.SetActive(true);
        finalPanel.SetActive(false);
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);

        // star
        foreach (var s in star)
        {
            s.SetActive(false);
        }

        // Button Events
        pauseBtn.onClick.AddListener(() => OpenPause());
        continueBtn.onClick.AddListener(() =>
        {
            ExcuteVibration.Instance.Touch();
            SoundManager.instance.PlaySFX("Touch");
            character.SetActive(true);
            UIManager.instance.ClosePanel(pausePanel);
            game.ContinueGame();
        });
        restartBtn.onClick.AddListener(() =>
        {
            ExcuteVibration.Instance.Touch();
            SoundManager.instance.PlaySFX("Touch");
            character.SetActive(false);
            SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.Instance.Scene);
        });
        goLevelMenuBtn.onClick.AddListener(() =>
        {
            ExcuteVibration.Instance.Touch();
            SoundManager.instance.PlaySFX("Touch");
            character.SetActive(false);
            SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.LevelSelect);
        });
        //settings
        goSettingsBtn.onClick.AddListener(() =>
        {
            ExcuteVibration.Instance.Touch();
            UIManager.instance.OpenPanel(settingsPanel);
        });
        settingsCloseBtn.onClick.AddListener(() =>
        {
            ExcuteVibration.Instance.Touch();
            UIManager.instance.ClosePanel(settingsPanel);
        });

        goToMenuBtn.onClick.AddListener(() =>
        {
            ExcuteVibration.Instance.Touch();
            character.SetActive(false);
            SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.LevelSelect);
        });

        playAgainBtn.onClick.AddListener(() =>
        {
            ExcuteVibration.Instance.Touch();
            SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.Instance.Scene);
        });

        showLeaderboardBtn.onClick.AddListener(() =>
        {
            ExcuteVibration.Instance.Touch();
            int stageIdx = (int)SceneLoadManager.Instance.Scene - 2;
#if !UNITY_EDITOR
            GPGSBinder.Instance.ShowTargetLeaderboardUI(GPGSBinder.Instance.CheckStageIdx(stageIdx));
#endif
        });
    }

    public void ChangeOutLineColor(BeatResult result)
    {
        switch (result)
        {
            case BeatResult.Perfect:
                TextMove("Perfect");
                _judgeText.DOColor(_perfectColor, 0.1f);
                break;

            case BeatResult.Fast:
                TextMove("Fast");
                _judgeText.DOColor(_fastColor, 0.1f);
                break;

            case BeatResult.Slow:
                TextMove("Slow");
                _judgeText.DOColor(_slowColor, 0.1f);
                break;

            case BeatResult.Fail:
                TextMove("Fail");
                _judgeText.DOColor(_failColor, 0.1f);
                break;
        }
    }

    public void ReverseTextColor(string result)
    {
        switch (result)
        {
            case "Perfect":
                _judgeText.DOColor(_perfectColor, 0f);
                TextMove("Perfect");
                break;

            case "Fast":
                _judgeText.DOColor(_fastColor, 0f);
                TextMove("Fast");
                break;

            case "Slow":
                _judgeText.DOColor(_slowColor, 0f);
                TextMove("Slow");
                break;

            case "Fail":
                _judgeText.DOColor(_failColor, 0f);
                TextMove("Fail");
                break;
        }
    }

    private void TextMove(string input)
    {
        _judgeText.text = input;
        _judgeText.DOFade(1, 0.1f);

        _textRect.DOLocalMove(_textEnd.localPosition, 0.2f)
            .onComplete += () =>
        {
            _judgeText.DOFade(0, 0.3f).onComplete += () => { _textRect.localPosition = _textStart.localPosition; };
        };
    }

    public void ShowFinalResult(int[] finalResultSummary, int total, int stageIdx, int levelIdx)
    {
        finalPanel.SetActive(true);
        pauseBtn.interactable = false;
        finalSlow.DOCounter(0, finalResultSummary[3], 1).onComplete += () =>
        {
            //finalSlow.text = $"{finalResultSummary[3]}/{total}";

            finalFast.DOCounter(0, finalResultSummary[1], 1).onComplete += () =>
            {
                //finalFast.text = $"{finalResultSummary[1]}/{total}";

                finalPerfect.DOCounter(0, finalResultSummary[2], 1).onComplete += () =>
                {
                    //finalPerfect.text = $"{finalResultSummary[2]}/{total}";

                    float temp = (float)finalResultSummary[2] / total;
                    int starCount = (int)Mathf.Ceil(temp * 10);
                    // ShowStar(DataCenter.Instance.GetLevelData(stageIdx, levelIdx).star);
                    // for (int i = 0; i < starCount; i++)
                    // {
                    //     if (i == starCount - 1)
                    //     {
                    //         GameObject g = Instantiate(starPrefab);
                    //         g.SetActive(true);
                    //
                    //         g.transform.SetParent(startPos.transform);
                    //         g.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                    //
                    //         g.transform.localScale = new Vector3(0, 0, 0);
                    //
                    //         g.transform.DOScale(new Vector3(1, 1, 1), 1f).SetDelay(_delay).SetEase(Ease.OutBack);
                    //
                    //         g.GetComponent<RectTransform>()
                    //             .DOLocalMove(target.localPosition, 1f)
                    //             .SetDelay(_delay + 0.5f)
                    //             .SetEase(Ease.InBack).onComplete += () =>
                    //         {
                    //             g.transform.DOScale(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.Flash).onComplete += () =>
                    //             {
                    //                 ShowStar(DataCenter.Instance.GetLevelData(stageIdx, levelIdx).star);
                    //             };
                    //         };
                    //
                    //         _delay += 0.2f;
                    //     }
                    //     else
                    //     {
                    //         GameObject g = Instantiate(starPrefab);
                    //         g.SetActive(true);
                    //
                    //         g.transform.SetParent(startPos.transform);
                    //         g.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);
                    //
                    //         g.transform.localScale = new Vector3(0, 0, 0);
                    //
                    //         g.transform.DOScale(new Vector3(1, 1, 1), 1f).SetDelay(_delay).SetEase(Ease.OutBack);
                    //
                    //         g.GetComponent<RectTransform>()
                    //             .DOLocalMove(target.localPosition, 1f)
                    //             .SetDelay(_delay + 0.5f)
                    //             .SetEase(Ease.InBack).onComplete += () =>
                    //         {
                    //             g.transform.DOScale(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.Flash);
                    //         };
                    //
                    //         _delay += 0.2f;
                    //     }
                    // }
                };
            };
        };
    }
    
    public void ShowButtonGroup()
    {
        buttonGroup.SetActive(true);
    }
    public void ShowStar(int starCount)
    {
        if (starCount == 3)
        {
            star[0].SetActive(true);
            star[0].GetComponent<Image>().color = successColor;
            star[0].transform.DOPunchRotation(new Vector3(0, 180, 0), 1f).onComplete += () =>
            {
                star[1].SetActive(true);
                star[1].GetComponent<Image>().color = successColor;
                star[1].transform.DOPunchRotation(new Vector3(0, 180, 0), 1f).onComplete += () =>
                {
                    star[2].SetActive(true);
                    star[2].GetComponent<Image>().color = successColor;
                    star[2].transform.DOPunchRotation(new Vector3(0, 180, 0), 1f);
                };
            };
        }
        else if (starCount == 2)
        {
            star[0].SetActive(true);
            star[0].GetComponent<Image>().color = successColor;
            star[0].transform.DOPunchRotation(new Vector3(0, 180, 0), 1f).onComplete += () =>
            {
                star[1].SetActive(true);
                star[1].GetComponent<Image>().color = successColor;
                star[1].transform.DOPunchRotation(new Vector3(0, 180, 0), 1f).onComplete += () =>
                {
                    star[2].SetActive(true);
                    star[2].transform.DOPunchRotation(new Vector3(0, 180, 0), 1f);
                };
            };
        }
        else
        {
            star[0].SetActive(true);
            star[0].GetComponent<Image>().color = successColor;
            star[0].transform.DOPunchRotation(new Vector3(0, 180, 0), 1f).onComplete += () =>
            {
                star[1].SetActive(true);
                star[1].transform.DOPunchRotation(new Vector3(0, 180, 0), 1f).onComplete += () =>
                {
                    star[2].SetActive(true);
                    star[2].transform.DOPunchRotation(new Vector3(0, 180, 0), 1f);
                };
            };
        }
    }
}