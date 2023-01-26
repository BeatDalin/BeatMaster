using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    public string bgmName = "";
    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.F))
        {
            SoundManager.instance.PlaySFX("Jump");
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            SoundManager.instance.PlaySFX("Attack");
        }
    }

    public void PlayBGMButton()
    {
        SoundManager.instance.PlayBGM(bgmName);
    }

    // public void PlaySFXButton()
    // {
    //     SoundManager.instance.PlaySFX("Jump");
    // }

}
