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
    public bool isLoaded = false;
    [Header("Scene Transition Effect")] 
    private GameObject _loadingCanvas;
    private Canvas _canvas;
    [SerializeField] private Image _loadImage;
    private static readonly int Cutoff = Shader.PropertyToID("_Cutoff");
    private float _showBackground = 1.2f;
    private float _hideBackground = -0.1f;
    private float _transitionSpd = 2f;

    public enum SceneType
    {
        Title,
        LevelSelect,
        Level1,
        Level2,
        Level3,
        Level4,
        Level1MonsterTest
    }

    public override void Init()
    {
        DontDestroyOnLoad(this);
        if (_loadImage == null)
        {
            _loadingCanvas = Instantiate(Resources.Load<GameObject>("UI/SceneTransitionCanvas"), transform);
            _loadImage = _loadingCanvas.GetComponentInChildren<Image>();
            _loadImage.material.SetFloat(Cutoff, _hideBackground); // filled
        }
        _loadImage.gameObject.SetActive(false);
        _canvas = _loadingCanvas.GetComponent<Canvas>();
        _canvas.worldCamera = Camera.main;
    }

    public void LoadLevelAsync(SceneType sceneType)
    {
        Scene = sceneType;
        SoundManager.instance.ChangeKoreo(Scene);
        StartCoroutine(CoSceneTransition());
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
    private IEnumerator CoSceneTransition()
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
        StartCoroutine(CoLoadLevelAsync());
    }

    /// <summary>
    /// Once next scene has been loaded, show transition effect to empty the hiding panel. 
    /// </summary>
    /// <returns>WaitForEndOfFrame()</returns>
    public IEnumerator CoSceneEnter()
    {
        isLoaded = true;
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
        isLoaded = false;
    }

    public bool GetTransitionEnd()
    {
        if (_loadImage.material.GetFloat(Cutoff) >= _showBackground)
        {
            return true;
        }

        return false;
    }
}
