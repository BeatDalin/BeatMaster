using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    // player가 따라 움직일 waypoint 정보
    [SerializeField] private Waypoints _waypoints;
    
    [SerializeField] private float _moveSpeed = 2f;

    [SerializeField] private GameObject _stagePathTrail;

    // player가 현재 위치에서 다음으로 이동할 위치
    private Transform _currWaypoint;
    private int _currLevel;
    private BuildingManager _buildingManager;
    private bool _isLocated;
    private Vector3 _startPos;
    
    void Start()
    {
        _buildingManager = FindObjectOfType<BuildingManager>();
        _currLevel = _buildingManager.currMaxLevel < 0 ? 0 : _buildingManager.currMaxLevel;
        
        // 현재 레벨 위치로 초기화
        _currWaypoint = _waypoints.transform.GetChild(_currLevel);
        //transform.position = _currWaypoint.position;

        _startPos = transform.position;
    }

    public void UpdateWaypointPosition()
    {
        // 다음으로 이동할 위치
        _currWaypoint = _waypoints.GetNextWaypoint(_currWaypoint);

        StartCoroutine(MovePos(_currWaypoint.position));
    }

    private void Update()
    {
        if (!_isLocated)
        {
            _stagePathTrail.GetComponent<TrailRenderer>().enabled = true;
            _stagePathTrail.SetActive(true);
            _isLocated = true;
            StartCoroutine(MoveTrail(_currWaypoint.position));
        }
    }

    IEnumerator MoveTrail(Vector3 wayPoint)
    {
        while (_stagePathTrail.transform.position != wayPoint)
        {
            _stagePathTrail.transform.position = 
                Vector3.MoveTowards(_stagePathTrail.transform.position, wayPoint, _moveSpeed * Time.deltaTime);
            yield return null;
        }
        _stagePathTrail.SetActive(false);
        _stagePathTrail.transform.position = _startPos;
        yield return new WaitForSeconds(1f);
        
        _isLocated = false;
    }

    IEnumerator MovePos(Vector3 currWayPoint)
    {
        while (transform.position != currWayPoint)
        {
            transform.position =
                Vector3.MoveTowards(transform.position, _currWaypoint.position, _moveSpeed * Time.deltaTime);
            _stagePathTrail.transform.position = 
                Vector3.MoveTowards(_stagePathTrail.transform.position, _currWaypoint.position, _moveSpeed * Time.deltaTime);
            yield return null;
        }
        // SceneLoadManager.Instance.LoadLevelAsync(SceneLoadManager.SceneType.Level4);
    }
}
