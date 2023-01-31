using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    public string bgmName;

    private void Start()
    {
        Time.timeScale = 0;
    }

    private void Update()
    {
        // if (Input.GetKeyDown(KeyCode.F))
        // {
        //     SoundManager.instance.PlaySFX("Jump");
        // }
        //
        // if (Input.GetKeyDown(KeyCode.J))
        // {
        //     SoundManager.instance.PlaySFX("Attack");
        // }

        if (Input.GetKeyDown(KeyCode.Tab))
        {
            Time.timeScale = 1;
            SoundManager.instance.PlayBGM(bgmName);
        }
    }
    

    // public void PlaySFXButton()
    // {
    //     SoundManager.instance.PlaySFX("Jump");
    // }

}
