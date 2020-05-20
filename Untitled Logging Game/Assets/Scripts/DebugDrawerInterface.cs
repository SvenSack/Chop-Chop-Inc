using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class DrawObject
{
    protected Color objectColor;
    public abstract void Draw();
}

public class LineInstance : DrawObject
{
    private Vector3 start;
    private Vector3 end;
    private Color color;

    public LineInstance(Vector3 start,Vector3 end,Color color)
    {
        this.start = start;
        this.end = end;
        this.color = color;
    }

    public override void Draw()
    {
        Gizmos.color = color;
        Gizmos.DrawLine(start, end);
    }
}

public class DebugDrawerInterface : MonoBehaviour
{
    List<LineInstance> lines = new List<LineInstance>();

    public void AddLine(Vector3 startLine,Vector3 endLine,Color color)
    {
        lines.Add(new LineInstance(startLine, endLine, color));
    }

    public void ClearLines()
    {
        lines.Clear();
    }

    private void OnDrawGizmos()
    {
        foreach(LineInstance line in lines)
        {
            line.Draw();
        }
    }

}
