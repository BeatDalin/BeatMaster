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
        
    }

    private enum GameObjects
    {
        
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
        GetButton((int)Buttons.MenuBtn).gameObject.AddUIEvent(StartGame);

    }

    private void StartGame(PointerEventData data)
    {
        SoundManager.instance.PlaySFX("Touch");
        SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.LevelSelect);
    }
    //private void 
    
}
