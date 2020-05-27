using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Jobs;
using Unity.Collections;

struct JobFaceGrouping
{




}

struct JobFace
{
    JobVertex v10;
    JobVertex v11;
    JobVertex v12;

    JobVertex v20;
    JobVertex v21;
    JobVertex v22;
}

struct JobVertex
{
    Vector3 position;
    Vector3 normal;
    Vector2 uv;
}


struct faceToPrimitiveMeshJob : IJobParallelFor
{
    [ReadOnly]
    NativeArray<Face> faces;

    [ReadOnly]
    public NativeArray<int> meshTriangles;
    [ReadOnly]
    public NativeArray<Vector3> meshNormals;
    [ReadOnly]
    public NativeArray<Vector3> meshVertices;

    Vector3 transformedPosition;
    Vector3 transformedNormal;

    Matrix4x4 worldMatrix;

    public void Execute(int i)
    {
        TriangleSplitState tri1CheckResult = TriangleSplitState.DefaultNoTriangle;
        TriangleSplitState tri2CheckResult = TriangleSplitState.DefaultNoTriangle;

        bool hasTriangle1 = faces[i].tri1.v0 != -1;

        if (hasTriangle1)
        {
            tri1CheckResult = triangleToPlaneCheck(
                meshVertices[faces[i].tri1.v0],
                meshVertices[faces[i].tri1.v1],
                meshVertices[faces[i].tri1.v2],
                transformedPosition, transformedNormal);

        }

        bool hasTriangle2 = faces[i].tri2.v0 != -1;

        if (hasTriangle2)
        {
            tri2CheckResult = triangleToPlaneCheck(
                meshVertices[faces[i].tri2.v0],
                meshVertices[faces[i].tri2.v1],
                meshVertices[faces[i].tri2.v2],
                transformedPosition, transformedNormal);
        }

        bool isBothTrianglesExist = hasTriangle1 && hasTriangle2;

        ////if both triangles exist and have the same triangle split state
        //if (isBothTrianglesExist && tri1CheckResult == tri2CheckResult)
        //{
        //    organizeFaceBasedOnTriangleSplitState(worldMatrix, tri1CheckResult, face, position, normal, upperMesh, lowerMesh);
        //}
        ////if both triangles exist but one triangle is intersecting but the other is either above or below the splitting plane
        //else if (isBothTrianglesExist && tri1CheckResult != tri2CheckResult)
        //{

        //    Vector3 worldV0 = worldMatrix.MultiplyPoint(mesh.vertices[face.tri1.v0]);
        //    Vector3 worldV1 = worldMatrix.MultiplyPoint(mesh.vertices[face.tri1.v1]);
        //    Vector3 worldV2 = worldMatrix.MultiplyPoint(mesh.vertices[face.tri1.v2]);

        //    Vector3 worldV3 = worldMatrix.MultiplyPoint(mesh.vertices[face.tri2.v0]);
        //    Vector3 worldV4 = worldMatrix.MultiplyPoint(mesh.vertices[face.tri2.v1]);
        //    Vector3 worldV5 = worldMatrix.MultiplyPoint(mesh.vertices[face.tri2.v2]);

        //    Vector3[] worldTrianglePointPositions = new Vector3[6];
        //    worldTrianglePointPositions[0] = worldV0;
        //    worldTrianglePointPositions[1] = worldV1;
        //    worldTrianglePointPositions[2] = worldV2;
        //    worldTrianglePointPositions[3] = worldV3;
        //    worldTrianglePointPositions[4] = worldV4;
        //    worldTrianglePointPositions[5] = worldV5;

        //    intersectingFaceSplit(worldMatrix, face, position, normal, worldTrianglePointPositions, lowerMesh, upperMesh);

        //}
        ////one of the triangles in the face do not exist ( this face only has one triangle)
        //else if (!isBothTrianglesExist)
        //{
        //    if (hasTriangle1)
        //    {
        //        FindDecisionForSingularTriangle(worldMatrix, tri1CheckResult, face.tri1, position, normal, lowerMesh, upperMesh);
        //    }
        //    if (hasTriangle2)
        //    {
        //        FindDecisionForSingularTriangle(worldMatrix, tri2CheckResult, face.tri2, position, normal, lowerMesh, upperMesh);
        //    }
        //}
    }

    private TriangleSplitState triangleToPlaneCheck(Vector3 transformedV0, Vector3 transformedV1, Vector3 transformedV2, Vector3 position, Vector3 normal)
    {
        bool v0AbovePlane = Utils.IsPointAbovePlane(transformedV0, position, normal);
        bool v1AbovePlane = Utils.IsPointAbovePlane(transformedV1, position, normal);
        bool v2AbovePlane = Utils.IsPointAbovePlane(transformedV2, position, normal);

        if (v0AbovePlane && v1AbovePlane && v2AbovePlane)
        {
            return TriangleSplitState.AbovePlane;
        }
        else if (!v0AbovePlane && !v1AbovePlane && !v2AbovePlane)
        {
            return TriangleSplitState.BelowPlane;
        }
        else
        {
            return TriangleSplitState.IntersectionOnPlane;
        }

    }

    private void organizeFaceBasedOnTriangleSplitState(Matrix4x4 world, TriangleSplitState state, Face face, Vector3 position,
        Vector3 normal, PrimitiveMesh upperPrimitiveMesh, PrimitiveMesh lowerPrimitiveMesh)
    {

        Vector3 worldV0 = world.MultiplyPoint(meshVertices[face.tri1.v0]);
        Vector3 worldV1 = world.MultiplyPoint(meshVertices[face.tri1.v1]);
        Vector3 worldV2 = world.MultiplyPoint(meshVertices[face.tri1.v2]);

        Vector3 worldV3 = world.MultiplyPoint(meshVertices[face.tri2.v0]);
        Vector3 worldV4 = world.MultiplyPoint(meshVertices[face.tri2.v1]);
        Vector3 worldV5 = world.MultiplyPoint(meshVertices[face.tri2.v2]);

        //switch (state)
        //{
        //    case TriangleSplitState.AbovePlane:
        //        upperPrimitiveMesh.AddFaceFrom(this, face);

        //        break;

        //    case TriangleSplitState.BelowPlane:
        //        lowerPrimitiveMesh.AddFaceFrom(this, face);

        //        break;

        //    case TriangleSplitState.IntersectionOnPlane:

        //        Vector3[] worldTrianglePointPositions = new Vector3[6];
        //        worldTrianglePointPositions[0] = worldV0;
        //        worldTrianglePointPositions[1] = worldV1;
        //        worldTrianglePointPositions[2] = worldV2;
        //        worldTrianglePointPositions[3] = worldV3;
        //        worldTrianglePointPositions[4] = worldV4;
        //        worldTrianglePointPositions[5] = worldV5;


        //        intersectingFaceSplit(world, face, position, normal, worldTrianglePointPositions, lowerPrimitiveMesh, upperPrimitiveMesh);

        //        break;



        //}
    }
}
