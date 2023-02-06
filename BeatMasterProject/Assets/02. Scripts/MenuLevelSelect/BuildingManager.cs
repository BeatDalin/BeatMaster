using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public int currStage = 1;
    public int currMaxLevel; // 현재 Clear한 레벨 중 가장 높은 레벨
    private WaypointMover _levelIndicator; //향후 Waypoint 완성 시 활성화 예정..

    // 현재 Stage의 LevelData들
    public LevelData[] currStageData;
    
    // 현재 Stage의 Building들
    [SerializeField] private GameObject[] _buildings;

    private void Awake()
    {
        // Debug용 Data 초기화
        //DataCenter.Instance.InitializeAllData();
        
        DataCenter.Instance.LoadData();
        
        // Level 정보 저장
        for (int i = 0; i < 5; i++)
        {
            currStageData[i] =
                DataCenter.Instance.GetLevelData(currStage - 1, i);
        }
        
        _levelIndicator = FindObjectOfType<WaypointMover>();

        //currMaxLevel = GetMaxLevelInStage();

        SetBuildings();
    }

    private void Start()
    {
        // Debug용 Data 초기화
        //DataCenter.Instance.InitializeAllData();
        
        // DataCenter.Instance.LoadData();
        //
        // // Level 정보 저장
        // for (int i = 0; i < 5; i++)
        // {
        //     currStageData[i] =
        //         DataCenter.Instance.GetLevelData(currStage - 1, i);
        // }
        //
        // _levelIndicator = FindObjectOfType<WaypointMover>();
        //
        // currMaxLevel = GetMaxLevelInStage();
        //
        // SetBuildings();
    }

    private void SetBuildings()
    {
        // Level 5 Clear 시 Stage Clear
        bool isStageClear = currMaxLevel == 4 ? true : false;
        
        // Level 5 Clear 시 보여줄 level 없으니 for문 범위 제한
        int limitMaxLevel = currMaxLevel + 1 > 4 ? 4 : currMaxLevel;
        
        // 현재 Clear한 레벨과 그 다음 레벨 모델을 보여준다.
        for (int i = 0; i <= limitMaxLevel; i++)
        {
            // 레벨 Clear여부에 따라 건축물과 별을 보여준다.
            Building building = _buildings[i].GetComponent<Building>();
            
            bool isClear = currStageData[i].levelClear;
            float alpha = currStageData[i].alpha;
            int star = currStageData[i].star;

            building.ShowBuilding(isStageClear, isClear, alpha, star);
        }
    }

    private void Update()
    {
        // if (currMaxLevel < GetMaxLevelInStage())
        // {
        //     currMaxLevel = GetMaxLevelInStage();
        //     
        //     _levelIndicator.UpdateWaypointPosition();
        // }
    }

    /// <summary>
    /// 현재 Stage 내에서 클리어한 최대 level을 반환하는 함수
    /// </summary>
    // /// <returns>return 값 -1 == level 1 Clear X, 값 0 = level 1 Clear, 값 4 == level 5 Clear</returns>
    // public int GetMaxLevelInStage()
    // {
    //     int maxLevel;
    //     
    //     maxLevel = -1;
    //
    //     for (int i = 4; i >= 0; --i)
    //     {
    //         if (currStageData[i].levelClear)
    //         {
    //             maxLevel = i;
    //             break;
    //         }
    //     }
    //     
    //     return maxLevel+1;
    // }
}

