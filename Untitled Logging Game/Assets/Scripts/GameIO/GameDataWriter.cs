using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GameDataWriter
{
    private BinaryWriter writer;

    public GameDataWriter(BinaryWriter writer)
    {
        this.writer = writer;
    }

    public void Write(float value)
    {
        writer.Write(value);
    }

    public void Write(int value)
    {
        writer.Write(value);
    }

    public void Write(string value)
    {
        writer.Write(value);
    }



}
