using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SmfLite;
using SonicBloom.Koreo;
using Unity.VisualScripting;
using UnityEngine;

public class LoadMidi : MonoBehaviour
{
    public TextAsset sourceFile;
    MidiFileContainer song;
    MidiTrackSequencer sequencer;

    private AudioSource _audioSource;
    private int lastSampleTime;

    [Header("Short Check Track")] public KoreographyTrack track; // short note track

    public KoreographyTrack originalTrack;

    public List<int> resultList = new List<int>();
    private int idx;
    private bool isWaiting = false;

    private List<int> sampleTimeList = new List<int>();
    public List<int> originEventList = new List<int>();

    private bool isEndOfPlay;

    private int s_time;

    private float[] spectrum = new float[64];

    void ResetAndPlay()
    {
        _audioSource.Play();
        sequencer = new MidiTrackSequencer(song.tracks[0], song.division, 130f);
        ApplyMessages(sequencer.Start());
    }

    IEnumerator Start()
    {
        _audioSource = GetComponent<AudioSource>();
        song = MidiFileLoader.Load(sourceFile.bytes);
        //Debug.Log(song.tracks[0].ToString());
        yield return new WaitForSeconds(1.0f);
        ResetAndPlay();
    }

    void Update()
    {
        s_time = _audioSource.timeSamples;
        
        if (sequencer != null && sequencer.Playing)
        {
            ApplyMessages(sequencer.Advance(Time.deltaTime));
        }

        

        if (_audioSource.timeSamples == _audioSource.clip.samples)
        {
            Debug.Log("노래끝남");


            if (!isEndOfPlay)
            {
                isEndOfPlay = true;

                for (int i = 0; i < resultList.Count; i++)
                {
                    KoreographyEvent koreoEvent = new KoreographyEvent();
                    koreoEvent.Payload = originalTrack.GetAllEvents()[i].Payload;
                    koreoEvent.StartSample = resultList[i];
                    koreoEvent.EndSample = resultList[i];
                    track.AddEvent(koreoEvent);
                    Debug.Log("트랙찍기");
                }
            }
        }

        // if (isEndOfPlay)
        // {
        //     isEndOfPlay = false;
        //     Debug.Log("조정시작");
        //
        //     for (int j = 0; j < originalTrack.GetAllEvents().Count; j++)
        //     {
        //         int subSampletime = int.MaxValue;
        //         int resultSampletime = 0;
        //
        //         for (int i = 0; i < sampleTimeList.Count; i++)
        //         {
        //             if (subSampletime > originalTrack.GetAllEvents()[j].StartSample - sampleTimeList[i])
        //             {
        //                 subSampletime = originalTrack.GetAllEvents()[j].StartSample - sampleTimeList[i];
        //                 Debug.Log($"subsampleTime {subSampletime}");
        //                 resultSampletime = sampleTimeList[i];
        //                 Debug.Log($"resultSampletime {resultSampletime}");
        //             }
        //         }
        //
        //         KoreographyEvent koreoEvent = new KoreographyEvent();
        //         koreoEvent.Payload = originalTrack.GetAllEvents()[j].Payload;
        //         koreoEvent.StartSample = resultSampletime;
        //         koreoEvent.EndSample = resultSampletime;
        //         Debug.Log($"{j} 트랙등록");
        //         track.AddEvent(koreoEvent);
        //     }
        // }
    }

    IEnumerator CoWaitforSampleTime(int index)
    {
        int lastTimeSample = 0;
        List<int> sampleList = sampleTimeList;


        while (lastTimeSample <= originalTrack.GetAllEvents()[index].StartSample)
        {
            lastTimeSample = _audioSource.timeSamples;
            yield return null;
        }

        KoreographyEvent koreoEvent = new KoreographyEvent();
        koreoEvent.Payload = originalTrack.GetAllEvents()[index].Payload;
        koreoEvent.StartSample = lastTimeSample;
        koreoEvent.EndSample = lastTimeSample;
        Debug.Log($"{index} 트랙등록");
        track.AddEvent(koreoEvent);
    }

