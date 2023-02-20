using System;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataCenter : MonoBehaviour
{
    [SerializeField]
    private Data _gameData; // keep it private, and update it through method using stage & level number
    private PlayerData _playerData;
    private ItemData[] itemData;
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
        _playerData.playerLv = 1;
        _playerData.playerStage = 1;
        _playerData.playerChar = 0; // default character index
        _playerData.itemData = new int[Enum.GetValues(typeof(StoreData.ItemPart)).Length]; // 부위별(index) 현재 장착중인 item(value)
        _playerData.itemData = Enumerable.Repeat(-1, Enum.GetValues(typeof(StoreData.ItemPart)).Length).ToArray(); // 미장착 상태일 때 -1

        _gameData.playerData = _playerData;
        _gameData.stageData = new StageData[4]; // temporally, set array size as 1
        LevelData temp = new LevelData();
        for (int i = 0; i < _gameData.stageData.Length; i++)
        {
            _gameData.stageData[i].stage = i + 1;
            _gameData.stageData[i].levelData = new LevelData[4];

            for (int j = 0; j < _gameData.stageData[i].levelData.Length; j++)
            {
                temp.level = j + 1;
                temp.isUnlocked = j == 0;
                temp.unlockCharNum = i == 0 ? j : 0; // 레벨1은 스테이지 번호대로 캐릭터 해금, 아니면 기본캐릭터 index 부여)
                _gameData.stageData[i].levelData[j] = temp;
            }
        }

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
        _gameData.storeData.charCount = Enum.GetValues(typeof(CharacterNum)).Length;
        _gameData.storeData.characterData = new CharacterData[_gameData.storeData.charCount];
        CreateCharacterData();

        _gameData.storeData.itemCount = Enum.GetValues(typeof(StoreData.ItemName)).Length;
        _gameData.storeData.itemData = new ItemData[_gameData.storeData.itemCount];
        CreateItemData();
    }

    private void CreateCharacterData()
    {
        CharacterData[] tempCharacterData = _gameData.storeData.characterData;

        int[] charPrice = { 0, 15, 20 };

        for (int i = 0; i < _gameData.storeData.charCount; i++)
        {
            tempCharacterData[i].characterNum = i;
            tempCharacterData[i].price = charPrice[i];
            tempCharacterData[i].unlockStage = 0;
            tempCharacterData[i].unlockLevel = i == 0 ? -1 : i - 1;
            tempCharacterData[i].isPurchased = i == 0;
            tempCharacterData[i].isUnlocked = i == 0;
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
            StoreData.ItemPart.Neck,
            StoreData.ItemPart.Face,
            StoreData.ItemPart.Head,
            StoreData.ItemPart.Head,
            StoreData.ItemPart.Head,
            StoreData.ItemPart.Head,
            StoreData.ItemPart.Background,
            StoreData.ItemPart.Head,
            StoreData.ItemPart.Background,
            StoreData.ItemPart.Background,
        };
        int[] itemPrice = { 10, 10, 10, 20, 20, 30 };

        for (int i = 0; i < _gameData.storeData.itemCount; i++)
        {
            tempItemData[i].itemName = itemList[i];
            tempItemData[i].itemPart = itemParts[i];
            tempItemData[i].isUnlocked = (int)tempItemData[i].itemName < (int)StoreData.ItemName.Crown; // crown전까지 unlocked true
            tempItemData[i].price = tempItemData[i].isUnlocked ? itemPrice[i] : 0;
            tempItemData[i].isPurchased = false;
        }

        _gameData.storeData.itemData = tempItemData;
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

    public PlayerData GetPlayerData()
    {
        return _gameData.playerData;
    }
}
