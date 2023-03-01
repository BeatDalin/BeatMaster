using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SonicBloom.Koreo;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Serialization;

public struct Music
{
    public int sample;
    public float spectrumData;
}

public class SpectrumData : MonoBehaviour
{
    [FormerlySerializedAs("spectrumSize")] [SerializeField] private int _spectrumSize;
    [SerializeField] private AudioSource _audioSource;
    [FormerlySerializedAs("targetTime")] [SerializeField] private float _targetTime;
    [FormerlySerializedAs("track")] [SerializeField] private KoreographyTrack _track;
    [SerializeField] private List<Music> _trackSampleTime;
    [FormerlySerializedAs("offset")] [SerializeField] private int _offset;

    private float[] _spectrum;
    private float _currentTime = 0f;
    private float _timer = 0f;
    private bool _isPlayed = false;

    private float _averageSpectrumSize;

    private List<Music> _collectSpectrumList;

    private void Start()
    {
        _spectrum = new float[_spectrumSize];
        
        _collectSpectrumList = new List<Music>();
        _trackSampleTime = new List<Music>();

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
        
        return totalSize / samples.Length;
    }

    private void Update()
    {

        if (!_isPlayed)
        {
            _currentTime += Time.deltaTime;
        }

        if (!_isPlayed && _currentTime >= 2f)
        {
            _isPlayed = true;
            _audioSource.Play();
            _currentTime = 0f;
        }
        
        if (_audioSource.isPlaying)
        {
            _currentTime += Time.deltaTime;
        }

        if (_currentTime > _targetTime) //데이터 분석해서 큰값만 트랙에 생성
        {
            _audioSource.Pause();
            
            _collectSpectrumList.Sort(((music, music1) => //내림차순 정렬
                music1.spectrumData.CompareTo(music.spectrumData)));

            if (_trackSampleTime.Count > 0)
            {
                if (_collectSpectrumList[0].sample - _trackSampleTime[^1].sample > _offset
                    && (Mathf.Abs(_averageSpectrumSize) < Mathf.Abs(_collectSpectrumList[0].spectrumData)))
                { // offset 이하로는 생기지 않도록(겹침 방지), 평균보다 작으면 생성 x
                    KoreographyEvent koreographyEvent = new KoreographyEvent();
                    koreographyEvent.Payload = new IntPayload();
                    koreographyEvent.StartSample = _collectSpectrumList[0].sample;
                    koreographyEvent.EndSample = _collectSpectrumList[0].sample;
                    
                    Music music = new Music();
                    music.spectrumData = _collectSpectrumList[0].spectrumData;
                    music.sample = _collectSpectrumList[0].sample;
                
                    _trackSampleTime.Add(music);
                    _track.AddEvent(koreographyEvent);
                }
                else
                {
                    _currentTime = 0f;
                    _collectSpectrumList.Clear();
            
                    _audioSource.Play();
                    return;
                }
            }
            else
            {
                KoreographyEvent koreographyEvent = new KoreographyEvent();
                koreographyEvent.Payload = new IntPayload();
                koreographyEvent.StartSample = _collectSpectrumList[0].sample;
                koreographyEvent.EndSample = _collectSpectrumList[0].sample;
                
                Music music = new Music();
                music.spectrumData = _collectSpectrumList[0].spectrumData;
                music.sample = _collectSpectrumList[0].sample;
                
                _trackSampleTime.Add(music);
                _track.AddEvent(koreographyEvent);
            }
            
            _currentTime = 0f;
            _collectSpectrumList.Clear();
            
            _audioSource.Play();
        }
        else //스펙트럼 데이터 수집
        {
            _spectrum = _audioSource.GetSpectrumData(_spectrumSize, 0, FFTWindow.Hamming);
            
            Music music = new Music();

            music.sample = _audioSource.timeSamples;
            music.spectrumData = _spectrum.Max();

            _collectSpectrumList.Add(music);
        }
    }
}