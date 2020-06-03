using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerScoreData
{
    public string Name;
    public float Score;



    public PlayerScoreData(string name, float score)
    {
        this.Name = name;
        this.Score = score;
    }

    public PlayerScoreData() { }

    public void Save(GameDataWriter dataWriter)
    {
        dataWriter.Write(Name);
        dataWriter.Write(Score);

        Debug.Log("Saved " + Name + "," + Score);

    }

    public void Load(GameDataReader dataReader)
    {
        Name = dataReader.ReadString();
        Score = dataReader.ReadFloat();

        Debug.Log("Loaded " + Name + "," + Score);


    }


}

public class PlayerScoreComparer : IComparer<PlayerScoreData>
{
    public int Compare(PlayerScoreData a, PlayerScoreData b)
    {
        return b.Score.CompareTo(a.Score);
    }
}
