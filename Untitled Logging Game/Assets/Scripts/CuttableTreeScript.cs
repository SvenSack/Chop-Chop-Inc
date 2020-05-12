using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


public class IntersectionComparer : IComparer<Vector3>
{
    private Vector3 baseDirection;
    private Vector3 basePosition;

    public IntersectionComparer(Vector3 baseDirection,Vector3 basePosition)
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
    private Vector3 basePosition;
    private Mesh mesh;

    public IndexDirectionComparer(Vector3 baseDirection, Vector3 basePosition,Mesh mesh)
    {
        this.baseDirection = baseDirection;
        this.basePosition = basePosition;
        this.mesh = mesh;
    }

    public int Compare(int indexA, int indexB)
    {

        float x = Vector3.Dot(baseDirection, mesh.vertices[indexA] - basePosition);
        float y = Vector3.Dot(baseDirection, mesh.vertices[indexB] - basePosition);

        return x.CompareTo(y);
    }



}



public struct Triangle
{
    public void Init()
    {
        v0 = -1;
        v1 = -1;
        v2 = -1;
    }

    public void InsureVertexOrdering(int indexA,int indexB,Mesh mesh)
    {
        Vector3 trueNormal = mesh.normals[v0];

        Vector3 foundNormal = Vector3.Cross(mesh.vertices[v1] - mesh.vertices[v0], mesh.vertices[v2] - mesh.vertices[v0]);

        Debug.Log("[bfr flipping]Checking for vertex at: ");
        Debug.Log("vert " + mesh.vertices[v0]);
        Debug.Log("vert " + mesh.vertices[v1]);
        Debug.Log("vert " + mesh.vertices[v2]);
        Debug.Log("Vector3.Dot(trueNormal,foundNormal)" + Vector3.Dot(trueNormal, foundNormal));

        if (Vector3.Dot(trueNormal,foundNormal) < 0)
        {
            
            FlipVertices(indexA, indexB);
        }

    }

    public void FlipVertices(int indexA, int indexB)
    {
        int[] indexArray = new int[3];
        indexArray[0] = v0;
        indexArray[1] = v1;
        indexArray[2] = v2;

        int temp = indexArray[indexA];
        indexArray[indexA] = indexArray[indexB];
        indexArray[indexB] = temp;

        v0 = indexArray[0];
        v1 = indexArray[1];
        v2 = indexArray[2];
    }

    public int v0, v1, v2;
}

public struct Face
{
    public void Init()
    {
        tri1.Init();
        tri2.Init();
    }

    public int[] ToIntArray()
    {
        int[] result = new int[6];

        result[0] = tri1.v0;
        result[1] = tri1.v1;
        result[2] = tri1.v2;

        result[3] = tri2.v0;
        result[4] = tri2.v1;
        result[5] = tri2.v2;

        return result;
    }

    public Triangle tri1;
    public Triangle tri2;
}

public class TreeSplitCollisionBox
{
    public List<Face> faces;

    public Vector3 min;
    public Vector3 max;

}

public enum TriangleSplitState
{
    BelowPlane,
    AbovePlane,
    IntersectionOnPlane,
    DefaultNoTriangle
}

public enum PointSplitState
{
    BelowPlane,
    AbovePlane
}

public struct IndividualVertex
{
    public Vector3 position;
    public Vector3 normal;
    public Vector2 UV;


    public void populateThisWithMeshAtIndex(Mesh mesh,int i)
    {
        position = mesh.vertices[i];
        normal = mesh.normals[i];
        UV = mesh.uv[i];
    }
}

public class IndividualTriangle
{
    public IndividualVertex V0;
    public IndividualVertex V1;
    public IndividualVertex V2;

    public IndividualTriangle()
    {

    }

    public IndividualTriangle(IndividualVertex v0,IndividualVertex v1,IndividualVertex v2)
    {
        V0 = v0;
        V1 = v1;
        V2 = v2;
    }
    //checks if Individual Triangle
    public bool AttemptNormalBasedVertexCorrection(Vector3 actualNormal,int flipIndexA,int flipIndexB)
    {
        if(Vector3.Dot(actualNormal,Vector3.Cross(V1.position - V0.position, V2.position - V0.position)) < 0)
        {
            FlipTriangle(flipIndexA, flipIndexB);
            return true;

        }
        return false;

    }

    public void FlipTriangle(int flipIndexA,int flipIndexB)
    {
        IndividualVertex[] vertices = new IndividualVertex[3];
        IndividualVertex tempVertex;

        vertices[0] = V0;
        vertices[1] = V1;
        vertices[2] = V2;

        tempVertex = vertices[flipIndexB];

        vertices[flipIndexB] = vertices[flipIndexA];
        vertices[flipIndexA] = tempVertex;

        V0 = vertices[0];
        V1 = vertices[1];
        V2 = vertices[2];
    }

    

}

public class IndividualFace
{
    public IndividualFace(IndividualTriangle tri1,IndividualTriangle tri2)
    {
        this.tri1 = tri1;
        this.tri2 = tri2;
    }

    public IndividualTriangle tri1;
    public IndividualTriangle tri2;

    public bool isFaceComplete = false;
}

public class PrimitiveMesh
{
    public List<IndividualFace> individualFaces;

    public void AddFaceFromSingularTriangle(IndividualTriangle tri1)
    {
        IndividualFace newFace = new IndividualFace(tri1, null);
        newFace.isFaceComplete = false;

        individualFaces.Add(newFace);
    }

    public void AddFaceFromTriangles(IndividualTriangle tri1, IndividualTriangle tri2)
    {
        IndividualFace newFace = new IndividualFace(tri1,tri2);
        newFace.isFaceComplete = true;

        individualFaces.Add(newFace);
    }

    public void AddFaceFrom(CuttableTreeScript cts,Face face)
    {
        Mesh mesh = cts.GetMesh();

        IndividualVertex V0;
        IndividualVertex V1;
        IndividualVertex V2;

        V0.position = mesh.vertices[face.tri1.v0];
        V1.position = mesh.vertices[face.tri1.v1];
        V2.position = mesh.vertices[face.tri1.v2];

        V0.normal = mesh.normals[face.tri1.v0];
        V1.normal = mesh.normals[face.tri1.v1];
        V2.normal = mesh.normals[face.tri1.v2];

        V0.UV = mesh.uv[face.tri1.v0];
        V1.UV = mesh.uv[face.tri1.v1];
        V2.UV = mesh.uv[face.tri1.v2];

        IndividualVertex V3;
        IndividualVertex V4;
        IndividualVertex V5;

        V3.position = mesh.vertices[face.tri2.v0];
        V4.position = mesh.vertices[face.tri2.v1];
        V5.position = mesh.vertices[face.tri2.v2];

        V3.normal = mesh.normals[face.tri2.v0];
        V4.normal = mesh.normals[face.tri2.v1];
        V5.normal = mesh.normals[face.tri2.v2];

        V3.UV = mesh.uv[face.tri2.v0];
        V4.UV = mesh.uv[face.tri2.v1];
        V5.UV = mesh.uv[face.tri2.v2];


        IndividualTriangle tri1 = new IndividualTriangle(V0,V1,V2);
        IndividualTriangle tri2 = new IndividualTriangle(V3,V4,V5);


        IndividualFace newFace = new IndividualFace(tri1,tri2);
        newFace.isFaceComplete = true;

        individualFaces.Add(newFace);
    }

