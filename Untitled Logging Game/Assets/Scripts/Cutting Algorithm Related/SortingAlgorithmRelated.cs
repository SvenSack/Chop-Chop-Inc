using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IntersectionComparer : IComparer<Vector3>
{
    private Vector3 baseDirection;
    public Vector3 basePosition;
    private Matrix4x4 world;


    public IntersectionComparer(Vector3 baseDirection, Vector3 basePosition, Matrix4x4 world)
    {
        this.baseDirection = baseDirection;
        this.basePosition = basePosition;

    }

    public int Compare(Vector3 intersectionPoint1, Vector3 intersectionPoint2)
    {
        float x = Vector3.Dot(baseDirection, intersectionPoint1 - basePosition);
        float y = Vector3.Dot(baseDirection, intersectionPoint2 - basePosition);



        return x.CompareTo(y);
    }

}

public class IndexDirectionComparer : IComparer<int>
{
    private Vector3 baseDirection;
    public Vector3 basePosition;
    private Mesh mesh;
    private Matrix4x4 world;

    public IndexDirectionComparer(Vector3 baseDirection, Vector3 basePosition, Mesh mesh, Matrix4x4 world)
    {
        this.baseDirection = baseDirection;
        this.basePosition = basePosition;
        this.mesh = mesh;
        this.world = world;
    }

    public int Compare(int indexA, int indexB)
    {
        Vector3 worldIndexA = world.MultiplyPoint(mesh.vertices[indexA]);
        Vector3 worldIndexB = world.MultiplyPoint(mesh.vertices[indexB]);


        float x = Vector3.Dot(baseDirection, (worldIndexA - basePosition).normalized);
        float y = Vector3.Dot(baseDirection, (worldIndexB - basePosition).normalized);

        if (Mathf.Abs(y - x) < 0.001f)
        {
            float xLength = Vector3.Distance(worldIndexA, basePosition);
            float yLength = Vector3.Distance(worldIndexB, basePosition);

            return xLength.CompareTo(yLength);

        }


        return x.CompareTo(y);
    }



}