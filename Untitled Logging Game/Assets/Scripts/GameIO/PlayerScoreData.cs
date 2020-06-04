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
