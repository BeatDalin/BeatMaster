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
        // Debug.Log(Application.persistentDataPath);
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
                temp.unlockCharNum = j + 1; // clear시 해금할 캐릭터 번호
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
        _gameData.storeData.charCount = 5; // temporary.. 상품 수 5개
        _gameData.storeData.characterData = new CharacterData[_gameData.storeData.charCount]; 
        
        CharacterData characterData = new CharacterData();
        for (int i = 0; i < _gameData.storeData.charCount; i++)
        {
            characterData.characterNum = i;
            characterData.price = 10;
            characterData.isPurchased = i == 0; // 첫 번째 캐릭터는 구매한 상태
            characterData.isUnlocked = i == 0; // 첫 번째 캐릭터는 해금 상태
            _gameData.storeData.characterData[i] = characterData;
        }
    }

    public StoreData GetStoreData()
    {
        return _gameData.storeData;
    }

    /// <summary>
    /// 캐릭터 구매 후 CharacterData의 isPurchased 업데이트
    /// 캐릭터 구매 후 gameData의 playerItem 업데이트
    /// </summary>
    /// <param name="charNum"></param>
    public void UpdateStorePurchaseData(int charNum)
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

    public PlayerData GetPlayerData()
    {
        return _gameData.playerData;
    }
}
