 using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelGameUI : GameUI
{
    [Header("Base UI for Level Game")]
    [SerializeField] private Text _itemText;
    [SerializeField] private Text _deathText;
    
    private void Awake()
    {
        game = FindObjectOfType<NormalGame>();
        InitUI();
    }

    public override void InitUI()
    {
        base.InitUI();
        _itemText.text = "0";
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
}
