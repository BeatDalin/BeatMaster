using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waypoints : MonoBehaviour
{
    [Range(0f, 2f)]
    [SerializeField] private float _waypointSize = 0.5f;

    // Scene Tab에 경로 기즈모 그리는 함수
    private void OnDrawGizmos()
    {
        foreach (Transform t in transform)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(t.position, _waypointSize);
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < transform.childCount-1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position,transform.GetChild(i+1).position);
        }
        
        Gizmos.DrawLine(transform.GetChild(transform.childCount-1).position, transform.GetChild(0).position);
    }
    
    // 다음 경로를 반환하는 함수
    public Transform GetNextWaypoint(Transform currentWaypoint)
    {
        if (currentWaypoint != null && currentWaypoint.GetSiblingIndex() < transform.childCount - 1)
        {
            return transform.GetChild(currentWaypoint.GetSiblingIndex() + 1);
        }

        else
        {
            return transform.GetChild(0);
        }
    }
}
