using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class DataCenter : MonoBehaviour
{
    [SerializeField]
    private Data _gameData; // keep it private, and update it through method using stage & level number
    private PlayerData _playerData;
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
        //Debug.Log(Application.persistentDataPath);
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
        _gameData.playerData = _playerData;
        _gameData.stageData = new StageData[1]; // temporally, set array size as 1
        LevelData temp = new LevelData();
        for (int i = 0; i < _gameData.stageData.Length; i++)
        {
            _gameData.stageData[i].stage = i + 1;
            _gameData.stageData[i].levelData = new LevelData[5];

            for (int j = 0; j < _gameData.stageData[i].levelData.Length; j++)
            {
                temp.level = j + 1;
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

    public void UpdateStageData(int stageIdx)
    {
        StageData curStageData = _gameData.stageData[stageIdx];
        curStageData.bossClear = true;
    }
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

        _gameData.storeData.characterData = new List<CharacterData>(); // 일단 item 3개로 설정. 추후 변경 가능

        CharacterData characterData = new CharacterData();
        for (int i = 0; i < 5; i++)
        {
            characterData.characterNum = i;
            characterData.price = 10;
            characterData.isPurchased = false;
            characterData.unlockStage = 0;
            characterData.unlockLevel = i;
            characterData.isUnlocked = false;
            _gameData.storeData.characterData.Add(characterData);
        }
    }

    public StoreData GetStoreData()
    {
        return _gameData.storeData;
    }

    public void UpdateStorePurchaseData(int charNum)
    {
        for (int i = 0; i < _gameData.storeData.characterData.Count; i++)
        {
            CharacterData characterData = _gameData.storeData.characterData[i];
            if (characterData.characterNum == charNum)
            {
                characterData.isPurchased = true;
                _playerData.playerItem -= characterData.price;
                _gameData.storeData.characterData[i] = characterData;
                _playerData.playerChar = charNum;//임시
                _gameData.playerData = _playerData;
            }
        }
        SaveData();
    }

    /// <summary>
    /// StorePopup 오픈, item 구매 후, 장착 후 호출
    /// --> 필요한지 잘 모르겠음
    // /// </summary>
    public void UpdateStoreData()
    {
        StoreData storeData = _gameData.storeData;

        for (int i = 0; i < storeData.characterData.Count; i++)
        {
            var tempItemData = storeData.characterData[i];
            storeData.characterData[i] = tempItemData;
        }
        _gameData.storeData = storeData;

        SaveData();
    }

    public PlayerData GetPlayerData()
    {
        return _gameData.playerData;
    }

    /// <summary>
    /// LevelClear마다 호출됨. item의 Unlock여부를 Update해준다.
    /// </summary>
    /// <param name="stageNum"></param>
    /// <param name="levelNum"></param>
    public void UpdateItemData(int stageNum, int levelNum)
    {
        var tempItemData = _gameData.storeData.characterData;

        for (int i = 0; i < tempItemData.Count; i++)
        {
            int unlockStage = tempItemData[i].unlockStage;

            // if (_gameData.stageData[].levelData[tempItemData[i].unlockLevel].levelClear)
            // {
            //     tempItemData[i].isUnlocked = true;
            // }
        }
    }
}
