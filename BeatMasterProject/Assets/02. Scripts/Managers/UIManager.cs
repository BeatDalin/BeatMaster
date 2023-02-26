using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public Stack<GameObject> popUpStack;
    public GameObject canvas;
    private void Awake()
    {
        if (instance != null)
        {
            return;
        }

        instance = this;

        popUpStack = new Stack<GameObject>();
        
        InitCanvas(SceneManager.GetActiveScene().name);
    }

    public void InitCanvas(string SceneName)
    {
        string canvasName = SceneName + "Canvas";

        canvas = GameObject.Find(canvasName);
    }

    public void OpenPanel(GameObject panel)
    {
        if (!panel.activeSelf)
        {
            SoundManager.instance.PlaySFX("Touch");
            panel.SetActive(true);
            panel.GetComponent<RectTransform>().localPosition = new Vector3(Screen.width, 0, 0);
        }
        //popUpStack.Push(panel);
    }
    public void ClosePanel(GameObject panel)
    {
        // if (popUpStack.Count == 0)
        // {
        //     return;
        // }
        //GameObject g = popUpStack.Pop();
        SoundManager.instance.PlaySFX("Touch");
        panel.GetComponent<RectTransform>().DOLocalMove(new Vector3(Screen.width, 0, 0), 0.6f).onComplete += () =>
        {
            panel.SetActive(false);
        };
    }
    
}
