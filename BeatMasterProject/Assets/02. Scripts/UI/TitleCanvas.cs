using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class TitleCanvas : UI_Base
{
    private enum Buttons
    {
        PlayBtn,
        MenuBtn,
        StoreBtn,
        GPGSBtn,
        SettingBtn,
        AnnounceBtn,
        GameQuitBtn,
        
    }

    private enum GameObjects
    {
        MenuGroup,
        StorePanel,
        GpgsPanel,
        SettingPanel,
        AnnouncePanel,
        QuitPanel,
    }
    
    private void Start()
    {
        Binding();
        GetButtons();
    }

    private void Binding()
    {
        Bind<Button>(typeof(Buttons));
        Bind<GameObject>(typeof(GameObjects));
    }

    private void GetButtons()
    {
        GetButton((int)Buttons.PlayBtn).gameObject.AddUIEvent(StartGame);
        GetButton((int)Buttons.MenuBtn).gameObject.AddUIEvent(Open);
        GetButton((int)Buttons.StoreBtn).gameObject.AddUIEvent(StartGame);
        GetButton((int)Buttons.GPGSBtn).gameObject.AddUIEvent(StartGame);
        GetButton((int)Buttons.SettingBtn).gameObject.AddUIEvent(StartGame);
        GetButton((int)Buttons.AnnounceBtn).gameObject.AddUIEvent(StartGame);
        GetButton((int)Buttons.GameQuitBtn).gameObject.AddUIEvent(StartGame);
        
    }

    private void StartGame(PointerEventData data)
    {
        SoundManager.instance.PlaySFX("Touch");
        SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.LevelSelect);
    }

    private void Open(PointerEventData data)
    {
        //UIManager.instance.
    }
    
}
