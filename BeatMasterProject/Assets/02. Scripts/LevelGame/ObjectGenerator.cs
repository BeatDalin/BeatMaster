using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] private List<Vector3Int> _longObjPosList;
    
    private static readonly int IsPlay = Animator.StringToHash("isPlay");
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void RecordLongPos(Vector3Int pos)
    {
        // 롱노트 트랙 체크 -> 롱노트 이벤트일 때는 short 페이로드 1로 바꿔줌
        
        _longObjPosList.Add(pos);
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
