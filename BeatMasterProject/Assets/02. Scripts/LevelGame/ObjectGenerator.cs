using System;
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
    [EventID] public string _checkPointEventID;
    [Tooltip("Prefab object that contains Animator component.")]
    [SerializeField] private GameObject _checkPointPrefab; 
    private GameObject _checkPointObj; // Check point object to be placed
    private Animator _checkPointAnim;
    public List<KoreographyEvent> checkPointList;
    [SerializeField] private List<Vector3> _checkPointPos;
    private int _checkPointIdx;
    [SerializeField] private bool[] _checkPointVisited;

    [Header("Short Note Obstacles")]
    [SerializeField] private GameObject _obstacleObj;
    [SerializeField] private Transform _obstacleContainer;
    [SerializeField] private List<Vector3> _shortObjPosList;

    [Header("Long Note Start/End")] 
    [SerializeField] private GameObject _longObj;
    [SerializeField] private Transform _longObjContainer;
    [SerializeField] private List<Vector3> _longObjPosList;

    private CharacterMovement _characterMovement;
    private Game _game;
    
    private static readonly int IsPlay = Animator.StringToHash("isPlay");

    private void Awake()
    {
        _game = FindObjectOfType<Game>();
        _characterMovement = FindObjectOfType<CharacterMovement>();
        // Get Events of Check Point
        checkPointList = SoundManager.instance.playingKoreo.GetTrackByID(_checkPointEventID).GetAllEvents();
        // Instantiate Prefab as a GameObject
        _checkPointObj = Instantiate(_checkPointPrefab, Vector3.zero, Quaternion.identity);
        _checkPointAnim = _checkPointObj.GetComponent<Animator>();
        // Check Point Initialize
        _checkPointIdx = -1;
        _checkPointVisited = new bool[checkPointList.Count];
    }

    private void Start()
    {
        PositLongNotify();
    }

    public void RecordShortPos(Vector3 pos)
    {
        _shortObjPosList.Add(pos);
    }
    public void RecordLongPos(Vector3 pos)
    {
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

    public void PositObstacles(float xPos, float yPos)
    {
        var obstacle = Instantiate(_obstacleObj, new Vector3(xPos + 1, yPos + 0.5f, 0), Quaternion.identity);
        obstacle.transform.SetParent(_obstacleContainer);
        // _objectGenerator.RecordShortPos(new Vector3(_tileX, _tileY + _groundYOffset));
        // _objectGenerator.PositObstacles(_tileX, _tileY + _groundYOffset);
    }

    
    #region CheckPoint

    public void RecordCheckPoint(int xPos, int yPos)
    {
        _checkPointPos.Add(new Vector3(xPos, yPos, 0));
    }
    
    /// <summary>
    /// Increase an index to access List of position and boolean values.
    /// Move the check point object to current check point position. Record current check point as visited. Play an Animation.
    /// </summary>
    /// <returns>Start sample of current check point, which will be rewindSampleTime in NormalGame.cs</returns>
    public int MoveCheckPointForward()
    {
        _checkPointIdx++;
        _checkPointObj.transform.position = _checkPointPos[_checkPointIdx];
        _checkPointVisited[_checkPointIdx] = true;
        _checkPointAnim.SetTrigger(IsPlay); // Play Animation
        return checkPointList[_checkPointIdx].StartSample;
    }
    // public void PositCheckPoint(int xPos, int yPos)
    // {
    //     var effect = Instantiate(_checkPointObj, new Vector3Int(xPos, yPos + 1, 0), Quaternion.identity);
    //     _checkPointAnim.Add(effect.GetComponent<Animator>());
    // }
    
    
    // public void PlayCheckAnim(int idx)
    // {
    //     _checkPointAnim[idx].SetTrigger(IsPlay);
    // }

    #endregion

    private void Update()
    {
        if (_checkPointPos.Count - 1 > _checkPointIdx)
        {
            if (_characterMovement.transform.position.x > _checkPointPos[_checkPointIdx + 1].x)
            {
                _game.curSample = checkPointList[_checkPointIdx + 1].StartSample;
                MoveCheckPointForward();
            }
        }
    }
}
