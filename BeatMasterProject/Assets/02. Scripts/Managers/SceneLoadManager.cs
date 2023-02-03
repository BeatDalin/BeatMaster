using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : Singleton<SceneLoadManager>
{
    public SceneType Scene { get; private set; }
    
    [Header("Scene Load")] 
    [SerializeField] private float _loadingTime = 2f;
    
    [Header("Scene Transition Effect")] 
    private GameObject _loadingCanvas;
    private Canvas _canvas;
    [SerializeField] private Image _loadImage;
    private bool _shouldHide = false;
    private static readonly int Cutoff = Shader.PropertyToID("_Cutoff");
    private float _showBackground = 1.2f;
    private float _hideBackground = -0.1f;
    private float _transitionSpd = 2f;

    public enum SceneType
    {
        MenuTitle,
        MenuLevelSelect,
        LevelGame,
        BossGame,
    }

    public void Init()
    {
        if (_loadImage == null)
        {
            _loadingCanvas = Instantiate(Resources.Load<GameObject>("UI/SceneTransitionCanvas"), transform);
            _loadImage = _loadingCanvas.GetComponentInChildren<Image>();
        }

        _canvas = _loadingCanvas.GetComponent<Canvas>();
        _canvas.worldCamera = Camera.main;

        // Scene Transition
        _loadImage.material.SetFloat(Cutoff, _hideBackground); // filled
        Debug.Log(_loadImage.material.GetFloat(Cutoff));
        StartCoroutine(CoSceneEnter());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.T))
        {
            StartCoroutine(CoSceneTransition());
        }
    }

    public void LoadLevelAsync(SceneType sceneType)
    {
        Scene = sceneType;
        StartCoroutine(CoLoadLevelAsync());
    }

    public void RestartGame()
    {
        StartCoroutine(CoLoadLevelAsync());
    }

    private IEnumerator CoLoadLevelAsync()
    {
        AsyncOperation async = SceneManager.LoadSceneAsync(Scene.ToString());
        yield return new WaitForSeconds(_loadingTime);
        
        if (async.progress < 1f)
        {
            // loading bar if you want
            yield return null;
        }
        StartCoroutine(CoSceneEnter());
    }
    
    /// <summary>
    /// Start from unfilled image and show transition effect. After showing transition effect, start loading next scene.
    /// </summary>
    /// <param name="sceneType">Scene to move on.</param>
    /// <returns>CoLoadLevelAsync()</returns>
    public IEnumerator CoSceneTransition(SceneType sceneType = default)
    {
        _loadImage.gameObject.SetActive(true);
        _loadImage.material.SetFloat(Cutoff, _showBackground); // unfilled
        // Fill and hide background
        while (_loadImage.material.GetFloat(Cutoff) > _hideBackground)
        {
            _loadImage.material.SetFloat(Cutoff,
                Mathf.MoveTowards(_loadImage.material.GetFloat(Cutoff), _hideBackground, _transitionSpd * Time.deltaTime));
            yield return new WaitForEndOfFrame();
        }
        Scene = sceneType;
        StartCoroutine(CoLoadLevelAsync());
    }

    /// <summary>
    /// Once next scene has been loaded, show transition effect to empty the hiding panel. 
    /// </summary>
    /// <returns>WaitForEndOfFrame()</returns>
    public IEnumerator CoSceneEnter()
    {
        _canvas.worldCamera = Camera.main;
        _loadImage.material.SetFloat(Cutoff, _hideBackground); // filled
        _loadImage.gameObject.SetActive(true);
        // Empty and show background
        while (_loadImage.material.GetFloat(Cutoff) < _showBackground)
        {
            _loadImage.material.SetFloat(Cutoff, 
                Mathf.MoveTowards(_loadImage.material.GetFloat(Cutoff), _showBackground, _transitionSpd * Time.deltaTime));
            yield return new WaitForEndOfFrame();
        }
        _loadImage.gameObject.SetActive(false);
    }
    
    
    
    // private float _fadeMod = 1f;
    // private float _timer = 0f;
    // private void FadeOut()
    // {
    //     _timer = 1f;
    //     foreach (Image image in _loadingCanvasImages)
    //     {
    //         Color color = image.color;
    //         color.a = _timer;
    //         image.color = color;
    //     }
    //     
    //     _loadingCanvas.SetActive(true);
    //     
    // }
    //
    // private IEnumerator CoFadeIn()
    // {
    //     while (_timer > 0f)
    //     {
    //         foreach (Image image in _loadingCanvasImages)
    //         {
    //             Color color = image.color;
    //             color.a = _timer;
    //             image.color = color;
    //         }
    //         
    //         _timer -= Time.deltaTime * _fadeMod;
    //         yield return new WaitForEndOfFrame();
    //     }
    //     
    //     _loadingCanvas.SetActive(false);
    // }
    
    
}