    public void AddTriangleFrom(CuttableTreeScript cts, Triangle triangle)
    {
        Mesh mesh = cts.GetMesh();

        IndividualVertex V0;
        IndividualVertex V1;
        IndividualVertex V2;

        V0.position = mesh.vertices[triangle.v0];
        V1.position = mesh.vertices[triangle.v1];
        V2.position = mesh.vertices[triangle.v2];

        V0.normal = mesh.normals[triangle.v0];
        V1.normal = mesh.normals[triangle.v1];
        V2.normal = mesh.normals[triangle.v2];

        V0.UV = mesh.uv[triangle.v0];
        V1.UV = mesh.uv[triangle.v1];
        V2.UV = mesh.uv[triangle.v2];

        IndividualVertex V3;
        IndividualVertex V4;
        IndividualVertex V5;

        V3.position = Vector3.zero;
        V4.position = Vector3.zero;
        V5.position = Vector3.zero;

        V3.normal = Vector3.zero;
        V4.normal = Vector3.zero;
        V5.normal = Vector3.zero;

        V3.UV = Vector2.zero;
        V4.UV = Vector2.zero;
        V5.UV = Vector2.zero;


        IndividualTriangle tri1 = new IndividualTriangle(V0,V1,V2);
        IndividualTriangle tri2 = new IndividualTriangle(V3,V4,V5);


        IndividualFace newFace = new IndividualFace(tri1,tri2);
        newFace.isFaceComplete = false;

        individualFaces.Add(newFace);



    }

    public void PopulateMesh(Mesh mesh)
    {

        List<Vector3> verticesList = new List<Vector3>();
        List<Vector3> normalList = new List<Vector3>();
        List<Vector2> UVList = new List<Vector2>();
        List<int> indicesList = new List<int>();

        for (int i = 0; i < individualFaces.Count; i++)
        {
            //fill in first triangle
            verticesList.Add(individualFaces[i].tri1.V0.position);
            verticesList.Add(individualFaces[i].tri1.V1.position);
            verticesList.Add(individualFaces[i].tri1.V2.position);

            normalList.Add(individualFaces[i].tri1.V0.normal);
            normalList.Add(individualFaces[i].tri1.V1.normal);
            normalList.Add(individualFaces[i].tri1.V2.normal);

            UVList.Add(individualFaces[i].tri1.V0.UV);
            UVList.Add(individualFaces[i].tri1.V1.UV);
            UVList.Add(individualFaces[i].tri1.V2.UV);

            //fill in second triangle if it exist
            if (individualFaces[i].isFaceComplete)
            {
                verticesList.Add(individualFaces[i].tri2.V0.position);
                verticesList.Add(individualFaces[i].tri2.V1.position);
                verticesList.Add(individualFaces[i].tri2.V2.position);

                normalList.Add(individualFaces[i].tri2.V0.normal);
                normalList.Add(individualFaces[i].tri2.V1.normal);
                normalList.Add(individualFaces[i].tri2.V2.normal);

                UVList.Add(individualFaces[i].tri2.V0.UV);
                UVList.Add(individualFaces[i].tri2.V1.UV);
                UVList.Add(individualFaces[i].tri2.V2.UV);
            }

        }

        for (int i = 0; i < verticesList.Count; i++)
        {
            indicesList.Add(i);
        }


        mesh.Clear();
        mesh.vertices = verticesList.ToArray();
        mesh.triangles = indicesList.ToArray();
        mesh.normals = normalList.ToArray();
        mesh.uv = UVList.ToArray();

    }

}

[RequireComponent(typeof(MeshFilter))]
public class CuttableTreeScript : MonoBehaviour
{
    List<TreeSplitCollisionBox> collisionBoxes;

    [SerializeField] int divisionIndex = 5;

    List<Vector3> upperVertices = new List<Vector3>();
    List<Vector3> bottomVertices = new List<Vector3>();
    List<Vector3> intersectionVertices = new List<Vector3>();

    [SerializeField] int debugShowIndex = 2;
    [SerializeField] float debugSphereSize = 1.0f;

    MeshFilter meshFilter;
    Mesh mesh;

    [SerializeField]GameObject DebugObjectTest;


    private void Start()
    {
        collisionBoxes = new List<TreeSplitCollisionBox>();

        meshFilter = gameObject.GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;

        bruteForceCollisionBoxInitialize();

        upperVertices.Clear();
        bottomVertices.Clear();
        intersectionVertices.Clear();
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {

            upperVertices.Clear();
            bottomVertices.Clear();
            intersectionVertices.Clear();

            Debug.Log("Cut");
            CutAt(DebugObjectTest.transform.position, DebugObjectTest.transform.up);
        }
    }

    public Mesh GetMesh()
    {
        return mesh;
    }

