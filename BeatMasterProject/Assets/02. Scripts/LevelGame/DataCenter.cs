using System;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DataCenter : MonoBehaviour
{
    [SerializeField]
    private Data _gameData; // keep it private, and update it through method using stage & level number
    private PlayerData _playerData;
    private Achievement _achievement;
    private readonly string _fileName = "Data.json"; // file name 
    private string _path => Application.persistentDataPath + '/' + _fileName;
    private static GameObject _go;
    private static DataCenter _instance;

    public static DataCenter Instance
    {
        get
        {
            if (!_instance)
            {
                _go = new GameObject();
                _go.name = "DataCenter";
                _instance = _go.AddComponent<DataCenter>();
                DontDestroyOnLoad(_go);
            }
            return _instance;
        }
    }

    private void Start()
    {
        // if you want to find where the json file is located...
        // Debug.Log(Application.persistentDataPath);
        // CreateData();
    }

    public void LoadData()
    {
        if (File.Exists(_path))
        {
            FileStream fs = new FileStream(_path, FileMode.Open); // open file from the path
            StreamReader stream = new StreamReader(fs);

            string data = stream.ReadToEnd();
            _gameData = JsonUtility.FromJson<Data>(data);
            _playerData = _gameData.playerData;
            _achievement = _gameData.achievement;

            stream.Close();
        }
        else
        {
            // if json file does not exist, create one
            CreateData();
        }
    }
    private void SaveData()
    {
        File.WriteAllText(_path, JsonUtility.ToJson(_gameData, true));
    }

    public void SaveData(LevelData levelData, int stageIdx, int levelIdx)
    {
        _gameData.stageData[stageIdx].levelData[levelIdx] = levelData;
        SaveData();
    }

    /*    public void SaveData(LevelData levelData, int stageIdx, int levelIdx, LeaderboardData leaderboardData)
        {
            _gameData.stageData[stageIdx].levelData[levelIdx] = levelData;
            _gameData.stageData[stageIdx].leaderboardData[levelIdx] = leaderboardData;
            SaveData();
        }*/

    public LevelData GetLevelData(int stageIdx, int levelIdx)
    {
        return _gameData.stageData[stageIdx].levelData[levelIdx];
    }

    public void InitializeAllData()
    {
        // will be connected to Data Initialize Button (데이터 초기화 버튼)
        CreateData(); // Initializing the json file is same as creating a new one and saving it
    }

    private void CreateData()
    {
        _gameData = new Data();
        _playerData = new PlayerData();
        _achievement = new Achievement();
        //_leaderboardData=new LeaderboardData();
        _playerData.playerLv = 1;
        _playerData.playerStage = 1;
        _playerData.playerChar = 0; // default character index
        _playerData.itemData = new int[Enum.GetValues(typeof(StoreData.ItemPart)).Length]; // 부위별(index) 현재 장착중인 item(value)
        _playerData.itemData = Enumerable.Repeat(-1, Enum.GetValues(typeof(StoreData.ItemPart)).Length).ToArray(); // 미장착 상태일 때 -1
        //_playerData.mapClearedCount = 0;

        _gameData.playerData = _playerData;
        _gameData.stageData = new StageData[4]; // temporally, set array size as 1
        LevelData temp = new LevelData();
        //LeaderboardData leaderboard= new LeaderboardData();
        for (int i = 0; i < _gameData.stageData.Length; i++)
        {
            _gameData.stageData[i].stage = i + 1;
            _gameData.stageData[i].levelData = new LevelData[4];
            //_gameData.stageData[i].leaderboardData= new LeaderboardData[4];

            for (int j = 0; j < _gameData.stageData[i].levelData.Length; j++)
            {
                temp.level = j + 1;
                temp.isUnlocked = j == 0;
                
                if (i == 0 && j == 0)
                {
                    temp.unlockCharNum = 1;
                }
                else if (i == 1 && j == 0)
                {
                    temp.unlockCharNum = 2;
                }
                else
                {
                    temp.unlockCharNum = 0;
                }

                _gameData.stageData[i].levelData[j] = temp;

                /*leaderboard.score = 0;
                _gameData.stageData[i].leaderboardData[j] = leaderboard;*/
            }
        }

        /*        _gameData.leaderboardData = new LeaderboardData[_gameData.stageData.Length];
                for(int i=0; i<_gameData.leaderboardData.Length; i++)
                {
                    _gameData.leaderboardData[i].deathCount = 0;
                    _gameData.leaderboardData[i].playTime = 0;
                    _gameData.leaderboardData[i].mapClearedCount=0;
                    _gameData.leaderboardData[i].starCount = 0;
                }*/

        _achievement.isFirstPurchased = false;
        _achievement.isStarted = false;
        _achievement.isGrown = false;
        _achievement.isMaster = false;
        /*        _achievement.isPlayedOnce = false;
                _achievement.isPlayedFive = false;
                _achievement.isPlayedTen = false;*/
        _achievement.isFirstItem = false;
        _achievement.isCrownItem = false;
        _achievement.isRestartedOverHundred = false;
        _achievement.isFirstPayment = false;
        _achievement.playCount = 0;

        CreateStoreData();
        SaveData();
    }

    /// <summary>
    /// Add new StageData when a boss level is cleared.
    /// </summary>
    public void AddStageData()
    {
        // Call AddStageData() when new stage is opened or previous stage's boss level has been cleared.
        var old = _gameData.stageData.ToList(); // convert the old array into a List
        StageData newStage = new StageData(); // create new StageData

        // push initial values to the new StageData
        newStage.stage = old.Count + 1;
        newStage.levelData = new LevelData[5];

        LevelData temp = new LevelData();
        for (int i = 0; i < 5; i++)
        {
            temp.level = i + 1;
            newStage.levelData[i] = temp;
        }
        old.Add(newStage);
        _gameData.stageData = old.ToArray();
        SaveData();
    }

    // public void UpdateStageData(int stageIdx)
    // {
    //     StageData curStageData = _gameData.stageData[stageIdx];
    //     curStageData.bossClear = true;
    // }

    /// <summary>
    /// Update Data and save updated content to json file.
    /// </summary>
    /// <param name="stageNum">Stage number (not an index) to be saved into Data </param>
    /// <param name="levelNum">Level number (not an index) to be save into Data</param>
    /// <param name="playerItem">Add this number to player's current item count</param>
    public void UpdatePlayerData(int stageNum, int levelNum, int playerItem = 0)
    {
        _playerData.playerStage = stageNum;
        _playerData.playerLv = levelNum;
        _playerData.playerItem += playerItem;
        _gameData.playerData = _playerData;
        SaveData();
    }

    private void CreateStoreData()
    {
        _gameData.storeData = new StoreData();
        _gameData.storeData.charCount = Enum.GetValues(typeof(StoreData.CharacterName)).Length;
        _gameData.storeData.characterData = new CharacterData[_gameData.storeData.charCount];
        CreateCharacterData();

        _gameData.storeData.itemCount = Enum.GetValues(typeof(StoreData.ItemName)).Length;
        _gameData.storeData.itemData = new ItemData[_gameData.storeData.itemCount];
        CreateItemData();

        _gameData.storeData.paidItemCount = Enum.GetValues(typeof(StoreData.PaidItemName)).Length;
        _gameData.storeData.paidItemData = new PaidItemData[_gameData.storeData.paidItemCount];
        CreatePaidItemData();
    }

    private void CreateCharacterData()
    {
        CharacterData[] tempCharacterData = _gameData.storeData.characterData;

        StoreData.CharacterName[] characterList =
            (StoreData.CharacterName[])Enum.GetValues(typeof(StoreData.CharacterName));
        
        int[] charPrice = { 0, 500, 500 };
        bool[] charIsPaidItem = { false, false, false };
        int[] unlockStages = { 0, 0, 1 };
        int[] unlockLevels = { -1, 0, 0 };
        string[] charDescription = { "Cheerful Corgi", "Awesome Corgi", "Cute Duck" };

        for (int i = 0; i < _gameData.storeData.charCount; i++)
        {
            tempCharacterData[i].characterName = characterList[i];
            tempCharacterData[i].characterNum = i;
            tempCharacterData[i].price = charPrice[i];
            tempCharacterData[i].unlockStage = unlockStages[i];
            tempCharacterData[i].unlockLevel = unlockLevels[i];
            tempCharacterData[i].isPurchased = i == 0;
            tempCharacterData[i].isUnlocked = i == 0;
            tempCharacterData[i].isPaidItem = charIsPaidItem[i];
            tempCharacterData[i].characterDescription = charDescription[i];
        }

        _gameData.storeData.characterData = tempCharacterData;
    }

    private void CreateItemData()
    {
        ItemData[] tempItemData = _gameData.storeData.itemData;

        StoreData.ItemName[] itemList = (StoreData.ItemName[])Enum.GetValues(typeof(StoreData.ItemName));
        StoreData.ItemPart[] itemParts =
        {
            StoreData.ItemPart.Background,
            StoreData.ItemPart.Attach,
            StoreData.ItemPart.Head,
            StoreData.ItemPart.Head,
            StoreData.ItemPart.Attach,
            StoreData.ItemPart.Face,
            StoreData.ItemPart.Attach,
            StoreData.ItemPart.Background,
            StoreData.ItemPart.Head,
            StoreData.ItemPart.Background,
            StoreData.ItemPart.Background,
            StoreData.ItemPart.Background
        };
        int[] itemPrice = { 50, 50, 70, 70, 150, 0};
        bool[] isPaidItem = { false, false, false, false, false, true, false, false, false, true, true, false };
        string[] itemDescription =
        {
            "Red balloon", "Cute ribbon", "Cozy santa hat", "Awesome magician hat", "Shiny crown",
            "Cool sunglasses", "Adorable rabbit band", "Heavy medal", "Beautiful flower crown",
            "Powerful cat", "Smart fox", "void"
        };


        for (int i = 0; i < _gameData.storeData.itemCount; i++)
        {
            tempItemData[i].itemName = itemList[i];
            tempItemData[i].itemPart = itemParts[i];
            tempItemData[i].isUnlocked = (int)tempItemData[i].itemName < (int)StoreData.ItemName.Crown; // crown전까지 unlocked true
            tempItemData[i].price = tempItemData[i].isUnlocked ? itemPrice[i] : 0;
            tempItemData[i].isPurchased = false;
            tempItemData[i].isPaidItem = isPaidItem[i];
            tempItemData[i].itemDescription = itemDescription[i];
        }

        _gameData.storeData.itemData = tempItemData;
    }

    private IEnumerator CoWaitToCreatePaidItemData()
    {
        // Wait user's wallet to be created.
        yield return FirebaseDataManager.Instance.waitForSearchEnd;
        CreatePaidItemData();
    }
    private void CreatePaidItemData()
    {
        StoreData.PaidItemName[] paidItemList = (StoreData.PaidItemName[])Enum.GetValues(typeof(StoreData.PaidItemName));
        int[] paidItemPrice = { 3000, 1000 };
        string[] paidItemDescription = { "Ultimate package for beginner", "Cute and smart fox" };

        Dictionary<int, StoreData.ItemName[]> dicPackageItem = new Dictionary<int, StoreData.ItemName[]>();
        dicPackageItem.Add(0, new StoreData.ItemName[]{StoreData.ItemName.fancySunglasses, StoreData.ItemName.petCat});
        dicPackageItem.Add(1, new StoreData.ItemName[]{StoreData.ItemName.petFox});

        Dictionary<int, int[]> dicPackageChar = new Dictionary<int, int[]>();
        dicPackageChar.Add(0, new int[] {0});
        dicPackageChar.Add(1, new int[] {0});

        
        PaidItemData[] tempPaidItemData = _gameData.storeData.paidItemData;
        for (int i = 0; i < _gameData.storeData.paidItemCount; i++)
        {
            tempPaidItemData[i].paidItemName = paidItemList[i];
            tempPaidItemData[i].price = paidItemPrice[i];
            // tempPaidItemData[i].isPurchased = false; // 앱 삭제 후 다시 받은 경우 고려해야. 서버에서 받아와야할지도
            tempPaidItemData[i].isPurchased = FirebaseDataManager.Instance.CheckProductInWallet(tempPaidItemData[i].paidItemName.ToString()); // 앱 삭제 후 다시 받은 경우 고려해야. 서버에서 받아와야할지도
            tempPaidItemData[i].packageCharacterNum = dicPackageChar[i].ToArray();
            tempPaidItemData[i].packageItemName = dicPackageItem[i];
            tempPaidItemData[i].paidItemDescription = paidItemDescription[i];
        }
    }

    public void CheckPaidItemPurchase()
    {
        ItemData[] tempItemData = _gameData.storeData.itemData;
        PaidItemData[] tempPaidItemData = _gameData.storeData.paidItemData;
        for (int i = 0; i < _gameData.storeData.paidItemCount; i++)
        {
            // tempPaidItemData[i].isPurchased = false; // 앱 삭제 후 다시 받은 경우 고려해야. 서버에서 받아와야할지도
            bool searchWalletResult = FirebaseDataManager.Instance.CheckProductInWallet(tempPaidItemData[i].paidItemName.ToString());
            tempPaidItemData[i].isPurchased = searchWalletResult; // 앱 삭제 후 다시 받은 경우 고려해야. 서버에서 받아와야할지도
            for (int j = 0; j < tempPaidItemData[i].packageItemName.Length; j++)
            {
                var index = (int)tempPaidItemData[i].packageItemName[j];
                tempItemData[index].isPurchased = searchWalletResult;
                tempItemData[index].isUnlocked = searchWalletResult;
            }
        }

        _gameData.storeData.itemData = tempItemData;
        _gameData.storeData.paidItemData = tempPaidItemData;
        SaveData();
    }

    public StoreData GetStoreData()
    {
        return _gameData.storeData;
    }
    public ItemData[] GetItemData()
    {
        return _gameData.storeData.itemData;
    }

    /// <summary>
    /// 캐릭터 구매 후 CharacterData의 isPurchased 업데이트
    /// 캐릭터 구매 후 playerData의 playerItem 업데이트
    /// </summary>
    /// <param name="charNum"></param>
    public void UpdateCharacterPurchaseData(int charNum)
    {
        _gameData.storeData.characterData[charNum].isPurchased = true;
        _gameData.playerData.playerItem -= _gameData.storeData.characterData[charNum].price;
#if !UNITY_EDITOR
        if (!_gameData.achievement.isFirstPurchased)
        {
            GPGSBinder.Instance.UnlockAchievement(GPGSIds.achievement_purchase_first_character, success => _gameData.achievement.isFirstPurchased = true);
        }
#endif
        SaveData();
    }

    /// <summary>
    /// 캐릭터 장착 후 현재 장착 중인 캐릭터 번호 업데이트
    // /// </summary>
    public void UpdatePlayerCharData(int charNum)
    {
        _gameData.playerData.playerChar = charNum;
        SaveData();
    }

    /// <summary>
    /// 아이템 구매 후 ItemData의 isPurchased 업데이트
    /// 아이템 구매 후 playerData의 itemData 업데이트
    /// </summary>
    /// <param name="itemName"></param>
    public void UpdateItemPurchaseData(StoreData.ItemName itemName)
    {
        _gameData.storeData.itemData[(int)itemName].isPurchased = true;
        _gameData.playerData.playerItem -= _gameData.storeData.itemData[(int)itemName].price;
#if !UNITY_EDITOR
        if (!_gameData.achievement.isFirstItem)
        {
            GPGSBinder.Instance.UnlockAchievement(GPGSIds.achievement_purchase_first_item, success => _gameData.achievement.isFirstItem = true);
        }
        if (!_gameData.achievement.isCrownItem && (int)itemName == 5)
        {
            GPGSBinder.Instance.UnlockAchievement(GPGSIds.achievement_purchase_crown_item, success => _gameData.achievement.isCrownItem = true);
        }
#endif
        SaveData();
    }

    /// <summary>
    /// 아이템 장착 후 현재 장착 중인 아이템 업데이트 
    /// </summary>
    /// <param name="itemPart"></param>
    /// <param name="itemName"></param>
    public void UpdatePlayerItemData(StoreData.ItemPart itemPart, StoreData.ItemName itemName)
    {
        Debug.Log((int)itemPart);
        _gameData.playerData.itemData[(int)itemPart] = (int)itemName;
        SaveData();
    }
    
    /// <summary>
    /// 유료 아이템 구매 후 PaidItemData의 isPurchased 업데이트
    /// 유료 아이템 구매 후 각 상품의 isPurchased, isUnlocked 업데이트
    /// </summary>
    /// <param name="paidItemName"></param>
    public void UpdatePaidItemPurchaseData(StoreData.PaidItemName paidItemName)
    {
        _gameData.storeData.paidItemData[(int)paidItemName].isPurchased = true;
        // To do : link achievement
        SaveData();
    }

    public PlayerData GetPlayerData()
    {
        return _gameData.playerData;
    }

    public Achievement GetAchievementData()
    {
        return _gameData.achievement;
    }

    /*    public LeaderboardData GetLeaderboardData(int stageIdx, int levelIdx)
        {
            return _gameData.stageData[stageIdx].leaderboardData[levelIdx];
        }*/
}
