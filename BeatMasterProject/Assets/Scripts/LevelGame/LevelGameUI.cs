using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class LevelGameUI : GameUI
{
    [Header("Game")]
    public NormalGame levelGame;
    [Header("Base UI for Level Game")]
    [SerializeField] private Text _itemText;
    [SerializeField] private Text _deathText;
    
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

    public override void InitUI()
    {
        _itemText.text = "0";
        timePanel.SetActive(true);
        finalPanel.SetActive(false);
        pausePanel.SetActive(false);
        settingsPanel.SetActive(false);
        // Button Events
        continueBtn.onClick.AddListener(() =>
        {
            UIManager.instance.ClosePopUp();
            levelGame.ContinueGame();
        });
        goLevelMenuBtn.onClick.AddListener(() => SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.MenuLevelSelect));
        //settings
        goSettingsBtn.onClick.AddListener(() => UIManager.instance.OpenPopUp(settingsPanel));
        settingsCloseBtn.onClick.AddListener(() => { UIManager.instance.ClosePopUp(); });
    }

    public override void UpdateText(TextType type, int number)
    {
        switch (type)
        {
            case TextType.Item:
                _itemText.text = number.ToString();
                break;
            case TextType.Death:
                _deathText.text = number.ToString();
                break;
            case TextType.Time:
                timeCount.text = number.ToString();
                break;
        }
    }

    protected override void OpenPause()
    {
        UIManager.instance.OpenPopUp(pausePanel);
        levelGame.PauseGame();
    }
}
