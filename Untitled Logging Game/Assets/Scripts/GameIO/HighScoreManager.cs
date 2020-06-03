using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public delegate void OnGameCompleted();

public class HighScoreManager : MonoBehaviour
{
    private List<PlayerScoreData> playerScoreData = new List<PlayerScoreData>();

    private static string fileName = "Scores.txt";

    private string savePath;

    private string currentPlayerName = "John Doe";
    private float currentPlayerScore;

    private InputField inputField;

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

    public void AddPlayerScoreData(PlayerScoreData scoreData)
    {
        playerScoreData.Add(scoreData);
    }

    public void AddCurrentPlayerScoreData()
    {
        PlayerScoreData scoreData = new PlayerScoreData(currentPlayerName, currentPlayerScore);
        playerScoreData.Add(scoreData);
    }

    public void SaveScoresToFile()
    {
        Debug.Log("File Exists " + File.Exists(savePath));

        playerScoreData.Sort(new PlayerScoreComparer());

        using (var writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
        {
            GameDataWriter dataWriter = new GameDataWriter(writer);

            dataWriter.Write(playerScoreData.Count);

            foreach (PlayerScoreData scoreData in playerScoreData)
            {
                scoreData.Save(dataWriter);
            }
        }

        Debug.Log("File Exists after " + File.Exists(savePath));
    }

    public void LoadScoresInFile()
    {
        using (var reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
        {
            GameDataReader dataReader = new GameDataReader(reader);

            int playerDataCount = dataReader.ReadInt();

            playerScoreData.Clear();

            for (int i = 0; i < playerDataCount; i++)
            {
                PlayerScoreData scoreData = new PlayerScoreData();
                scoreData.Load(dataReader);
                playerScoreData.Add(scoreData);
            }
        }
    }

    public List<PlayerScoreData> GetScoreData()
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

        PlayerScoreData randomData = new PlayerScoreData(name, score);
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