    public void CutAt(Vector3 position, Vector3 normal)
    {
        Matrix4x4 worldMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

        //will be usefull later for a possible optimization
        //Matrix4x4 inverseWorldMatrix = Matrix4x4.Inverse(worldMatrix);

        //Vector3 transformedPosition = inverseWorldMatrix.MultiplyPoint3x4(position);
        //Vector3 transformedNormal = (Matrix4x4.Transpose(worldMatrix)).MultiplyVector(normal);

        PrimitiveMesh FacesSplitAbove = new PrimitiveMesh();
        PrimitiveMesh FacesSplitBelow = new PrimitiveMesh();
        FacesSplitAbove.individualFaces = new List<IndividualFace>();
        FacesSplitBelow.individualFaces = new List<IndividualFace>();
        
        foreach (TreeSplitCollisionBox collisionBox in collisionBoxes)
        {
            foreach(Face face in collisionBox.faces)
            {
                TriangleSplitState tri1CheckResult = TriangleSplitState.DefaultNoTriangle;
                TriangleSplitState  tri2CheckResult = TriangleSplitState.DefaultNoTriangle;

                bool hasTriangle1 = face.tri1.v0 != -1;
                
                if (hasTriangle1)
                {
                    Vector3 worldV0 = worldMatrix.MultiplyPoint(mesh.vertices[face.tri1.v0]);
                    Vector3 worldV1 = worldMatrix.MultiplyPoint(mesh.vertices[face.tri1.v1]);
                    Vector3 worldV2 = worldMatrix.MultiplyPoint(mesh.vertices[face.tri1.v2]);

                    tri1CheckResult = TriangleToPlaneCheck(worldV0, worldV1, worldV2, position, normal);

                }

                bool hasTriangle2 = face.tri2.v0 != -1;

                if (hasTriangle2)
                {
                    Vector3 worldV0 = worldMatrix.MultiplyPoint(mesh.vertices[face.tri2.v0]);
                    Vector3 worldV1 = worldMatrix.MultiplyPoint(mesh.vertices[face.tri2.v1]);
                    Vector3 worldV2 = worldMatrix.MultiplyPoint(mesh.vertices[face.tri2.v2]);

                    tri2CheckResult = TriangleToPlaneCheck(worldV0, worldV1, worldV2, position, normal);
                }

                bool isBothTrianglesExist = hasTriangle1 && hasTriangle2;

                //if both triangles exist and have the same triangle split state
                if(isBothTrianglesExist && tri1CheckResult == tri2CheckResult)
                {
                    FindDecisionForFace(worldMatrix, tri1CheckResult, face, position, normal,FacesSplitAbove,FacesSplitBelow);
                }
                //if both triangles exist but do not have the same triangle split state
                else if(isBothTrianglesExist && tri1CheckResult != tri2CheckResult)
                {
                    

                }
                //one of the triangles in the face do not exist
                else if (!isBothTrianglesExist)
                {
                    if(hasTriangle1)
                    {
                        FindDecisionForSingularTriangle(worldMatrix, tri1CheckResult, face.tri1, position, normal, FacesSplitBelow,FacesSplitAbove);
                    }
                    if(hasTriangle2)
                    {
                        FindDecisionForSingularTriangle(worldMatrix, tri2CheckResult, face.tri2, position, normal,FacesSplitBelow, FacesSplitAbove);
                    }
                }
                



            }
        }

        FacesSplitBelow.PopulateMesh(mesh);

        GameObject newTree = new GameObject();
        var meshRenderer = newTree.AddComponent<MeshRenderer>();
        meshRenderer.material = GetComponent<MeshRenderer>().sharedMaterial;


        var newMeshFilter = newTree.AddComponent<MeshFilter>();

        newMeshFilter.mesh = new Mesh();

        newTree.transform.position = transform.position;
        newTree.transform.rotation = transform.rotation;
        newTree.transform.localScale = transform.localScale;

        FacesSplitAbove.PopulateMesh(newMeshFilter.mesh);




    }



    private TriangleSplitState TriangleToPlaneCheck(Vector3 transformedV0, Vector3 transformedV1,Vector3 transformedV2,Vector3 position,Vector3 normal)
    {
        bool v0AbovePlane = IsPointAbovePlane(transformedV0, position, normal);
        bool v1AbovePlane = IsPointAbovePlane(transformedV1, position, normal);
        bool v2AbovePlane = IsPointAbovePlane(transformedV2, position, normal);

        if (v0AbovePlane && v1AbovePlane && v2AbovePlane)
        {
            return TriangleSplitState.AbovePlane;
        }
        else if(!v0AbovePlane && !v1AbovePlane && !v2AbovePlane)
        {
            return TriangleSplitState.BelowPlane;
        }
        else
        {
            return TriangleSplitState.IntersectionOnPlane;
        }
        
    }
    //at this point, we know that both triangles have the same TriangleSplitState
    private void FindDecisionForFace(Matrix4x4 world,TriangleSplitState state, Face face, Vector3 position, 
        Vector3 normal,PrimitiveMesh upperPrimitiveMesh,PrimitiveMesh lowerPrimitiveMesh)
    {
        //face.tri1.InsureVertexOrdering(1, 2,mesh);
        //face.tri2.InsureVertexOrdering(0, 2, mesh);

        Vector3 worldV0 = world.MultiplyPoint(mesh.vertices[face.tri1.v0]);
        Vector3 worldV1 = world.MultiplyPoint(mesh.vertices[face.tri1.v1]);
        Vector3 worldV2 = world.MultiplyPoint(mesh.vertices[face.tri1.v2]);

        Vector3 worldV3 = world.MultiplyPoint(mesh.vertices[face.tri2.v0]);
        Vector3 worldV4 = world.MultiplyPoint(mesh.vertices[face.tri2.v1]);
        Vector3 worldV5 = world.MultiplyPoint(mesh.vertices[face.tri2.v2]);



        switch (state)
        {
            case TriangleSplitState.AbovePlane:

                upperVertices.Add(worldV0);
                upperVertices.Add(worldV1);
                upperVertices.Add(worldV2);

                upperVertices.Add(worldV3);
                upperVertices.Add(worldV4);
                upperVertices.Add(worldV5);

                upperPrimitiveMesh.AddFaceFrom(this, face);

                break;

            case TriangleSplitState.BelowPlane:

                bottomVertices.Add(worldV0);
                bottomVertices.Add(worldV1);
                bottomVertices.Add(worldV2);

                bottomVertices.Add(worldV3);
                bottomVertices.Add(worldV4);
                bottomVertices.Add(worldV5);

                lowerPrimitiveMesh.AddFaceFrom(this, face);

                break;

            case TriangleSplitState.IntersectionOnPlane:

                Vector3[] worldTrianglePointPositions = new Vector3[6];
                worldTrianglePointPositions[0] = worldV0;
                worldTrianglePointPositions[1] = worldV1;
                worldTrianglePointPositions[2] = worldV2;
                worldTrianglePointPositions[3] = worldV3;
                worldTrianglePointPositions[4] = worldV4;
                worldTrianglePointPositions[5] = worldV5;

                
                IntersectingFaceSplit(world, face, position, normal, worldTrianglePointPositions, lowerPrimitiveMesh, upperPrimitiveMesh);


                //List<Vector3> foundIntersectionPoint;
                //UnOptimizedGetFaceToPlaneIntersectionPoints(world, face, position, normal, out foundIntersectionPoint);

                //int[] faceIndices = face.ToIntArray();
                //bool[] triangleState = new bool[6];

                //triangleState[0] = IsPointAbovePlane(worldV0, position, normal);
                //triangleState[1] = IsPointAbovePlane(worldV1, position, normal);
                //triangleState[2] = IsPointAbovePlane(worldV2, position, normal);
                //triangleState[3] = IsPointAbovePlane(worldV3, position, normal);
                //triangleState[4] = IsPointAbovePlane(worldV4, position, normal);
                //triangleState[5] = IsPointAbovePlane(worldV5, position, normal);

                //List<int> trianglesAboveSplittingPlane = new List<int>();
                //List<int> trianglesBelowSplittingPlane = new List<int>();

                ////for each triangleState
                //for (int i = 0; i < triangleState.Length; i++)
                //{
                //    if (triangleState[i])
                //    {
                //        trianglesAboveSplittingPlane.Add(faceIndices[i]);
                //    }
                //    else
                //    {
                //        trianglesBelowSplittingPlane.Add(faceIndices[i]);
                //    }

                //}

                //List<int> uniqueTrianglesAboveSplittingPlane = GetUniqueVertices(trianglesAboveSplittingPlane);
                //List<int> uniqueTrianglesBelowSplittingPlane = GetUniqueVertices(trianglesBelowSplittingPlane);
                //List<Vector3> uniqueIntersectionPoints = GetUniqueVector3Collection(foundIntersectionPoint);

                //if (uniqueTrianglesBelowSplittingPlane.Count != 2) { break; }
                ////create bottom faces
                //Vector3 basePosition = mesh.vertices[uniqueTrianglesBelowSplittingPlane[0]];
                //Vector3 baseDirection = mesh.vertices[uniqueTrianglesBelowSplittingPlane[1]] - basePosition;

                //Debug.Log("Found Position and Ordering " + basePosition + "," + baseDirection);

                //IntersectionComparer ic = new IntersectionComparer
                //    (baseDirection, basePosition);

                //uniqueIntersectionPoints.Sort(ic);

                //assembleFacesFromSplitVertices(uniqueIntersectionPoints, uniqueTrianglesBelowSplittingPlane, false, world, lowerPrimitiveMesh);

                //uniqueTrianglesAboveSplittingPlane.Reverse();
                //assembleFacesFromSplitVertices(uniqueIntersectionPoints, uniqueTrianglesAboveSplittingPlane, true, world, upperPrimitiveMesh);




                break;



        }
    }


