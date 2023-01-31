using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossGameUI : GameUI
{
    [Header("Base UI for Boss Game")]
    [SerializeField] private Text _deathText;

    private void Awake()
    {
        game = FindObjectOfType<BossGame>();
        InitUI();
    }

    public override void UpdateText(TextType type, int number)
    {
        switch (type)
        {
            case TextType.Death:
                _deathText.text = number.ToString();
                break;
            case TextType.Time:
                timeCount.text = number.ToString();
                break;
        }
    }
}
