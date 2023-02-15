using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using DG.Tweening;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
    private RectTransform _rectTransform;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    public void MoveCoin(Vector2 movePos, GameObject coinParent)
    {
        transform.SetParent(coinParent.transform);

        _rectTransform.anchorMin = new Vector2(0, 1);
        _rectTransform.anchorMax = new Vector2(0, 1);

        transform.DOLocalMove(movePos, 1f).onComplete += () =>
        {
            Destroy(gameObject);
        };
    }
}