    private void IntersectingFaceSplit(Matrix4x4 world,Face face,Vector3 position,Vector3 normal,Vector3[] worldTrianglePointPositions,PrimitiveMesh lowerPrimitiveMesh,PrimitiveMesh upperPrimitiveMesh )
    {
        List<Vector3> foundIntersectionPoint;
        UnOptimizedGetFaceToPlaneIntersectionPoints(world, face, position, normal, out foundIntersectionPoint);



        int[] faceIndices = face.ToIntArray();
        bool[] triangleState = new bool[6];

        triangleState[0] = IsPointAbovePlane(worldTrianglePointPositions[0], position, normal);
        triangleState[1] = IsPointAbovePlane(worldTrianglePointPositions[1], position, normal);
        triangleState[2] = IsPointAbovePlane(worldTrianglePointPositions[2], position, normal);
        triangleState[3] = IsPointAbovePlane(worldTrianglePointPositions[3], position, normal);
        triangleState[4] = IsPointAbovePlane(worldTrianglePointPositions[4], position, normal);
        triangleState[5] = IsPointAbovePlane(worldTrianglePointPositions[5], position, normal);

        List<int> trianglesAboveSplittingPlane = new List<int>();
        List<int> trianglesBelowSplittingPlane = new List<int>();

        //for each triangleState
        for (int i = 0; i < triangleState.Length; i++)
        {
            if (triangleState[i])
            {
                trianglesAboveSplittingPlane.Add(faceIndices[i]);
            }
            else
            {
                trianglesBelowSplittingPlane.Add(faceIndices[i]);
            }

        }






        List<int> uniqueTrianglesAboveSplittingPlane = GetUniqueVertices(trianglesAboveSplittingPlane);
        List<int> uniqueTrianglesBelowSplittingPlane = GetUniqueVertices(trianglesBelowSplittingPlane);
        List<Vector3> uniqueIntersectionPoints = GetUniqueVector3Collection(foundIntersectionPoint);

        //create bottom faces
        Vector3 basePosition;
        Vector3 baseDirection;

        Debug.Log("uniqueTrianglesAboveSplittingPlane " + uniqueTrianglesAboveSplittingPlane.Count);
        Debug.Log("uniqueTrianglesBelowSplittingPlane " + uniqueTrianglesBelowSplittingPlane.Count);
        Debug.Log("uniqueIntersectionPoints" + uniqueIntersectionPoints.Count);
        Debug.Log("---------------------------------------");
        bool foundOrdering = GetOrderingPositionAndDirection(out basePosition, out baseDirection,  mesh,  uniqueTrianglesBelowSplittingPlane, uniqueTrianglesAboveSplittingPlane);

        Vector3 min, max;
        GetMinMaxOfVertices(out min, out max, worldTrianglePointPositions);
        baseDirection = max - min;


       

        //////////////////////////////////////////

        if (!foundOrdering) { return; }
        Debug.Log("Continuing face rearangge");

        IndexDirectionComparer idc = new IndexDirectionComparer(baseDirection, basePosition, mesh);

        IntersectionComparer ic = new IntersectionComparer
            (baseDirection, basePosition);

        uniqueIntersectionPoints.Sort(ic);
        uniqueTrianglesBelowSplittingPlane.Sort(idc);
        uniqueTrianglesAboveSplittingPlane.Sort(idc);

        //Debug.Log("Found Position and Ordering " + basePosition + "," + baseDirection);
        Debug.Log("uniqueTrianglesAboveSplittingPlane ");
        DEBUG_logIndicesList(mesh, world, uniqueTrianglesAboveSplittingPlane);
        Debug.Log("uniqueTrianglesBelowSplittingPlane ");
        DEBUG_logIndicesList(mesh, world, uniqueTrianglesBelowSplittingPlane);
        Debug.Log("uniqueIntersectionPoints ");
        DEBUG_logVertices(uniqueIntersectionPoints);


        List<Vector3> intersectionPoints = new List<Vector3>();
        intersectionPoints.Add(uniqueIntersectionPoints[0]);
        intersectionPoints.Add(uniqueIntersectionPoints[uniqueIntersectionPoints.Count-1]);

        intersectionVertices.Add(uniqueIntersectionPoints[0]);
        intersectionVertices.Add(uniqueIntersectionPoints[uniqueIntersectionPoints.Count - 1]);



        assembleFacesFromSplitVertices(intersectionPoints, uniqueTrianglesBelowSplittingPlane, false, world, lowerPrimitiveMesh);

        assembleFacesFromSplitVertices(intersectionPoints, uniqueTrianglesAboveSplittingPlane, true, world, upperPrimitiveMesh);
    }


