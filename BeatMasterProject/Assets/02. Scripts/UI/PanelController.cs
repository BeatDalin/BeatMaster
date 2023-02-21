using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PanelController : MonoBehaviour
{
    public void ClosePanel(GameObject panelName)
    {
        panelName.GetComponent<RectTransform>().DOLocalMove(new Vector3(Screen.width, 0, 0), 0.4f).onComplete +=
            () => { panelName.SetActive(false); };
    }
}