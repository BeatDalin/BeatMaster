// using System;
// using System.Collections;
// using System.Collections.Generic;
// using DG.Tweening;
// using SonicBloom.Koreo;
// using SonicBloom.Koreo.Players;
// using UnityEngine;
//
// public class ButtonScale : MonoBehaviour
// {
//     [SerializeField] private GameObject[] _objects;
//     
//     private int objectIdx = 0;
//
//     private void Awake()
//     {
//         Koreographer.Instance.GetKoreographyAtIndex(0);
//     }
//     private void Start()
//     {
//         Koreographer.Instance.RegisterForEvents("MenuBGMTrack", ChangeScale);
//     }
//
//     void ChangeScale(KoreographyEvent evt)
//     {
//         if (objectIdx == _objects.Length)
//         {
//             objectIdx = 0;
//         }
//         if (objectIdx == 0)
//         {
//             _objects[objectIdx].GetComponent<DOTweenAnimation>().DOPlay();
//             objectIdx++;
//         }
//         else
//         {
//             _objects[objectIdx - 1].GetComponent<DOTweenAnimation>().DORewind();
//             _objects[objectIdx].GetComponent<DOTweenAnimation>().DOPlay();
//             objectIdx++;
//         }
//         
//     }
// }
