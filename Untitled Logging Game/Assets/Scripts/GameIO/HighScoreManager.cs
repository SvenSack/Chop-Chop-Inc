using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public delegate void OnGameCompleted();

public class HighScoreManager : MonoBehaviour
{
    private List<PlayerGameData> playerScoreData = new List<PlayerGameData>();

    private static string fileName = "Scores.txt";

    private string savePath;

    private string currentPlayerName = "John Doe";
    private float currentPlayerScore;

    private InputField inputField;

    private int loginCount;

    void Awake()
    {
        savePath = Path.Combine(Application.persistentDataPath, fileName);
        inputField = FindObjectOfType<InputField>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.S))
        {
            Debug.Log("Saving");
            SaveScoresToFile();
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

    public void SetPlayerScore(float newScore)
    {
        currentPlayerScore = newScore;
    }

    public void AddPlayerScoreData(PlayerGameData scoreData)
    {
        playerScoreData.Add(scoreData);
    }

    public void AddCurrentPlayerScoreData()
    {
        PlayerGameData scoreData = new PlayerGameData(currentPlayerName, currentPlayerScore);
        playerScoreData.Add(scoreData);
    }

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

    public void LoadScoresInFile()
    {
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
        currentPlayerScore = Random.Range(0, 40.0f);

        AddCurrentPlayerScoreData();
        SaveScoresToFile();
    }

    public void DEBUG_ClearAndSave()
    {
        playerScoreData.Clear();
        SaveScoresToFile();
    }
}
