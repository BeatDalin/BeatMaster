using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    [SerializeField] private GameObject _flat;
    [SerializeField] private GameObject _model;
    [SerializeField] private GameObject _starCanvas;
    [SerializeField] private Image[] _starImg;

    public void ShowBuilding(bool isClear)
    {
        Instantiate(this);
        _flat.GetComponent<MeshRenderer>().enabled = true;
        _flat.GetComponent<BoxCollider>().enabled = !isClear;
        _model.GetComponent<MeshRenderer>().enabled = isClear;
        _starCanvas.GetComponent<Canvas>().enabled = isClear;
    }

    public void ShowStar(int star)
    {
        for (int i = 0; i < star; i++)
        {
            _starImg[i].enabled = true;
        }
    }
}
   
    
