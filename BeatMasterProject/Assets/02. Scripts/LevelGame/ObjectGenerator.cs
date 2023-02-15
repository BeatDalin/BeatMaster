using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using SonicBloom.Koreo;
using UnityEngine;

public class ObjectGenerator : MonoBehaviour
{
    [Header("Map")] 
    [SerializeField] [EventID] private string _mapEventID;
    private List<KoreographyEvent> _mapEvents;
    [Header("Item")]
    [SerializeField] private GameObject _starObj; // Item
    [SerializeField] private Transform _itemContainer;
    [Header("Check Point")] 
    [SerializeField] [EventID] private string _checkPointEventID;
    [SerializeField] private List<KoreographyEvent> _checkPointList;
    [Tooltip("Prefab object that contains Animator component.")]
    [SerializeField] private GameObject _checkPointPrefab; 
    private GameObject _checkPointObj; // Check point object to be placed
    private Animator _checkPointAnim;
    [SerializeField] private List<Vector3> _checkPointPos;
    private int _checkPointIdx;

    [Header("Short Note Obstacles")]
    [SerializeField] private GameObject _obstObj;
    [SerializeField] private Transform _obstContainer;
    [SerializeField] private List<Vector3> _obstPosList;
    private List<Obstacle> _obstacleScripts = new List<Obstacle>();
    private int _obsIdx;
    [SerializeField] private int[] _obstacleCounts;

    [Header("Long Note Start/End")]
    [SerializeField] private GameObject _longObj;
    [SerializeField] private Transform _longObjContainer;
    [SerializeField] private List<Vector3> _longObjPosList;

    // private CharacterMovement _characterMovement;
    // private Game _game;
    
    private static readonly int IsPlay = Animator.StringToHash("isPlay");

    private void Awake()
    {
        // Get Events of Map
        _mapEvents = SoundManager.instance.playingKoreo.GetTrackByID(_mapEventID).GetAllEvents();
        // Get Events of Check Point
        _checkPointList = SoundManager.instance.playingKoreo.GetTrackByID(_checkPointEventID).GetAllEvents();
        // Instantiate Prefab as a GameObject
        _checkPointObj = Instantiate(_checkPointPrefab, Vector3.zero, Quaternion.identity);
        _checkPointAnim = _checkPointObj.GetComponent<Animator>();
        // Check Point Initialize
        _checkPointIdx = -1;
        // ObstacleCount array to count the number of obstacles before checkPoints
        _obsIdx = 0;
        _obstacleCounts = new int[_checkPointList.Count];
    }

    private void Start()
    {
        PositLongNotify();
    }

    public void RecordShortPos(Vector3 pos)
    {
        _obstPosList.Add(pos);
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
        var obstacle = Instantiate(_obstObj, new Vector3(xPos + 1, yPos + 0.6f, 0), Quaternion.identity);
        obstacle.transform.SetParent(_obstContainer);
        _obstacleScripts.Add(obstacle.GetComponent<Obstacle>());

        // Record the number of obstacles before each check point.
        if (xPos + 1 > _checkPointPos[_obsIdx].x)
        {
            _obsIdx++;
            _obstacleCounts[_obsIdx] = _obstacleCounts.Sum();
        }
        if (xPos + 1 <= _checkPointPos[_obsIdx].x)
        {
            _obstacleCounts[_obsIdx]++;
        }
    }

    /// <summary>
    /// A function to reset animation state of obstacles. Called at NormalGame's Rewind().
    /// </summary>
    public void ResetObstAnimation()
    {
        // Reset all obstacle's animation before next checkpoint.
        for (int i = _obstacleCounts[_checkPointIdx]; i < _obstacleCounts[_checkPointIdx + 1]; i++)
        {
            _obstacleScripts[i].ResetAnim();
        }
    }

    
    #region CheckPoint
    /// <summary>
    /// Record Check Point's Location, Enroll 
    /// </summary>
    /// <param name="xPos"></param>
    /// <param name="yPos"></param>
    public void RecordCheckPoint(int xPos, int yPos)
    {
        _checkPointPos.Add(new Vector3(xPos, yPos, 0)); // Record position
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
        // _checkPointVisited[_checkPointIdx] = true;
        _checkPointAnim.SetTrigger(IsPlay); // Play Animation
        return _checkPointList[_checkPointIdx].StartSample;
    }
    #endregion
}
