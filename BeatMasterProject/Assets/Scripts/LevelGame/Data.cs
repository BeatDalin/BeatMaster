using System;

[Serializable]
public struct Data
{
    public int playerStage; // player's current  max stage number
    public int playerLv; // player's current max level in playerStage number

    public StageData[] stageData;
} 

[Serializable]
public struct StageData
{
    public int stage;
    public bool bossClear;
    public LevelData[] levelData; // contains five LevelData
}

[Serializable]
public struct LevelData
{
    public int level;
    public int perfectCount;
    public int fastCount;
    public int slowCount;

    public int star;
    public float alpha;
    public bool levelClear;
}