    private bool GetOrderingPositionAndDirection(out Vector3 position,out Vector3 direction,Mesh mesh,List<int> uniqueTrianglesBelowSplittingPlane,List<int> uniqueTrianglesAboveSplittingPlane)
    {

        int nextI = 1;

        if( uniqueTrianglesAboveSplittingPlane.Count > nextI)
        {
            position = mesh.vertices[uniqueTrianglesAboveSplittingPlane[nextI]];
            direction = mesh.vertices[uniqueTrianglesAboveSplittingPlane[0]] - position;

            return true;
        }
        else if( uniqueTrianglesBelowSplittingPlane.Count > nextI)
        {
            position = mesh.vertices[uniqueTrianglesBelowSplittingPlane[0]];
            direction = mesh.vertices[uniqueTrianglesBelowSplittingPlane[nextI]] - position;

            return true;
        }
        else
        {
            position = Vector3.zero;
            direction = Vector3.zero;
            return false;
        }

    }

    private void assembleFacesFromSplitVertices(List<Vector3> uniqueIntersectionPoints,List<int> trianglesInSplitPlane,bool isIntersectionPointBottomLeftVertex, Matrix4x4 world,PrimitiveMesh meshToPopulate)
    {
        Matrix4x4 inverseWorld = Matrix4x4.Inverse(world);

        int iterationCount = uniqueIntersectionPoints.Count > trianglesInSplitPlane.Count ? uniqueIntersectionPoints.Count : trianglesInSplitPlane.Count;
       

        bool isTriangleFlipped = false;

        IndividualVertex flippedVertexStore;
        flippedVertexStore.position = Vector3.zero;
        flippedVertexStore.normal = Vector3.zero;
        flippedVertexStore.UV = new Vector2(1, 1);

        for (int i = 0; i < iterationCount-1; i++)
        {
            int currentItersectionPointI = GetCurrentIndex(uniqueIntersectionPoints.Count, i);
            Vector3 objectSpaceItersectionPoint = inverseWorld.MultiplyPoint(uniqueIntersectionPoints.ElementAt(currentItersectionPointI));

            bool nextIntersectionPointExist = i + 1 < uniqueIntersectionPoints.Count;
            bool nextTrianglePointExist = i + 1 < trianglesInSplitPlane.Count;

            IndividualVertex bottomLeftVertex;
            IndividualVertex upperLeftVertex;
            //IndividualTriangle tri1 = new IndividualTriangle();

            if (nextTrianglePointExist && nextIntersectionPointExist)
            {
                IndividualVertex bottomRightVertex;
                IndividualVertex upperRightVertex;
                //IndividualTriangle tri2 = new IndividualTriangle();
 
                Vector3 nextObjectSpaceIntersectionPoint = inverseWorld.MultiplyPoint(uniqueIntersectionPoints.ElementAt(i + 1));

                //all intersection points are bottom, triangles points are upper
                if (isIntersectionPointBottomLeftVertex)
                {
                    upperLeftVertex.position = mesh.vertices[trianglesInSplitPlane.ElementAt(i)];
                    upperLeftVertex.normal = mesh.normals[trianglesInSplitPlane.ElementAt(i)];
                    upperLeftVertex.UV = mesh.uv[trianglesInSplitPlane.ElementAt(i)];

                   
                    upperRightVertex.position = mesh.vertices[trianglesInSplitPlane.ElementAt(i+1)];
                    upperRightVertex.normal = mesh.normals[trianglesInSplitPlane.ElementAt(i+1)];
                    upperRightVertex.UV = mesh.uv[trianglesInSplitPlane.ElementAt(i+1)];

                    bottomLeftVertex.position = objectSpaceItersectionPoint;
                    bottomLeftVertex.normal = upperLeftVertex.normal;
                    bottomLeftVertex.UV = new Vector2(1, 1);

                    bottomRightVertex.position = nextObjectSpaceIntersectionPoint;
                    bottomRightVertex.normal = upperRightVertex.normal;
                    bottomRightVertex.UV = new Vector2(1, 1);

                    IndividualTriangle tri1 = new IndividualTriangle(bottomLeftVertex, upperLeftVertex, upperRightVertex);
                    IndividualTriangle tri2 = new IndividualTriangle(bottomLeftVertex, upperRightVertex,bottomRightVertex);


                    if (tri1.AttemptNormalBasedVertexCorrection(bottomLeftVertex.normal, 1, 2))
                    {
                        isTriangleFlipped = true;
                        tri2.V1 = upperLeftVertex;
                        flippedVertexStore = upperLeftVertex;
                    }

                    if (tri2.AttemptNormalBasedVertexCorrection(bottomLeftVertex.normal, 0, 2))
                    {
                        isTriangleFlipped = true;
                        tri2.V1 = upperRightVertex;
                    }

                    //if(tri2.AttemptNormalBasedVertexCorrection())

                    meshToPopulate.AddFaceFromTriangles(tri1, tri2);

                    Debug.Log(" upperLeftVertex.position " + world.MultiplyPoint(upperLeftVertex.position).ToString("F2"));
                    Debug.Log(" upperRightVertex.position  " + world.MultiplyPoint(upperRightVertex.position).ToString("F2"));
                    Debug.Log(" bottomLeftVertex.position " + world.MultiplyPoint(bottomLeftVertex.position).ToString("F2"));
                    Debug.Log(" bottomRightVertex.position " + world.MultiplyPoint(bottomRightVertex.position).ToString("F2"));

                }
                //all intersection points are upper, triangles points are bottom
                else
                {

                    bottomLeftVertex.position = mesh.vertices[trianglesInSplitPlane.ElementAt(i)];
                    bottomLeftVertex.normal = mesh.normals[trianglesInSplitPlane.ElementAt(i)];
                    bottomLeftVertex.UV = mesh.uv[trianglesInSplitPlane.ElementAt(i)];

                    bottomRightVertex.position = mesh.vertices[trianglesInSplitPlane.ElementAt(i+1)]; ;
                    bottomRightVertex.normal = mesh.normals[trianglesInSplitPlane.ElementAt(i+1)]; ;
                    bottomRightVertex.UV = mesh.uv[trianglesInSplitPlane.ElementAt(i+1)]; ;

                    upperLeftVertex.position = objectSpaceItersectionPoint;
                    upperLeftVertex.normal = bottomLeftVertex.normal;
                    upperLeftVertex.UV = new Vector2(1, 1);

                    upperRightVertex.position = nextObjectSpaceIntersectionPoint;
                    upperRightVertex.normal = bottomRightVertex.normal;
                    upperRightVertex.UV = new Vector2(1, 1);

                    IndividualTriangle tri1 = new IndividualTriangle(bottomLeftVertex, upperLeftVertex, upperRightVertex);
                    IndividualTriangle tri2 = new IndividualTriangle(bottomLeftVertex, upperRightVertex, bottomRightVertex);


                    if (tri1.AttemptNormalBasedVertexCorrection(bottomLeftVertex.normal, 1, 2))
                    {
                        isTriangleFlipped = true;
                        tri2.V1 = upperLeftVertex;
                        flippedVertexStore = upperLeftVertex;
                    }

                    if (tri2.AttemptNormalBasedVertexCorrection(bottomLeftVertex.normal, 0, 2))
                    {
                        isTriangleFlipped = true;
                        tri2.V1 = upperRightVertex;
                    }



                    meshToPopulate.AddFaceFromTriangles(tri1, tri2);

                    //Debug.Log(" upperLeftVertex.position " + world.MultiplyPoint(upperLeftVertex.position).ToString("F2"));
                    //Debug.Log(" upperRightVertex.position  " + world.MultiplyPoint(upperRightVertex.position).ToString("F2"));
                    //Debug.Log(" bottomLeftVertex.position " + world.MultiplyPoint(bottomLeftVertex.position).ToString("F2"));
                    //Debug.Log(" bottomRightVertex.position " + world.MultiplyPoint(bottomRightVertex.position).ToString("F2"));

                }


            }
            else
            {
                IndividualVertex bottomRightVertex;
                IndividualVertex upperRightVertex;

                int currentTriangleIndex = GetCurrentIndex(trianglesInSplitPlane.Count, i);

                if (nextIntersectionPointExist)
                {
                    Vector3 nextObjectSpaceIntersectionPoint = inverseWorld.MultiplyPoint(uniqueIntersectionPoints.ElementAt(i + 1));

                    

                    //all intersection points are bottom, triangles points are upper
                    if (isIntersectionPointBottomLeftVertex)
                    {

                        upperLeftVertex.position = mesh.vertices[trianglesInSplitPlane.ElementAt(currentTriangleIndex)];
                        upperLeftVertex.normal = mesh.normals[trianglesInSplitPlane.ElementAt(currentTriangleIndex)];
                        upperLeftVertex.UV = mesh.uv[trianglesInSplitPlane.ElementAt(currentTriangleIndex)];

                        bottomLeftVertex.position = objectSpaceItersectionPoint;
                        bottomLeftVertex.normal = upperLeftVertex.normal;
                        bottomLeftVertex.UV = new Vector2(1, 1);

                        bottomRightVertex.position = nextObjectSpaceIntersectionPoint;
                        bottomRightVertex.normal = upperLeftVertex.normal;
                        bottomRightVertex.UV = new Vector2(1, 1);

                        IndividualTriangle tri1 = new IndividualTriangle(upperLeftVertex, bottomRightVertex, bottomLeftVertex);

                        if (isTriangleFlipped)
                        {
                            isTriangleFlipped = false;
                            tri1.V0 = flippedVertexStore;

                        }

                        tri1.AttemptNormalBasedVertexCorrection(upperLeftVertex.normal, 1, 2);

                        meshToPopulate.AddFaceFromSingularTriangle(tri1);

                    }
                    //all intersection points are upper, triangles points are bottom
                    else
                    {
                        bottomLeftVertex.position = mesh.vertices[trianglesInSplitPlane.ElementAt(currentTriangleIndex)];
                        bottomLeftVertex.normal = mesh.normals[trianglesInSplitPlane.ElementAt(currentTriangleIndex)];
                        bottomLeftVertex.UV = mesh.uv[trianglesInSplitPlane.ElementAt(currentTriangleIndex)];

                        upperLeftVertex.position = objectSpaceItersectionPoint;
                        upperLeftVertex.normal = bottomLeftVertex.normal;
                        upperLeftVertex.UV = new Vector2(1, 1);

                        upperRightVertex.position = nextObjectSpaceIntersectionPoint;
                        upperRightVertex.normal = upperLeftVertex.normal;
                        upperRightVertex.UV = new Vector2(1, 1);

                        IndividualTriangle tri1 = new IndividualTriangle(bottomLeftVertex, upperLeftVertex, upperRightVertex);
                        meshToPopulate.AddFaceFromSingularTriangle(tri1);

                        if (isTriangleFlipped)
                        {
                            isTriangleFlipped = false;
                            tri1.V1 = flippedVertexStore;
                        }

                        tri1.AttemptNormalBasedVertexCorrection(upperLeftVertex.normal, 1, 2);

                        meshToPopulate.AddFaceFromSingularTriangle(tri1);
                    }
                }
                //
                if (nextTrianglePointExist)
                {
                    //all intersection points are bottom, triangles points are upper
                    if (isIntersectionPointBottomLeftVertex)
                    {
                        bottomLeftVertex.position = mesh.vertices[trianglesInSplitPlane.ElementAt(currentTriangleIndex)];
                        bottomLeftVertex.normal = mesh.normals[trianglesInSplitPlane.ElementAt(currentTriangleIndex)];
                        bottomLeftVertex.UV = mesh.uv[trianglesInSplitPlane.ElementAt(currentTriangleIndex)];

                        upperRightVertex.position = objectSpaceItersectionPoint;
                        upperRightVertex.normal = bottomLeftVertex.normal;
                        upperRightVertex.UV = new Vector2(1, 1);

                        bottomLeftVertex.position = mesh.vertices[trianglesInSplitPlane.ElementAt(currentTriangleIndex + 1)];
                        bottomLeftVertex.normal = mesh.normals[trianglesInSplitPlane.ElementAt(currentTriangleIndex + 1)];
                        bottomLeftVertex.UV = mesh.uv[trianglesInSplitPlane.ElementAt(currentTriangleIndex + 1)];


                    }
                    else
                    {

                    }
                }




            }

          
        }
    }

    

