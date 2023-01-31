using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    [SerializeField] private GameObject _flat;
    [SerializeField] private GameObject _model;
    [SerializeField] private GameObject _starCanvas;

    //public GameObject[] _starImg;

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
        for (int i = 0; i < 3; i++)
        {
            if (i < star)
            {
                _starCanvas.transform.GetChild(i).GetComponent<Image>().enabled = true;
            }
            else
            {
                _starCanvas.transform.GetChild(i).GetComponent<Image>().enabled = false;

            }
        }
    }
}
   
    
