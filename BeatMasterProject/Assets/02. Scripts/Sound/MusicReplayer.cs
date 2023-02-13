using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicReplayer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(CoReplayMusic());
    }

    /// <summary>
    /// Start playing background music after background music has been stopped.
    /// </summary>
    /// <returns>WaitUntil(() => !SoundManager.instance.musicPlayer.IsPlaying)</returns>
    private IEnumerator CoReplayMusic()
    {
        yield return new WaitUntil(() => !SoundManager.instance.musicPlayer.IsPlaying);
        yield return new WaitForSeconds(5f); // Give 5 seconds break.
        SoundManager.instance.musicPlayer.Play();
        StartCoroutine(CoReplayMusic());
    }
}
