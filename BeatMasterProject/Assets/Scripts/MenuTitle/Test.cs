using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using Cinemachine.Utility;
using DG.Tweening;
using SonicBloom.Koreo;
using Unity.VisualScripting;
using UnityEditor.U2D.Path;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class Test : MonoBehaviour
{
    [SerializeField] private Transform end;
    [SerializeField] private Image outlinecolor;
    [SerializeField] private Color originColor;
    [SerializeField] private Color _color;
    [SerializeField] private UIOutline _outline;

    [SerializeField] private GameObject perfect;
    [SerializeField] private GameObject fast;
    [SerializeField] private GameObject slow;
    [SerializeField] private GameObject fail;
   
    // Start is called before the first frame update
    void Start()
    {
        // Vector3 start = transform.position;
        // middle = new Vector3(Math.Abs(end.position.x + start.x) / 2, Math.Abs(end.position.y + start.y), 0);
        //
        // transform.DOPath(new[]
        //     {
        //         middle, start + Vector3.up, middle + Vector3.left * 2, end.position, middle + Vector3.right * 2, end.position + Vector3.up
        //     }
        //     , 1f, PathType.CubicBezier);

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            perfect.SetActive(true);
            fast.SetActive(false);
            slow.SetActive(false);
            fail.SetActive(false);
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            perfect.SetActive(false);
            fast.SetActive(false);
            slow.SetActive(false);
            fail.SetActive(true);
            fail.GetComponent<CanvasGroup>().DOFade(1, 0.2f).onComplete += () =>
            {
                fail.GetComponent<CanvasGroup>().DOFade(0f, 0.2f).onComplete += () =>
                {
                    fail.SetActive(false);
                };
            };
        }
        
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            perfect.SetActive(false);
            fast.SetActive(true);
            slow.SetActive(false);
            fail.SetActive(false);
        }
        
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            perfect.SetActive(false);
            fast.SetActive(false);
            slow.SetActive(true);
            fail.SetActive(false);
        }
    }
}
