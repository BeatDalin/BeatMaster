using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class UIExperiment : MonoBehaviour
{
    public Text itemText;
    public Text deathText;

    public GameObject finalPanel;
    public Text finalFast;
    public Text finalPerfect;
    public Text finalSlow;

    // Start is called before the first frame update
    void Start()
    {
        InitUI();
    }

    public void InitUI()
    {
        itemText.text = "0";
        finalPanel.SetActive(false);
    }
    public void ShowItemCount(int itemCount)
    {
        itemText.text = itemCount.ToString();
    }

    public void ShowDeathCount(int death)
    {
        deathText.text = death.ToString();
    }

    public void ShowFinalResult(int[] finalResultSummary, int total)
    {
        finalFast.text = $"{finalResultSummary[1]}/{total}";
        finalPerfect.text = $"{finalResultSummary[2]}/{total}";
        finalSlow.text = $"{finalResultSummary[3]}/{total}";
        finalPanel.SetActive(true);
    }
}
