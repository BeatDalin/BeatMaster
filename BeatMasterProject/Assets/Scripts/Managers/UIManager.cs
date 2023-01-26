using System;
using System.Collections.Generic;
using DG.Tweening;
using SonicBloom.Koreo;
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
        DontDestroyOnLoad(gameObject);

        popUpStack = new Stack<GameObject>();
        
        InitCanvas(SceneManager.GetActiveScene().name);
    }

    private void Start()
    {
      
    }

    public void InitCanvas(string SceneName)
    {
        string canvasName = SceneName + "Canvas";

        canvas = GameObject.Find(canvasName);
    }

    public void OpenPopUp(GameObject panel)
    {
        panel.SetActive(true);
        panel.GetComponent<RectTransform>().localPosition = new Vector3(Screen.width, 0, 0);
        popUpStack.Push(panel);
    }
    public void ClosePopUp()
    {
        if (popUpStack.Count == 0)
        {
            return;
        }
        GameObject g = popUpStack.Pop();
        g.GetComponent<RectTransform>().DOLocalMove(new Vector3(Screen.width, 0, 0), 0.4f).onComplete += () =>
        {
            g.SetActive(false);
        };
    }
    
}
