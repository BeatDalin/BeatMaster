using System;
using System.Collections;
using System.Collections.Generic;
using SonicBloom.Koreo;
using UnityEngine;
using Random = UnityEngine.Random;

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
    [Header("Check Point - Prefabs")] 
    [Tooltip("Prefab object that contains Animator component.")]
    [SerializeField] private GameObject _checkPointPrefab;
    [Tooltip("An array of particle prefabs. Random colors will appear.")]
    [SerializeField] private ParticleSystem[] _checkPointParticleSystems;
    [SerializeField] private ParticleSystem[] _checkPointParticleFlicks;
    [SerializeField] private ParticleSystem[] _checkPointParticleGlow;
    [SerializeField] private ParticleSystem[] _checkPointParticleSmoke;
    [SerializeField] private int[] _randomParticleNums;
    [SerializeField] private Transform _particleContainer;
    private GameObject _checkPointObj; // Check point object to be placed
    private Animator _checkPointAnim;
    public List<Vector3> checkPointPos;
    private int _checkPointIdx;

    [Header("Short Note Obstacles")]
    [SerializeField] private GameObject _obstObj;
    [SerializeField] private Transform _obstContainer;
    [SerializeField] private List<Vector3> _jumpPosList;
    [SerializeField] private List<Obstacle> _obstacleScripts = new List<Obstacle>();
    private int _obsIdx;
    [SerializeField] private int[] _obstacleCounts;
    private const string _obstacleTag0 = "Obstacle0";

    [Header("Long Note Start/End")]
    [SerializeField] private GameObject _longObj;
    [SerializeField] private Transform _longObjContainer;
    [SerializeField] private List<Vector3> _longObjPosList;

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
        // Check Point Particle Systems
        _randomParticleNums = new int[_checkPointList.Count];
        InitParticles();
        // Record random particle nums
        for (int i = 0; i < _checkPointList.Count; i++)
        {
            _randomParticleNums[i] = Random.Range(0, _checkPointParticleSystems.Length);
        }

        // ObstacleCount array to count the number of obstacles before checkPoints
        _obsIdx = 0;
        _obstacleCounts = new int[_checkPointList.Count];
    }

    private void Start()
    {
        PositLongNotify();
    }

    public void RecordJumpPos(Vector3 pos)
    {
        _jumpPosList.Add(pos);
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
    
    public void PositItems(float xPos, float yPos)
    {
        var item = Instantiate(_starObj, new Vector3(xPos, yPos, 0), Quaternion.identity);
        item.transform.SetParent(_itemContainer);
    }

    public void PositObstacles(float xPos, float yPos)
    {
        var obstacle = Instantiate(_obstObj, new Vector3(xPos + 1, yPos + 0.6f, 0), Quaternion.identity);
        if (obstacle.CompareTag(_obstacleTag0))
        {
            obstacle.transform.position -= Vector3.up * 0.2f;
        }
        obstacle.transform.SetParent(_obstContainer);
        _obstacleScripts.Add(obstacle.GetComponent<Obstacle>());

        // Record the number of obstacles before each check point.
        // Check point position is already behind current position -> Increase an index to access obstacle count array
        if (xPos + 1 > checkPointPos[_obsIdx].x)
        {
            _obsIdx++;
            _obstacleCounts[_obsIdx] = _obstacleCounts[_obsIdx - 1]; // Starts from the number of obstacles before currently arrived checkpoint
        }
        
        // Increase the number of obstacles before arriving next checkpoint
        if (xPos + 1 <= checkPointPos[_obsIdx].x)
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
        // Reset obstacles between my recent check point and next check point.
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
    public void RecordCheckPoint(float xPos, float yPos)
    {
        checkPointPos.Add(new Vector3(xPos, yPos, 0)); // Record position
    }

    private void InitParticles()
    {
        int particleCounts = _particleContainer.childCount;
        _checkPointParticleSystems = new ParticleSystem[particleCounts];
        _checkPointParticleFlicks = new ParticleSystem[particleCounts];
        _checkPointParticleGlow = new ParticleSystem[particleCounts];
        _checkPointParticleSmoke = new ParticleSystem[particleCounts];
        for (int i = 0; i < particleCounts; i++)
        {
            Transform child = _particleContainer.GetChild(i);
            _checkPointParticleSystems[i] = child.GetComponent<ParticleSystem>();
            _checkPointParticleFlicks[i] = child.GetChild(0).GetComponent<ParticleSystem>();
            _checkPointParticleGlow[i] = child.GetChild(2).GetComponent<ParticleSystem>();
            _checkPointParticleSmoke[i] = child.GetChild(3).GetComponent<ParticleSystem>();
        }
    }
    
    /// <summary>
    /// Increase an index to access List of positions and boolean values.
    /// Move the check point object to current check point position. Record current check point as visited. Play an Animation.
    /// </summary>
    /// <returns>Start sample of current check point, which will be rewindSampleTime in NormalGame.cs</returns>
    public int MoveCheckPointForward()
    {
        _checkPointIdx++;
        _checkPointObj.transform.position = checkPointPos[_checkPointIdx];
        _particleContainer.position = checkPointPos[_checkPointIdx] + Vector3.down;
        int randIdx = _randomParticleNums[_checkPointIdx];
        _checkPointParticleSystems[randIdx].Play();
        _checkPointParticleFlicks[randIdx].Play();
        _checkPointParticleGlow[randIdx].Play();
        _checkPointParticleSmoke[randIdx].Play();
        // _checkPointVisited[_checkPointIdx] = true;
        _checkPointAnim.SetTrigger(IsPlay); // Play Animation
        return _checkPointList[_checkPointIdx].StartSample;
    }
    #endregion
}
