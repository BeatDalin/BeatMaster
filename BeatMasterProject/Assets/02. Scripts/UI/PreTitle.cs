using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PreTitle : MonoBehaviour
{
    [SerializeField] private CanvasGroup _loadingPanelGroup;
    [SerializeField] private DebuggingMode _debugMode;
    void Start()
    {
        if (Debug.isDebugBuild)
        {
            _debugMode.enabled = true;
        }
        StartCoroutine(CoWaitStart());
    }

    private IEnumerator CoWaitStart()
    {
        yield return new WaitUntil(() => _loadingPanelGroup.alpha <= 0.3f);
        SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.Title);
        yield return new WaitForSeconds(1f);
        SoundManager.instance.musicPlayer.Play();
    }
}
