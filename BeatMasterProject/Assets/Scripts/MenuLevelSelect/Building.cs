using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Building : MonoBehaviour
{
    [SerializeField] private GameObject _flat;
    [SerializeField] private GameObject _starCanvas;
    [SerializeField] private Material[] _materials; //index 0 == black, index 1 == color
    public void ShowBuilding(bool isStageClear, bool isClear, float alpha, int star)
    {
        GameObject model = gameObject.transform.GetChild(1).gameObject;

        Canvas stars = _starCanvas.GetComponent<Canvas>();

        if (isClear)
        {
            MeshRenderer mesh = model.GetComponent<MeshRenderer>();

            Material currMat = isStageClear ? _materials[1] : _materials[0];
            
            currMat.color = new Color(currMat.color.r, currMat.color.g, currMat.color.b, alpha);
            mesh.material = currMat;
            
            ShowStar(star);
            
            stars.enabled = true;
            
            model.SetActive(true);
        }
        
        else
        {
            stars.enabled = false;
            
            model.SetActive(false);
        }
        
        _flat.SetActive(true);
        
        Instantiate(this);
    }

    private void ShowStar(int star)
    {
        for (int i = 0; i < 3; i++)
        {
            Image starImg = _starCanvas.transform.GetChild(i).GetComponent<Image>();

            if (i < star)
            {
                starImg.enabled = true;
            }
            else
            {
                starImg.enabled = false;
            }
        }
    }
    
}
   
    