    private void FindDecisionForSingularTriangle(Matrix4x4 world, TriangleSplitState state, Triangle tri, Vector3 position, Vector3 normal,PrimitiveMesh lowerMesh,PrimitiveMesh upperMesh)
    {
        Vector3 worldV0 = world.MultiplyPoint(mesh.vertices[tri.v0]);
        Vector3 worldV1 = world.MultiplyPoint(mesh.vertices[tri.v1]);
        Vector3 worldV2 = world.MultiplyPoint(mesh.vertices[tri.v2]);

        switch(state)
        {
            case TriangleSplitState.AbovePlane:

                upperVertices.Add(worldV0);
                upperVertices.Add(worldV1);
                upperVertices.Add(worldV2);

                upperMesh.AddTriangleFrom(this, tri);

                break;

            case TriangleSplitState.BelowPlane:

                bottomVertices.Add(worldV0);
                bottomVertices.Add(worldV1);
                bottomVertices.Add(worldV2);

                lowerMesh.AddTriangleFrom(this, tri);

                break;

            case TriangleSplitState.IntersectionOnPlane:


                List<Vector3> triangleIntersectionPoints = UnOptimizedFindTriangleToPlaneIntersectionPoint(worldV0, worldV1, worldV2, position, normal);

                foreach (var intersectionPoint in triangleIntersectionPoints)
                {
                    intersectionVertices.Add(intersectionPoint);
                }

                break;
        }
    }

