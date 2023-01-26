using System;
using System.Collections;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.Serialization;
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
    [Header("Time Count UI")]
    [SerializeField] public GameObject timePanel;
    [SerializeField] public Text timeCount;
    [Header("Pause UI")]
    [SerializeField] private GameObject _pausePanel;
    [SerializeField] private Button _continueBtn;
    [SerializeField] private Button _restartBtn;
    [SerializeField] private Button _goSettingsBtn;
    [SerializeField] private Button _goMenuBtn;
    [Header("Settings UI")]
    [SerializeField] private GameObject _settingsPanel;
    [SerializeField] private Button _settingsCloseBtn;

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
    
    public void ShowFinalResult(int[] finalResultSummary, int total)
    {
        _finalFast.text = $"{finalResultSummary[1]}/{total}";
        _finalPerfect.text = $"{finalResultSummary[2]}/{total}";
        _finalSlow.text = $"{finalResultSummary[3]}/{total}";
        _finalPanel.SetActive(true);
    }
    
    private void OpenPause()
    {
        UIManager.instance.OpenPopUp(_pausePanel);
        levelGame.PauseGame();
    }

    
}
