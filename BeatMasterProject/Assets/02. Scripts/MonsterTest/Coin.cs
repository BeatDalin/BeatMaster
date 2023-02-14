using System;
using System.Collections;
using System.Collections.Generic;
using Cinemachine.Utility;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class Coin : MonoBehaviour
{
    public void MoveCoin(Vector2 movePos)
    {
        transform.DOLocalMove(movePos, 1f).onComplete += () =>
        {
            Destroy(gameObject);
        };
    }
}
