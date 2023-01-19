using SonicBloom.Koreo;
using SonicBloom.Koreo.Players;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum BeatResult
{
    Fail,
    Perfect,
    Fast,
    Slow,
}
public abstract class Game : MonoBehaviour
{
    protected Koreography playingKoreo;
    protected SimpleMusicPlayer musicPlayer;

    /// <summary>
    /// To use sample time of event tracks, this should be written first. Used for KoreographyEventCallbackWithTime.
    /// </summary>
    /// <param name="koreoEvent">Each KoreographyEvent</param>
    /// <param name="sampleTime">The current time for this KoreographyEvent.</param>
    /// <param name="sampleDelta">The number of samples that were played back since the previous frame. You can get the previous frame’s sampleTimewith(sampleTime-sampleDelta).</param>
    /// <param name="deltaSlice">Extra timing information required for simulation stability when the callback is called multiple times in a frame.</param>
    public delegate void KoreographyEventCallbackWithTime(KoreographyEvent koreoEvent, int sampleTime, int sampleDelta, DeltaSlice deltaSlice);


    [Header("Result Check")]
    public BeatResult[] longResult;
    public BeatResult[] shortResult;

    // long notes
    protected int longIdx = 0;
    protected bool isLongPressed = false;
    protected bool isLongKeyCorrect = false;
    // short notes
    protected int shortIdx = 0;
    //protected bool isShortKeyPressed = false; // To prevent double check...
    protected bool isShortKeyCorrect = false;


    protected virtual void Awake()
    {
        playingKoreo = Koreographer.Instance.GetKoreographyAtIndex(0);
        musicPlayer = FindObjectOfType<SimpleMusicPlayer>();
    }

    protected virtual void Start()
    {
        StartWithDelay();
    }

    protected virtual void Init()
    {
        musicPlayer.LoadSong(playingKoreo, 0, false);
        longResult = new BeatResult[playingKoreo.GetTrackByID("LongJump").GetAllEvents().Count];
        shortResult = new BeatResult[playingKoreo.GetTrackByID("Jump").GetAllEvents().Count];
        longIdx = 0;
        shortIdx = 0;
        isLongPressed = false;
        isLongKeyCorrect = false;
    }

    public abstract void CheckBeatResult(BeatResult[] resultArr, BeatResult tempResult, int idx, bool isKeyCorrect);

    protected void StartWithDelay()
    {
        StartCoroutine(CoStartWithDelay());
    }
    IEnumerator CoStartWithDelay()
    {
        // UI 업데이트 할 거라면 이쪽에서 호출 가능
        Debug.Log("wait for playing...");
        yield return new WaitForSeconds(3);
        Debug.Log("Start...!");

        musicPlayer.Play();
    }

    protected bool CheckFinish()
    {
        // If index becomes the length of Arrays (or length -1), the game has been ended.
        if (shortIdx >= shortResult.Length - 1 && longIdx >= longResult.Length - 1)
        {
            return true;
        }
        return false;
    }
}