    //assumes that face contains 2 triangles
    private void UnOptimizedGetFaceToPlaneIntersectionPoints(Matrix4x4 world, Face face, Vector3 position,Vector3 normal, out List<Vector3> intersectionPoints)
    {
        intersectionPoints = new List<Vector3>();

        Vector3 worldV0 = world.MultiplyPoint(mesh.vertices[face.tri1.v0]);
        Vector3 worldV1 = world.MultiplyPoint(mesh.vertices[face.tri1.v1]);
        Vector3 worldV2 = world.MultiplyPoint(mesh.vertices[face.tri1.v2]);

        Vector3 worldV3 = world.MultiplyPoint(mesh.vertices[face.tri2.v0]);
        Vector3 worldV4 = world.MultiplyPoint(mesh.vertices[face.tri2.v1]);
        Vector3 worldV5 = world.MultiplyPoint(mesh.vertices[face.tri2.v2]);

        List<Vector3> triangleIntersectionPoints = UnOptimizedFindTriangleToPlaneIntersectionPoint
            (worldV0, worldV1, worldV2, position, normal);

        List<Vector3> secondTriangleIntersectionPoints = UnOptimizedFindTriangleToPlaneIntersectionPoint
            (worldV3, worldV4, worldV5, position, normal);

   
        foreach(var intersectionPoint in triangleIntersectionPoints)
        {
            
            intersectionPoints.Add(intersectionPoint);
        }

        foreach (var intersectionPoint in secondTriangleIntersectionPoints)
        {
            intersectionPoints.Add(intersectionPoint);
            
        }

    }

    private List<Vector3> UnOptimizedFindTriangleToPlaneIntersectionPoint(Vector3 transformedV0, Vector3 transformedV1, Vector3 transformedV2, Vector3 position, Vector3 normal)
    {
        List<Vector3> result = new List<Vector3>();

        Vector3 intersection1;
        if(UnOptimizedFindLineToPlaneIntersection(transformedV0,transformedV1,position,normal,out intersection1) || 
            UnOptimizedFindLineToPlaneIntersection(transformedV1, transformedV0, position, normal, out intersection1)
            )
        {
            result.Add(intersection1);
        }

        Vector3 intersection2;
        if (UnOptimizedFindLineToPlaneIntersection(transformedV1, transformedV2, position, normal, out intersection2) ||
            UnOptimizedFindLineToPlaneIntersection(transformedV2, transformedV1, position, normal, out intersection2)
            )
        {
            result.Add(intersection2);
        }

        Vector3 intersection3;
        if (UnOptimizedFindLineToPlaneIntersection(transformedV2, transformedV0, position, normal, out intersection3) ||
             UnOptimizedFindLineToPlaneIntersection(transformedV0, transformedV2, position, normal, out intersection3)
            )
        {
            result.Add(intersection3);
        }

        return result;
    }

    private bool UnOptimizedFindLineToPlaneIntersection(Vector3 transformedV0, Vector3 transformedV1, Vector3 position, Vector3 normal,out Vector3 intersection)
    {
        Vector3 lineToUse = transformedV1 - transformedV0;

        Vector3 P0 = transformedV0;
        Vector3 P1 = lineToUse.normalized;
        Vector3 A = position;

        float t = (Vector3.Dot(A, normal) - Vector3.Dot(P0, normal)) / Vector3.Dot(P1, normal);

        intersection = P0 + P1 * t;

        return t > 0.0f && t < lineToUse.magnitude;

    }

    private void bruteForceCollisionBoxInitialize()
    {
        TreeSplitCollisionBox tscb = new TreeSplitCollisionBox();
        tscb.faces = new List<Face>();
        collisionBoxes.Add(tscb);

        Debug.Log("indice count " + mesh.triangles.Length);

        for (int i = 0; i < mesh.triangles.Length; i+=6)
        {

            if (i + 5 >= mesh.triangles.Length) { break; }

            int v0 = mesh.triangles[i];
            int v1 = mesh.triangles[i + 1];
            int v2 = mesh.triangles[i + 2];

            

            int v3 = mesh.triangles[i + 3];
            int v4 = mesh.triangles[i + 4];
            int v5 = mesh.triangles[i + 5];

            Debug.Assert(i + 5 < mesh.triangles.Length);

            Triangle tri1 = new Triangle();
            tri1.v0 = v0;
            tri1.v1 = v1;
            tri1.v2 = v2;

            Triangle tri2 = new Triangle();
            tri2.v0 = v3;
            tri2.v1 = v4;
            tri2.v2 = v5;



            Vector3 tri1Normal = mesh.normals[v0];
            Vector3 tri2Normal = mesh.normals[v3];
            
            if ((tri1Normal - tri2Normal).magnitude < 0.001f)
            {
                Face face = new Face();
                face.Init();

                face.tri1 = tri1;
                face.tri2 = tri2;

                tscb.faces.Add(face);
            }
            else
            {
                Face face1 = new Face();
                face1.Init();
                Face face2 = new Face();
                face2.Init();

                face1.tri1 = tri1;
                face2.tri2 = tri2;

                tscb.faces.Add(face1);
                tscb.faces.Add(face2);
            }


        }

        Debug.Log("mesh face count " + tscb.faces.Count);
    }

    private void initializeCollisionBoxes()
    {
        
        //get vertex count
        int indicesCount = mesh.triangles.Length;
        

        int maxCollisionBoxIndex = indicesCount / divisionIndex;

        //maxCollisionBoxIndex
        maxCollisionBoxIndex = maxCollisionBoxIndex - maxCollisionBoxIndex % 3;
        

        for(int i = 0; i < divisionIndex; i++)
        {
            TreeSplitCollisionBox tscb = new TreeSplitCollisionBox();
            tscb.faces = new List<Face>();
            collisionBoxes.Add(tscb);

            for(int j = 0; j < maxCollisionBoxIndex;j+=6)
            {
                //get triangle indices
                int v0 = mesh.triangles[i * maxCollisionBoxIndex + j];
                int v1 = mesh.triangles[i * maxCollisionBoxIndex + j + 1];
                int v2 = mesh.triangles[i * maxCollisionBoxIndex + j + 2];
                int v3 = mesh.triangles[i * maxCollisionBoxIndex + j + 3];
                int v4 = mesh.triangles[i * maxCollisionBoxIndex + j + 4];
                int v5 = mesh.triangles[i * maxCollisionBoxIndex + j + 5];

                Triangle tri1 = new Triangle();
                tri1.v0 = v0;
                tri1.v1 = v1;
                tri1.v2 = v2;

                Triangle tri2 = new Triangle();
                tri2.v0 = v3;
                tri2.v1 = v4;
                tri2.v2 = v5;

                Vector3 tri1Normal = mesh.normals[v0];
                Vector3 tri2Normal = mesh.normals[v3];

                if(tri1Normal.Equals(tri2Normal))
                {
                    Face face = new Face();
                    face.Init();

                    face.tri1 = tri1;
                    face.tri2 = tri2;

                    tscb.faces.Add(face);
                }
                else
                {
                    Face face1 = new Face();
                    face1.Init();
                    Face face2 = new Face();
                    face2.Init();

                    face1.tri1 = tri1;
                    face2.tri2 = tri2;

                    tscb.faces.Add(face1);
                    tscb.faces.Add(face2);
                }

            }



        }


    }





