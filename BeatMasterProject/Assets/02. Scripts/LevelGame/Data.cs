using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct Data
{
    public PlayerData playerData;
    public StageData[] stageData; // contains four StageData
    public StoreData storeData;
    public Achievement achievement;
}

[Serializable]
public struct PlayerData
{
    public int playerStage; // Player's current max stage number
    public int playerLv; // Player's current max level in playerStage number
    public int playerItem; // Player's item(coin) count
    public int playerChar; // Player's current Character index
    public int[] itemData; // equipped items [itemPart(enum), itemNum -1 == nothing] 
}

[Serializable]
public struct StageData
{
    public int stage;
    //public bool bossClear;
    public LevelData[] levelData; // contains four LevelData
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
    public bool isUnlocked;
}

[Serializable]
public struct StoreData
{
    public enum ItemPart
    {
        Background = 0,
        Neck,
        Face,
        Head,
    }

    public enum ItemName
    {
        Balloon = 0,
        Ribbon,
        Sunglasses,
        SantaHat,
        MagicianHat,
        Crown,
        RabbitBand,
        Medal,
        FlowerCrown,
        Pet,
        Empty = 99,
    }

    public int charCount; // 상점 내 모든 캐릭터 수
    public int itemCount; // 상점 내 모든 아이템 수
    public CharacterData[] characterData; // 모든 캐릭터 목록(해금 체크를 위함)
    public ItemData[] itemData; // 모든 아이템 목록
}

[Serializable]
public struct CharacterData
{
    public int characterNum;
    public int price;
    public int unlockStage;
    public int unlockLevel;
    public bool isPurchased;
    public bool isUnlocked;
}

[Serializable]
public struct ItemData
{
    public StoreData.ItemName itemName;
    public StoreData.ItemPart itemPart;
    public int price;
    public bool isPurchased;
    public bool isUnlocked;
}

[Serializable]
public struct RewindData
{
    public Vector2 rewindPos;
    public string judgeResult;
}

[Serializable]
public struct Achievement
{
    public bool isFirstPurchased;
    public bool isStarted;
    public bool isGrown;
    public bool isMaster;
    /*public bool isPlayedOnce;
    public bool isPlayedFive;
    public bool isPlayedTen;*/
    public bool isFirstItem;
    public bool isCrownItem;
    public bool isRestartedOverHundred;
    public bool isFirstPayment;
    public int playCount;
}
