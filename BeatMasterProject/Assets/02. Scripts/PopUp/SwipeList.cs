using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class SwipeList : MonoBehaviour
{
    public GameObject scrollbar;
    private float _scrollPos = 0;
    private float[] _pos;
    private float _distance;

    void Start()
    {
        _pos = new float[transform.childCount];
        _distance = 1f / (_pos.Length - 1f);
    }
    void Update()
    {
        for (int i = 0; i < _pos.Length; i++)
        {
            _pos[i] = _distance * i;
        }

        if (Input.GetMouseButton(0))
        {
            _scrollPos = scrollbar.GetComponent<Scrollbar>().value;
        }

        else
        {
            for (int i = 0; i < _pos.Length; i++)
            {
                if (_scrollPos < _pos[i] + (_distance / 2) && _scrollPos > _pos[i] - (_distance / 2))
                {
                    scrollbar.GetComponent<Scrollbar>().value =
                        Mathf.Lerp(scrollbar.GetComponent<Scrollbar>().value, _pos[i], 0.1f);
                }
            }
        }
    }
}
