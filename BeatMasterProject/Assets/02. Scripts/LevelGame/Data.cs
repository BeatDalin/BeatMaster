using System;
using System.Collections.Generic;

[Serializable]
public struct Data
{
    public PlayerData playerData;
    public StageData[] stageData;
    public StoreData storeData;
}

[Serializable]
public struct PlayerData
{
    public int playerStage; // Player's current max stage number
    public int playerLv; // Player's current max level in playerStage number
    public int playerItem; // Player's item(coin) count
    public int playerChar;   // Player's current Character index
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

[Serializable]
public struct StoreData
{
    public List<ItemData> itemData; // 전체 상품 목록(해금 체크를 위함)
}

[Serializable]
public struct ItemData
{
    public int itemNum;
    public int price;
    public bool isPurchased;
    public int unlockStage; // index
    public int unlockLevel; // index
    public bool isUnlocked;
}

