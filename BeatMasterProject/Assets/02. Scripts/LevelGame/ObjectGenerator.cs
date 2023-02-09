using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    [Header("Item")]
    [SerializeField] private GameObject _starObj; // Item
    [SerializeField] private Transform _itemContainer;
    [Header("Check Point")]
    [SerializeField] private GameObject _checkPointObj; // 
    [SerializeField] private List<Animator> _checkPointAnim = new List<Animator>();

    [Header("Long Note Start/End")] 
    [SerializeField] private GameObject _longObj;
    [SerializeField] private Transform _longObjContainer;
    [SerializeField] private List<Vector3> _longObjPosList;
    
    private static readonly int IsPlay = Animator.StringToHash("isPlay");
    
    public void RecordLongPos(Vector3 pos)
    {
        Debug.Log("Record Long Pos");
        _longObjPosList.Add(pos);
    }

    public void PositLongNotify()
    {
        for (int i = 0; i < _longObjPosList.Count; i++)
        {
            var item = Instantiate(_longObj, _longObjPosList[i], Quaternion.identity);
            item.transform.SetParent(_longObjContainer);
        }
    }
    
        
    public void PositItems(int xPos, int yPos)
    {
        var item = Instantiate(_starObj, new Vector3(xPos, yPos, 0), Quaternion.identity);
        item.transform.SetParent(_itemContainer);
    }

    public void PositCheckPoint(int xPos, int yPos)
    {
        var effect = Instantiate(_checkPointObj, new Vector3Int(xPos, yPos + 1, 0), Quaternion.identity);
        _checkPointAnim.Add(effect.GetComponent<Animator>());
    }
    
    
    public void PlayCheckAnim(int idx)
    {
        _checkPointAnim[idx].SetTrigger(IsPlay);
    }
    
}
