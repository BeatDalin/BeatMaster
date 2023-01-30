using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingManager : MonoBehaviour
{
    public int currStage = 1;
    public int currMaxLevel; // 현재 Clear한 레벨 중 가장 높은 레벨
    //[SerializeField] GameObject levelIndicator; //향후 Waypoint 완성 시 활성화 예정..

    // 현재 Stage의 LevelData들
    public LevelData[] currStageData;
    
    // 현재 Stage의 Building들
    [SerializeField] private GameObject[] _buildings;
    
    private void Start()
    {
        DataCenter.Instance.LoadData();
        
        currMaxLevel = GetMaxLevelInStage(currMaxLevel);

        // Level 5까지 Clear했을 경우 다음 Level은 보여줄 필요가 없으니 break
        int limitMaxLevel = currMaxLevel + 1 > 4 ? 4 : currMaxLevel + 1;
        
        // 현재 레벨의 모델을 보여준다.
        for (int i = 0; i < limitMaxLevel; i++)
        {
            // Clear한 레벨 정보를 저장
            currStageData[i] = 
                DataCenter.Instance.GetLevelData(currStage - 1, i);
            
            // 레벨 Clear여부에 따라 건축물과 별을 보여준다.
            Building building = _buildings[i].GetComponent<Building>();
            bool isClear = currStageData[i].levelClear;
            
            building.ShowBuilding(isClear);
            
            if (isClear)
            {
                building.ShowStar(currStageData[i].star);
            }

            else
            {
                break;
            }
        }
    }

    /// <summary>
    /// 현재 Stage 내에서 클리어한 최대 level을 반환하는 함수
    /// </summary>
    /// <returns>return 값 0 == level 1 Clear여부 알 수 없음, 값 4 == level 5 Clear</returns>
    public int GetMaxLevelInStage(int currMaxLevel)
    {
        int maxLevel;
        
        maxLevel = currMaxLevel;

        for (int i = 4; i >= 0; i--)
        {
            if (currStageData[i].levelClear)
            {
                maxLevel = i;
                break;
            }
        }
        
        return maxLevel;
    }
}

