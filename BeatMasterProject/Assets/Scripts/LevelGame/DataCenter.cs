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

    void Start()
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
        _gameData.playerLv = 1;
        _gameData.playerStage = 1;
        _gameData.stageData = new StageData[1]; // temporally, set array size as 2
        LevelData temp = new LevelData();
        for (int i = 0; i < _gameData.stageData.Length; i++)
        {
            _gameData.stageData[i].stage = i+1;
            _gameData.stageData[i].levelData = new LevelData[5];
            
            for (int j = 0; j < _gameData.stageData[i].levelData.Length; j++)
            {
                temp.level = j+1;
                _gameData.stageData[i].levelData[j] = temp;
            }
        }

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
        _gameData.playerStage = stageNum;
        _gameData.playerLv = levelNum;
        _gameData.playerItem += playerItem;
        SaveData();
    }
}