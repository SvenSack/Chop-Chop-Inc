using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;



public class GameStatistics
{
    public GameStatistics(int scoreCount,int loginCount)
    {
        this.scoresStored = scoreCount;
        this.loginCount = loginCount;
    }

    public int scoresStored;
    public int loginCount;

}

/// <summary>
/// Stores all of the inforamtion that will be saved in file
/// </summary>
public class PlayerGameData
{
    public string Name;
    public float Score;
   
    public int AchievedLevel;
    public string FeedBack;

    public int treesCut;
    public int treesPlanted;

    public int highestComboCut;
    public int highestComboPlant;

    //automatically set
    public string Time;
    public string Date;

    public PlayerGameData(string name, float score,int achievedLevel = -1)
    {
        this.Name = name;
        this.Score = score;
        this.AchievedLevel = achievedLevel;

        DateTime now = DateTime.Now;

        Time = now.TimeOfDay.ToString();
        Date = now.Date.ToString();

    }

    public void SetLevelGameData(LevelScoreData data)
    {
        treesCut = data.treesCut;
        treesPlanted = data.treesPlanted;

        highestComboCut = data.highestComboCut;
        highestComboPlant = data.highestComboPlant;

    }

    public PlayerGameData() { }

}

public class PlayerScoreComparer : IComparer<PlayerGameData>
{
    public int Compare(PlayerGameData a, PlayerGameData b)
    {
        return b.Score.CompareTo(a.Score);
    }
}



public class LevelScoreData
{
    public int treesCut;
    public int treesPlanted;
    public float score;

    public int highestComboCut;
    public int highestComboPlant;

    public float difficulty;

    public LevelScoreData()
    {
        this.treesCut = 0;
        this.treesPlanted =0;
        this.score = 0;

        this.highestComboCut = 0;
        this.highestComboPlant = 0;

        this.difficulty = 0;
    }

    public LevelScoreData(int treesCut,int treesPlanted,float score,int highestComboCut,int highestComboPlant,float difficulty)
    {
        this.treesCut = treesCut;
        this.treesPlanted = treesPlanted;
        this.score = score;

        this.highestComboCut = highestComboCut;
        this.highestComboPlant = highestComboPlant;

        this.difficulty = difficulty;

    }

    public static LevelScoreData operator +(LevelScoreData a,LevelScoreData b)
    {
        LevelScoreData result = new LevelScoreData();

        result.highestComboCut = a.highestComboCut + b.highestComboCut;
        result.highestComboPlant = a.highestComboPlant + b.highestComboPlant;

        result.treesCut = a.treesCut + b.treesCut;
        result.treesPlanted = a.treesPlanted + b.treesPlanted;

        result.score = a.score + b.score;

        return result;


    }

}
