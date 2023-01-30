using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    [SerializeField] private GameObject _flat;
    [SerializeField] private GameObject _model;
    [SerializeField] private GameObject _starCanvas;

    private Image[] _starImg = new Image[3];

    private void Start()
    {
        for (int i = 0; i < 3; i++)
        {
            _starImg[i] = _starCanvas.transform.GetChild(i).GetComponent<Image>();
        }
    }
    public void ShowBuilding(bool isClear)
    {
        _flat.GetComponent<MeshRenderer>().enabled = true;
        _flat.GetComponent<BoxCollider>().enabled = !isClear;
        _model.GetComponent<MeshRenderer>().enabled = isClear;
        _starCanvas.GetComponent<Canvas>().enabled = isClear;
        
        Instantiate(this);
    }

    public void ShowStar(int star)
    {
        for (int i = 0; i < star; i++)
        {
            _starImg[i].enabled = true;
        }
    }
}
   
    
