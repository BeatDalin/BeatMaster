using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;

public class StarMovement : MonoBehaviour
{
    [SerializeField] private GameObject[] stars;
    [SerializeField] private RectTransform target;
    [SerializeField] private GameObject startPos;
    private void Start()
    {
        float delay = 0f;

        for (int i = 0; i < 30; i++)
        {
            GameObject g = Instantiate(stars[0]);
            g.SetActive(true);
            g.transform.SetParent(startPos.transform);
            g.GetComponent<RectTransform>().localPosition = new Vector3(0, 0, 0);

            g.transform.localScale = new Vector3(0, 0, 0);
            
            g.transform.DOScale(new Vector3(1, 1, 1), 1f).SetDelay(delay).SetEase(Ease.OutBack);
            
            g.GetComponent<RectTransform>()
                .DOAnchorPos(new Vector2(target.anchoredPosition.x, target.anchoredPosition.y), 1f)
                .SetDelay(delay + 0.5f)
                .SetEase(Ease.InBack).onComplete += () =>
            {
                g.transform.DOScale(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.Flash);
            };

            delay += 0.2f;
        }
        
        // foreach (var star in stars)
        // {
        //     star.SetActive(true);
        //     star.transform.DORotate(new Vector3(0, 0, 0), 0.5f).SetDelay(delay).SetEase(Ease.Flash);
        //
        //     star.transform.DOScale(new Vector3(1, 1, 1), 1f).SetDelay(delay).SetEase(Ease.OutBack);
        //     
        //     star.GetComponent<RectTransform>()
        //         .DOAnchorPos(new Vector2(target.anchoredPosition.x, target.anchoredPosition.y), 1f)
        //         .SetDelay(delay + 0.5f)
        //         .SetEase(Ease.InBack).onComplete += () =>
        //     {
        //         star.transform.DOScale(new Vector3(0, 0, 0), 0.5f).SetEase(Ease.Flash);
        //     };
        //
        //     delay += 0.2f;
        // }
    }
}
