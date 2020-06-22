using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnGameCompleted();

public class HighScoreManager : MonoBehaviour
{
    public int currentLevel = 0;
    private static readonly int maxLevel = 3;

    private List<PlayerGameData> playerScoreData = new List<PlayerGameData>();

    private static readonly string fileName = "Scores.txt";
    private static readonly string currentPlayerData = "CurrentPlayer.txt";

    private string savePath;
    private string currentPlayerDataPath;

    private string currentPlayerName = "Unfilled";
    private LevelScoreData[] currentPlayerScores = new LevelScoreData[maxLevel];

    private InputField inputField;

    private int loginCount;

    void Awake()
    {
        for (int i = 0; i < currentPlayerScores.Length; i++)
        {
            currentPlayerScores[i] = new LevelScoreData();
        }

        savePath = Path.Combine(Application.persistentDataPath, fileName);
        currentPlayerDataPath = Path.Combine(Application.persistentDataPath, currentPlayerData);

        inputField = FindObjectOfType<InputField>();

        LoadCurrentLevelScore();
    }


    private void OnApplicationQuit()
    {
        loginCount++;
        ResetLevelDataInFile();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Saving");
            SetPlayerScore(3, 4, 5, 6, 7,1);


            SaveCurrentLevelDataToFile();
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Loading");
            LoadScoresInFile();
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            DEBUG_AddRandomPlayerData();
        }

        if (Input.GetKeyDown(KeyCode.M))
        {
            Debug.Log("DEBUG_ClearAndSave");
            DEBUG_ClearAndSave();
        }

        if(Input.GetKeyDown(KeyCode.T))
        {
            Debug.Log("DEBUG_AddAndSaveCurrentPlayerData()");
            DEBUG_AddAndSaveCurrentPlayerData();
        }
    }

    public void Initialize()
    {
        LoadScoresInFile();
    }

    public void SetPlayerScore(int treesCut, int treesPlanted, float score, int highestComboCut, int highestComboPlant,float difficulty)
    {
        LevelScoreData scoreData = new LevelScoreData
            (treesCut,
            treesPlanted,
            score,
            highestComboCut,
            highestComboPlant,
            difficulty);

        currentPlayerScores[currentLevel] = scoreData;
    }

    public void SetLevelDifficulty(float difficulty)
    {
        currentPlayerScores[currentLevel].difficulty = difficulty;
    }

    public float GetLevelDifficulty(int level)
    {
        return currentPlayerScores[level].difficulty;
    }

    public void AddPlayerScoreData(PlayerGameData scoreData)
    {
        playerScoreData.Add(scoreData);
    }

    public void AddCurrentPlayerScoreData()
    {
        float sum = 0;
        foreach(var levelData in currentPlayerScores)
        {
            sum += levelData.score;
        }

        PlayerGameData scoreData = new PlayerGameData(currentPlayerName, sum);
        playerScoreData.Add(scoreData);
    }

    //--------------------------------- Saving related stuff ------------------------------------------//
    public void SaveScoresToFile()
    {
        playerScoreData.Sort(new PlayerScoreComparer());

        using (var writer = new StreamWriter(File.Open(savePath, FileMode.Create)))
        {
            GameDataWriter dataWriter = new GameDataWriter(writer);

            GameStatistics ScoreCountObj = new GameStatistics(playerScoreData.Count, loginCount);

            string jsonScoreDataCount = JsonUtility.ToJson(ScoreCountObj);
            dataWriter.Write(jsonScoreDataCount);

            foreach (PlayerGameData scoreData in playerScoreData)
            {
                string jsonScoreData = JsonUtility.ToJson(scoreData);
                dataWriter.Write(jsonScoreData);
            }
        }
    }

    public void SaveCurrentLevelDataToFile()
    {
        SaveLevelDataToFile(currentPlayerScores);
    }

    public void ResetLevelDataInFile()
    {
        LevelScoreData[] emptyScoreData = new LevelScoreData[maxLevel];

        for (int i = 0; i < emptyScoreData.Length; i++)
        {
            emptyScoreData[i] = new LevelScoreData();
        }

        SaveLevelDataToFile(emptyScoreData);

    }

    public void SaveLevelDataToFile(LevelScoreData[] scoreArray)
    {
        using (var writer = new StreamWriter(File.Open(currentPlayerDataPath, FileMode.Create)))
        {
            GameDataWriter dataWriter = new GameDataWriter(writer);

            for (int i = 0; i < maxLevel; i++)
            {
                string jsonScoreData = JsonUtility.ToJson(scoreArray[i]);
                dataWriter.Write(jsonScoreData);
            }
        }
    }

    
    //------------------------------------- Loading related stuff ------------------------------------------//
    public void LoadScoresInFile()
    {
        if(!File.Exists(savePath))
        {
            SaveScoresToFile();
        }

        using (var reader = new StreamReader(File.Open(savePath, FileMode.Open)))
        {
            Debug.Log("Loading Scores");

            GameDataReader dataReader = new GameDataReader(reader);

            string strPlayerDataCount = dataReader.ReadString();
            GameStatistics playerDataCount = JsonUtility.FromJson<GameStatistics>(strPlayerDataCount);

            loginCount = playerDataCount.loginCount;

            playerScoreData.Clear();

            for (int i = 0; i < playerDataCount.scoresStored; i++)
            {
                string scoreDataStr = dataReader.ReadString();
                PlayerGameData scoreData = JsonUtility.FromJson<PlayerGameData>(scoreDataStr);

                Debug.Log("Loaded " + scoreDataStr);

                playerScoreData.Add(scoreData);
            }
        }
    }


    public void LoadCurrentLevelScore()
    {
        if(!File.Exists(currentPlayerDataPath))
        {
            ResetLevelDataInFile();
        }

        using (var reader = new StreamReader(File.Open(currentPlayerDataPath, FileMode.Open)))
        {
            Debug.Log("Loading Scores");

            GameDataReader dataReader = new GameDataReader(reader);

            for (int i = 0; i < maxLevel; i++)
            {
                string levelDataStr = dataReader.ReadString();
                LevelScoreData scoreData = JsonUtility.FromJson<LevelScoreData>(levelDataStr);
            }
        }
    }



    public List<PlayerGameData> GetScoreData()
    {
        return playerScoreData;
    }

    public void OnEditEnd(string str)
    {
        if(inputField)
        {
            currentPlayerName = inputField.text;
            Debug.Log("currentPlayerName  " + currentPlayerName);
        }
        else
        {
            Debug.LogError("No InputField found!");
        }
    }

    public void DEBUG_AddRandomPlayerData()
    {
        string alphabet = "abcdefghijklmnopqrstuvwxyz";

        int count = Random.Range(3, 10);
        string name = "";

        for (int i = 0; i < count; i++)
        {
            name += alphabet[Random.Range(0, 25)];
        }

        float score = Random.Range(0, 40.0f);

        PlayerGameData randomData = new PlayerGameData(name, score);
        Debug.Log("Added " + name + " with score " + score);

        playerScoreData.Add(randomData);
    }

    public void DEBUG_AddAndSaveCurrentPlayerData()
    {
        Debug.Log("Adding player name " + currentPlayerName);
        currentPlayerScores[currentLevel].score = Random.Range(0, 40.0f);

        AddCurrentPlayerScoreData();
        SaveScoresToFile();
    }

    public void DEBUG_ClearAndSave()
    {
        playerScoreData.Clear();
        SaveScoresToFile();
    }
}
