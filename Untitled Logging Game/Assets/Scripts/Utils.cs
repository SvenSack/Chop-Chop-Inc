using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utils : MonoBehaviour
{
    static public void GetMinMaxOfVertices<T>(out Vector3 min, out Vector3 max, T collection) where T : ICollection<Vector3>
    {
        Vector3 minResult = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 maxResult = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        foreach (Vector3 vertex in collection)
        {
            //check for minResult
            if (vertex.x < minResult.x)
            {
                minResult.x = vertex.x;
            }

            if (vertex.y < minResult.y)
            {
                minResult.y = vertex.y;
            }

            if (vertex.z < minResult.z)
            {
                minResult.z = vertex.z;
            }



            //check for maxResult
            if (vertex.x > maxResult.x)
            {
                maxResult.x = vertex.x;
            }

            if (vertex.y > maxResult.y)
            {
                maxResult.y = vertex.y;
            }

            if (vertex.z > maxResult.z)
            {
                maxResult.z = vertex.z;
            }

        }

        min = minResult;
        max = maxResult;
    }

    static public void CustomLineToPlaneIntersection(Vector3 start, Vector3 end, Vector3 planePosition, Vector3 planeNormal, out Vector3 intersection,out float t)
    {
        Vector3 lineToUse = start - end;

        Vector3 P0 = start;
        Vector3 P1 = lineToUse.normalized;
        Vector3 A = planePosition;

        t = (Vector3.Dot(A, planeNormal) - Vector3.Dot(P0, planeNormal)) / Vector3.Dot(P1, planeNormal);

        intersection = P0 + P1 * t;


    }

    static public bool IsPointAbovePlane(Vector3 pointPosition, Vector3 planePosition, Vector3 normal)
    {
        return Vector3.Dot(pointPosition - planePosition, normal) > 0;
    }

    static public bool IsPointAbovePlane(Vector3 pointPosition, Vector3 planePosition, Vector3 normal, out PointSplitState state)
    {
        bool isPointAbovePlane = Utils.IsPointAbovePlane(pointPosition, planePosition, normal);

        if (isPointAbovePlane)
        {
            state = PointSplitState.AbovePlane;
        }
        else
        {
            state = PointSplitState.BelowPlane;
        }

        return isPointAbovePlane;

    }


}
