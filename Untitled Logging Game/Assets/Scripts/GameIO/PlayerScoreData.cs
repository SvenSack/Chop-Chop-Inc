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
    public string Time;
    public string Date;
    public int AchievedLevel;
    public string FeedBack;

    public PlayerGameData(string name, float score,int achievedLevel = -1)
    {
        this.Name = name;
        this.Score = score;
        this.AchievedLevel = achievedLevel;

        DateTime now = DateTime.Now;

        Time = now.TimeOfDay.ToString();
        Date = now.Date.ToString();

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
}