    void ApplyMessages(List<MidiEvent> messages)
    {
        // if (messages != null)
        // {
        //     //Debug.Log($"audio time {time}");
        //     Debug.Log($"track time {originalTrack.GetAllEvents()[idx].StartSample / 44100}");
        // }
        //
        // if (time - originalTrack.GetAllEvents()[idx].StartSample / 44100 >= 0)
        // {
        //     idx++;
        // }
        
        if (s_time - originalTrack.GetAllEvents()[idx].StartSample <= 0)
        {
            if (messages != null)
            {
                sampleTimeList.Add(s_time);
            }
        }
        else
        {
            idx++;
            resultList.Add(sampleTimeList[^1]);
        }

        
        


        // if (Mathf.Abs(_audioSource.timeSamples - originalTrack.GetAllEvents()[idx].StartSample) <= 10000)
        // {
        //     if (messages != null)
        //     {
        //         idx++;
        //         if (idx <= originalTrack.GetAllEvents().Count)
        //         {
        //             resultList.Add(_audioSource.timeSamples);
        //             // Debug.Log("노트 생성");
        //             // KoreographyEvent koreoEvent = new KoreographyEvent();
        //             // koreoEvent.Payload = originalTrack.GetAllEvents()[idx - 1].Payload;
        //             // koreoEvent.StartSample = _audioSource.timeSamples;
        //             // koreoEvent.EndSample = _audioSource.timeSamples;
        //             // track.AddEvent(koreoEvent);
        //         }
        //     }
        // }
        //         Debug.Log(_audioSource.timeSamples);
        //         if (idx < originalTrack.GetAllEvents().Count) //&& !isWaiting)
        //         {
        //             //idx++;
        //             StartCoroutine(CoWaitforSampleTime(idx - 1));
        //         }
        //
        //         //
        //         // //int minRange = _audioSource.timeSamples - 3000;
        //         int maxRange = _audioSource.timeSamples + 1000;
        //
        //
        //         if (originalTrack.GetAllEvents()[idx].StartSample <= maxRange)
        //         {
        //             //Debug.Log(_audioSource.timeSamples);
        //             if (idx < originalTrack.GetAllEvents().Count)
        //             {
        //                 Debug.Log("노트 생성");
        //                 KoreographyEvent koreoEvent = new KoreographyEvent();
        //                 koreoEvent.Payload = originalTrack.GetAllEvents()[idx].Payload;
        //                 koreoEvent.StartSample = _audioSource.timeSamples;
        //                 koreoEvent.EndSample = _audioSource.timeSamples;
        //                 track.AddEvent(koreoEvent);
        //                 idx++;
        //             }
        //         }
        // foreach (var m in messages)
        // {
        //     if ((m.status & 0xf0) == 0x90)
        //     {
        //         if (m.data1 == 0x29)
        //         {
        //             Debug.Log(m.data1);
        //         }
        //         
        //         // if (m.data1 == 0x24)
        //         // {
        //         //     if (lastSampleTime < _audioSource.timeSamples - 10000)
        //         //     {
        //         //         lastSampleTime = _audioSource.timeSamples;
        //         //         KoreographyEvent koreoEvent = new KoreographyEvent();
        //         //         koreoEvent.Payload = new IntPayload();
        //         //         koreoEvent.StartSample = _audioSource.timeSamples;
        //         //         koreoEvent.EndSample = _audioSource.timeSamples;
        //         //         track.AddEvent(koreoEvent);
        //         //         Debug.Log(_audioSource.timeSamples);
        //         //     }
        //         //     // _normalGame.isPress = true;
        //         //     // Debug.Log(_normalGame.isPress);
        //         // }
        //         //
        //         // if (m.data1 == 73)
        //         // {
        //         //     if (lastSampleTime < _audioSource.timeSamples - 10000)
        //         //     {
        //         //         lastSampleTime = _audioSource.timeSamples;
        //         //         KoreographyEvent koreoEvent = new KoreographyEvent();
        //         //         koreoEvent.Payload = new IntPayload();
        //         //         koreoEvent.StartSample = _audioSource.timeSamples;
        //         //         koreoEvent.EndSample = _audioSource.timeSamples;
        //         //         track.AddEvent(koreoEvent);
        //         //         Debug.Log(_audioSource.timeSamples);
        //         //     }
        //         // }
        //         // else if (m.data1 == 0x2a)
        //         // {
        //         //     KoreographyEvent koreoEvent = new KoreographyEvent();
        //         //     koreoEvent.Payload = new IntPayload();
        //         //     koreoEvent.StartSample = _audioSource.timeSamples;
        //         //     koreoEvent.EndSample = _audioSource.timeSamples;
        //         //     track.AddEvent(koreoEvent);
        //         //     Debug.Log(m.data1);
        //         // }
        //         // else if (m.data1 == 0x2e)
        //         // {
        //         //     KoreographyEvent koreoEvent = new KoreographyEvent();
        //         //     koreoEvent.Payload = new IntPayload();
        //         //     koreoEvent.StartSample = _audioSource.timeSamples;
        //         //     koreoEvent.EndSample = _audioSource.timeSamples;
        //         //     track.AddEvent(koreoEvent);
        //         //     Debug.Log(m.data1);
        //         // }
        //         // else if (m.data1 == 0x26 || m.data1 == 0x27 || m.data1 == 0x28)
        //         // {
        //         //     KoreographyEvent koreoEvent = new KoreographyEvent();
        //         //     koreoEvent.Payload = new IntPayload();
        //         //     koreoEvent.StartSample = _audioSource.timeSamples;
        //         //     koreoEvent.EndSample = _audioSource.timeSamples;
        //         //     track.AddEvent(koreoEvent);
        //         //     Debug.Log(m.data1);
        //         // }
        //     }
        //
        //     idx++;
        // }
        // }
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(0, 0, 300, 50), "Reset"))
        {
            ResetAndPlay();
        }
    }
}