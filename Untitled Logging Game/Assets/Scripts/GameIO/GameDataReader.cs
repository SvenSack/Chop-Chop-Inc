using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameDataReader
{
    StreamReader reader;

    public GameDataReader(StreamReader reader)
    {
        this.reader = reader;
    }

    public string ReadString()
    {
        return reader.ReadLine();
    }





}
