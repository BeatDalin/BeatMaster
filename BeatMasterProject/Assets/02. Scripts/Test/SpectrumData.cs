using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SonicBloom.Koreo;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public struct Music
{
    public int sample;
    public float spectrumData;
}

public class SpectrumData : MonoBehaviour
{
    [SerializeField] private int spectrumSize;
    [SerializeField] private AudioSource _audioSource;
    [SerializeField] private float targetTime;
    [SerializeField] private KoreographyTrack track;
    [SerializeField] private List<Music> trackSampleTime;
    [SerializeField] private int offset;

    private float[] spectrum;
    private float currentTime = 0f;
    private float timer = 0f;
    private bool isPlayed = false;

    private float averageSpectrumSize;

    private List<Music> collectSpectrumList;

    private void Start()
    {
        spectrum = new float[spectrumSize];
        
        collectSpectrumList = new List<Music>();
        trackSampleTime = new List<Music>();

        Debug.Log(GetAverageSpectrumSize());
        
        _audioSource.PlayDelayed(2f);
    }

    private float GetAverageSpectrumSize()
    {
        float[] samples = new float[_audioSource.clip.samples * _audioSource.clip.channels];
        _audioSource.clip.GetData(samples, 0);

        float maxSize = float.MinValue;

        float totalSize = 0f;
        int idx = 0;

        for (int i = 0; i < samples.Length; i++)
        {
            totalSize += Mathf.Abs(samples[i]);
        }
        
        return totalSize / samples.Length - 1;
    }

    private void Update()
    {

        if (!isPlayed)
        {
            currentTime += Time.deltaTime;
        }

        if (!isPlayed && currentTime >= 2f)
        {
            isPlayed = true;
            _audioSource.Play();
            currentTime = 0f;
        }
        
        if (_audioSource.isPlaying)
        {
            currentTime += Time.deltaTime;
        }

        if (currentTime > targetTime) //데이터 분석해서 큰값만 트랙에 생성
        {
            _audioSource.Pause();
            
            collectSpectrumList.Sort(((music, music1) => //내림차순 정렬
                music1.spectrumData.CompareTo(music.spectrumData)));

            if (trackSampleTime.Count > 0)
            {
                if ((Mathf.Abs(trackSampleTime[^1].sample - collectSpectrumList[0].sample) >= offset) 
                    && (Mathf.Abs(averageSpectrumSize) < Mathf.Abs(collectSpectrumList[0].spectrumData))) 
                { // offset 이하로는 생기지 않도록(겹침 방지), 평균보다 작으면 생성 x
                    KoreographyEvent koreographyEvent = new KoreographyEvent();
                    koreographyEvent.Payload = new IntPayload();
                    koreographyEvent.StartSample = collectSpectrumList[0].sample;
                    koreographyEvent.EndSample = collectSpectrumList[0].sample;
                    
                    Music music = new Music();
                    music.spectrumData = collectSpectrumList[0].spectrumData;
                    music.sample = collectSpectrumList[0].sample;
                
                    trackSampleTime.Add(music);
                    track.AddEvent(koreographyEvent);
                }
                else
                {
                    Debug.Log("건너뜀");
                    currentTime = 0f;
                    collectSpectrumList.Clear();
            
                    _audioSource.Play();
                    return;
                }
            }
            else
            {
                KoreographyEvent koreographyEvent = new KoreographyEvent();
                koreographyEvent.Payload = new IntPayload();
                koreographyEvent.StartSample = collectSpectrumList[0].sample;
                koreographyEvent.EndSample = collectSpectrumList[0].sample;
                
                Music music = new Music();
                music.spectrumData = collectSpectrumList[0].spectrumData;
                music.sample = collectSpectrumList[0].sample;
                
                trackSampleTime.Add(music);
                track.AddEvent(koreographyEvent);
            }
            
            currentTime = 0f;
            collectSpectrumList.Clear();
            
            _audioSource.Play();
        }
        else //스펙트럼 데이터 수집
        {
            spectrum = _audioSource.GetSpectrumData(spectrumSize, 0, FFTWindow.Hamming);
            
            Music music = new Music();

            music.sample = _audioSource.timeSamples;
            music.spectrumData = spectrum.Max();

            collectSpectrumList.Add(music);
        }
    }
}