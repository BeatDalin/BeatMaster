using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMover : MonoBehaviour
{
    // player가 따라 움직일 waypoint 정보
    [SerializeField] private Waypoints _waypoints;
    
    [SerializeField] private float moveSpeed = 2f;
    
    // player가 현재 위치에서 다음으로 이동할 위치
    private Transform _currWaypoint;
    private int _currLevel;
    private BuildingManager _buildingManager;
    void Start()
    {
        _buildingManager = FindObjectOfType<BuildingManager>();
        _currLevel = _buildingManager.currMaxLevel;
        
        // 현재 레벨 위치로 초기화
        _currWaypoint = _waypoints.transform.GetChild(_currLevel);
        transform.position = _currWaypoint.position;
        
    }

    public void UpdatePosition()
    {
        // 다음으로 이동할 위치
        _currWaypoint = _waypoints.GetNextWaypoint(_currWaypoint);
        
        transform.position =
            Vector3.MoveTowards(transform.position, _currWaypoint.position, moveSpeed * Time.deltaTime);
    }

    public void MovePosition(Transform _transform)
    {
        _currWaypoint = _transform;
        transform.position = _transform.position;

    }
}
