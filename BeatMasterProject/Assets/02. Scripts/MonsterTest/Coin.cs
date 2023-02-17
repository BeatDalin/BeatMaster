using System;
using DG.Tweening;
using UnityEditor;
using UnityEngine;

public class Coin : MonoBehaviour
{
    private RectTransform _rectTransform;
    private MonsterPooling _monsterPooling;
    

    private void Start()
    {
        _monsterPooling = FindObjectOfType<MonsterPooling>();
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

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.tag.Equals("Player"))
        {
            MoveCoin(_monsterPooling._coinScreenPos, _monsterPooling.coinParent);
        }
    }
}