    //---------------------------------- Helper Functions----------------------------------------//

    private int GetCurrentIndex(int maxIndex, int requestedIndex)
    {
        return requestedIndex >= maxIndex - 1 ? maxIndex - 1 : requestedIndex; 
    }

    private bool IsPointAbovePlane(Vector3 pointPosition,Vector3 planePosition,Vector3 normal)
    {
        return Vector3.Dot(pointPosition - planePosition, normal) > 0;
    }

    private bool IsPointAbovePlane(Vector3 pointPosition, Vector3 planePosition, Vector3 normal,out PointSplitState state)
    {
        bool isPointAbovePlane = IsPointAbovePlane(pointPosition, planePosition, normal);

        if(isPointAbovePlane)
        {
            state = PointSplitState.AbovePlane;
        }
        else
        {
            state = PointSplitState.BelowPlane;
        }

        return isPointAbovePlane;

    }

    private List<int> GetUniqueVertices(List<int> nonUniqueIndiceList)
    {
        List<int> uniqueVertices = new List<int>();
        List<Vector3> seenVertices = new List<Vector3>();


        for (int i = 0; i < nonUniqueIndiceList.Count; i++)
        {
            bool hasSeenVertex = false;
            Vector3 vertexToTest = mesh.vertices[nonUniqueIndiceList[i]];

            foreach (Vector3 vertex in seenVertices)
            {
                if (vertexToTest.Equals(vertex))
                {
                    hasSeenVertex = true;
                    break;
                }
            }

            if (!hasSeenVertex)
            {
                //Debug.Log("unique vertex found  ")
                seenVertices.Add(mesh.vertices[nonUniqueIndiceList[i]]);
                uniqueVertices.Add(nonUniqueIndiceList[i]);
            }



        }

        return uniqueVertices;
    }

    private List<Vector3> GetUniqueVector3Collection(List<Vector3> nonUniqueCollection)
    {
        List<Vector3> uniqueCollection = new List<Vector3>();

        for (int i = 0; i < nonUniqueCollection.Count; i++)
        {
            bool hasSeenElement = false;

            foreach (var vertex in uniqueCollection)
            {
                if ((nonUniqueCollection[i] - vertex).magnitude < 0.001f)
                {
                    hasSeenElement = true;
                    break;
                }
            }

            if (!hasSeenElement)
            {
                uniqueCollection.Add(nonUniqueCollection[i]);
            }

        }

        return uniqueCollection;
    }

    private void GetMinMaxOfVertices(out Vector3 min ,out Vector3 max,Vector3[] vectors)
    {
        Vector3 minResult = new Vector3(float.MaxValue, float.MaxValue, float.MaxValue);
        Vector3 maxResult = new Vector3(float.MinValue, float.MinValue, float.MinValue);

        foreach(Vector3 vertex in vectors)
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

    //private List<Vector3> 


    //------------------------ Debugging related functions --------------------------//

    private void OnDrawGizmos()
    {
        //initialization testing
        if(collisionBoxes != null)
        {
            DEBUG_drawMeshInitialize();
        }


        Gizmos.color = Color.red;

        foreach(var position in upperVertices)
        {
            Gizmos.DrawSphere(position, debugSphereSize);
        }

        Gizmos.color = Color.blue;

        foreach (var position in bottomVertices)
        {
            Gizmos.DrawSphere(position, debugSphereSize);
        }

        Gizmos.color = new Color(1,0,1);

        foreach (var position in intersectionVertices)
        {

            Gizmos.DrawSphere(position, debugSphereSize);
        }



    }

    private void DEBUG_drawMeshInitialize()
    {
        List<Color> colorList = new List<Color>();
        colorList.Add(Color.red);
        colorList.Add(Color.blue);
        colorList.Add(Color.green);
        colorList.Add(Color.yellow);
        colorList.Add(Color.gray);

        if(debugShowIndex > collisionBoxes.Count) { return; }

        for (int k = 0; k < debugShowIndex; k++)
        {
            Gizmos.color = colorList[k];

            for (int i = 0; i < collisionBoxes[k].faces.Count; i++)
            {
                DEBUG_drawCollisionBox(collisionBoxes[k]);
            }
        }
    }

    private void DEBUG_drawCollisionBox(TreeSplitCollisionBox box)
    {
        for(int i = 0; i < box.faces.Count; i++)
        {
            Face face = box.faces[i];

            if (face.tri1.v0 != -1)
            {
                DEBUG_drawTrianglePoints(face.tri1);

            }
            if (face.tri2.v1 != -1)
            {
                DEBUG_drawTrianglePoints(face.tri2);
            }
        }
    }

    private void DEBUG_drawTrianglePoints(Triangle tri)
    {
        Matrix4x4 worldM = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);

        Vector3 pos1 = worldM.MultiplyPoint3x4(mesh.vertices[tri.v0]);
        Vector3 pos2 = worldM.MultiplyPoint3x4(mesh.vertices[tri.v1]);
        Vector3 pos3 = worldM.MultiplyPoint3x4(mesh.vertices[tri.v2]);

        Gizmos.DrawSphere(pos1, debugSphereSize);
        Gizmos.DrawSphere(pos2, debugSphereSize);
        Gizmos.DrawSphere(pos3, debugSphereSize);
    }

    private void DEBUG_logIndicesList(Mesh mesh,Matrix4x4 world,List<int> indices)
    {
        for(int i =0; i < indices.Count;i++)
        {
            Debug.Log("vert " + i + "is " + world.MultiplyPoint(mesh.vertices[indices[i]]) );
        }

    }

    private void DEBUG_logVertices(List<Vector3> vectors)
    {
        for (int i = 0; i < vectors.Count; i++)
        {
            Debug.Log("vert " + i + "is " + vectors[i]);
        }
    }

}

//store effected Collision Boxes

//store un-effected Collision Boxes

//from un-effected, store collision boxes below splitting plane 

//from un-effected, store collision boxes above splitting plane 

//for each effected collision box
    //for each face
