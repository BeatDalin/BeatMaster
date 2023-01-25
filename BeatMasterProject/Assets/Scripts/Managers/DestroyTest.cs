using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyTest : MonoBehaviour
{
    void Start()
    {
        StartCoroutine(LoadTest());
    }

    private IEnumerator LoadTest()
    {
        yield return new WaitForSeconds(1f);
        SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.BossGame);
    }
    void Update()
    {
        
    }
}
