using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreTitle : MonoBehaviour
{
    [SerializeField] private CanvasGroup _loadingPanelGroup;

    void Start()
    {
        StartCoroutine(CoWaitStart());
    }

    private IEnumerator CoWaitStart()
    {
        yield return new WaitUntil(() => _loadingPanelGroup.alpha <= 0);
        SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.Title);
        yield return new WaitForSeconds(1f);
        SoundManager.instance.musicPlayer.Play();
    }
}
