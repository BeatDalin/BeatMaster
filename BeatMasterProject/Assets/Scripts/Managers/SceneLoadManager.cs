using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoadManager : Singleton<SceneLoadManager>
{
    public SceneType Scene { get; private set; }
    
    public enum SceneType
    {
        MainMenu,
        LevelMenu,
        NormalGame,
        BossGame,
    }

    public void Init()
    {
        
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
        if (async.progress < 1f)
        {
            // 로딩 창 구현
            yield return null;
        }
        // 임의 대기 시간
        
        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(0.5f);
        // 로딩 창 꺼주기
        
    }


}
