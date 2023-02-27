using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrollViewExtension : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    [SerializeField] private Transform _parent;

    private CustomScrollRect _scrollRect;
    private Vector2 _nowVector2 = new Vector2(0, 0);
    private int _preVector2X = 0;

    private void OnEnable()
    {
        _scrollRect.SetHorizontalNormalizedPosition(0);
    }
    private void Awake()
    {
        _scrollRect = GetComponent<CustomScrollRect>();
    }

    public void RoundValue(Vector2 Pos)
    {
        float absIndex = 1f / ((float)_parent.childCount - 1f);
        _nowVector2.x = Mathf.Clamp((Mathf.Round(Pos.x / absIndex) * absIndex), 0, 1);
    }

    public void SetActiveMode(int nowVector2, int preVector2)
    {
        _preVector2X = nowVector2;
    }

    public void OnBeginDrag(PointerEventData e)
    {
        _scrollRect.OnBeginDrag(e);
    }
    public void OnDrag(PointerEventData e)
    {
        _scrollRect.OnDrag(e);
    }
    public void OnEndDrag(PointerEventData e)
    {
        _scrollRect.SetHorizontalNormalizedPosition(_nowVector2.x);
        SetActiveMode((int)_nowVector2.x, _preVector2X);
    }
}