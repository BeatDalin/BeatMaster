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
    public int unlockCharNum; // 일단은 level 하나 당 한 캐릭터만 해금
    public bool levelClear;
}

[Serializable]
public struct StoreData
{
    public int charCount; // 상점 내 모든 캐릭터 수
    public CharacterData[] characterData; // 모든 캐릭터 목록(해금 체크를 위함)
}

[Serializable]
public struct CharacterData
{
    public int characterNum;
    public int price;
    public bool isPurchased;
    public bool isUnlocked;
}

