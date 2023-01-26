using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    public string bgmName;

    public void Start()
    {
        Time.timeScale = 0;
    }

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

        if (Input.GetKeyDown(KeyCode.Space))
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
