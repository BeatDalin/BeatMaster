using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class ResultStarAnim : MonoBehaviour
{
    [SerializeField] private GameObject[] _star;
    [SerializeField] private Color _successColor;
    [SerializeField] private Text _itemCountText;

    public int score;

    public int itemCount;
    // Start is called before the first frame update
    void Start()
    {
        SetStarColor();
        GainItemAnim();
    }

    private void GainItemAnim()
    {
        if (itemCount != 0)
        {
            
        }
    }

    private void SetStarColor()
    {
        if (score >= 65)
        {
            _star[0].SetActive(true);
            _star[0].GetComponent<Image>().color = _successColor;
            _star[0].transform.DORotate(new Vector3(0, 180, 0), 0.5f).onComplete += () =>
            {
                _star[1].SetActive(true);
                _star[1].GetComponent<Image>().color = _successColor;
                _star[1].transform.DORotate(new Vector3(0, 180, 0), 0.5f).onComplete += () =>
                {
                    _star[2].SetActive(true);
                    _star[2].GetComponent<Image>().color = _successColor;
                    _star[2].transform.DORotate(new Vector3(0, 180, 0), 0.5f);
                };
            };
        }
        else if (score >= 35)
        {
            _star[0].SetActive(true);
            _star[0].GetComponent<Image>().color = _successColor;
            _star[0].transform.DORotate(new Vector3(0, 180, 0), 0.5f).onComplete += () =>
            {
                _star[1].SetActive(true);
                _star[1].GetComponent<Image>().color = _successColor;
                _star[1].transform.DORotate(new Vector3(0, 180, 0), 0.5f).onComplete += () =>
                {
                    _star[2].SetActive(true);
                    _star[2].transform.DORotate(new Vector3(0, 180, 0), 0.5f);
                };
            };
        }
        else if (score > 0)
        {
            _star[0].SetActive(true);
            _star[0].GetComponent<Image>().color = _successColor;
            _star[0].transform.DORotate(new Vector3(0, 180, 0), 0.5f).onComplete += () =>
            {
                _star[1].SetActive(true);
                _star[1].transform.DORotate(new Vector3(0, 180, 0), 0.5f).onComplete += () =>
                {
                    _star[2].SetActive(true);
                    _star[2].transform.DORotate(new Vector3(0, 180, 0), 0.5f);
                };
            };
        }
    }
}
