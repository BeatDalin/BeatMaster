using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoadManager : Singleton<SceneLoadManager>
{
    public SceneType Scene { get; private set; }
    [SerializeField] private float _loadingTime = 2f;
    private float _fadeMod = 1f;
    float _timer = 0f;

    private GameObject _loadingCanvas;
    private List<Image> _loadingCanvasImages = new List<Image>();

    public enum SceneType
    {
        MainMenu,
        LevelMenu,
        NormalGame,
        BossGame,
    }

    public void Init()
    { 
        _loadingCanvas = Instantiate(ResourceManager.Instance.LoadingUI, transform);
        Debug.Log(_loadingCanvas);
        _loadingCanvas.SetActive(false);
        _loadingCanvasImages.AddRange(_loadingCanvas.GetComponentsInChildren<Image>()); 
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
        // 다음 씬이 먼저 보이는 현상이 있음 그냥 FadeOut은 바로 보여주는 게 나을 수도..?

        FadeOut();
        
        AsyncOperation async = SceneManager.LoadSceneAsync(Scene.ToString());
        
        yield return new WaitForSeconds(_loadingTime);
        
        if (async.progress < 1f)
        {
            // 로딩 창 구현
            yield return null;
        }
        // 임의 대기 시간
        
        
        StartCoroutine(CoFadeIn());
        // 로딩 창 꺼주기
        
    }

    private void FadeOut()
    {
        _timer = 1f;
        foreach (Image image in _loadingCanvasImages)
        {
            Color color = image.color;
            color.a = _timer;
            image.color = color;
        }
        
        _loadingCanvas.SetActive(true);
        
        /*while (_timer < 1f)
        {
            foreach (Image image in _loadingCanvasImages)
            {
                Color color = image.color;
                color.a = _timer;
                image.color = color;
            }

            _timer += Time.deltaTime * _fadeMod;
            yield return new WaitForEndOfFrame();
        }*/
        
    }
    
    private IEnumerator CoFadeIn()
    {
        while (_timer > 0f)
        {
            foreach (Image image in _loadingCanvasImages)
            {
                Color color = image.color;
                color.a = _timer;
                image.color = color;
            }
            
            _timer -= Time.deltaTime * _fadeMod;
            yield return new WaitForEndOfFrame();
        }
        
        _loadingCanvas.SetActive(false);
    }
    
    
}
