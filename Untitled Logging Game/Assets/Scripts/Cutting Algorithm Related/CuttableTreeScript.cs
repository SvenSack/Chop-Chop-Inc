using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public struct ConnectionTypeToCentroid
{
    public TriangleConnectionType tct;
    public Vector3 objectSpaceCentroid;
}

public enum TriangleConnectionType
{
    DoubleIntersection,
    DoubleOriginalPoint,
    DefaultNoType

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


[RequireComponent(typeof(MeshFilter))]


public class CuttableTreeScript : MonoBehaviour
{


    private List<TreeSplitCollisionBox> collisionBoxes;

    [HideInInspector] public CuttableMeshPhysicsManager meshColliderGenerator;

    List<Vector3> upperVertices = new List<Vector3>();
    List<Vector3> bottomVertices = new List<Vector3>();
    List<Vector3> DEBUG_intersectionVertices = new List<Vector3>();

    [SerializeField] int debugShowIndex = 2;
    [SerializeField] float debugSphereSize = 1.0f;

    MeshFilter meshFilter;
    Mesh mesh;

    [HideInInspector] public GameObject DebugObjectTest;

    private List<MeshLidPairing> lidPairings = new List<MeshLidPairing>();

    private Vector3 preCutCentroid;
    private Vector3 objectSpacePreCutCentroid;

    public int leafParticleIndex;
    private bool isFirstTree = true;

<<<<<<<


=======

>>>>>>>
    private void Start()
    {
        Debug.Log("Start Called for CuttableTree in Object " + gameObject.name);

        collisionBoxes = new List<TreeSplitCollisionBox>();

        meshFilter = gameObject.GetComponent<MeshFilter>();
        mesh = meshFilter.mesh;

        bruteForceCollisionBoxInitialize();

        upperVertices.Clear();
        bottomVertices.Clear();
        DEBUG_intersectionVertices.Clear();


        preCutCentroid = new Vector3(0, 0, 0);

        foreach(Vector3 position in mesh.vertices)
        {
            preCutCentroid += position;
        }

        preCutCentroid /= (mesh.vertices.Length);

        objectSpacePreCutCentroid = preCutCentroid;

        meshColliderGenerator = GetMeshColliderGenerator();

        if (isFirstTree)
        {
            ensureCentroidLeaf();
        }
    }

    public void SetIsFirstTree(bool state)
    {
        isFirstTree = state;
    }

    private void ensureCentroidLeaf()
    {
        foreach(Transform child in transform)
        {
            if(child.tag == "leaf")
            {
                Utils.EnsurePositionIsCentroid(child);
            }
        }
    }

    public GameObject CutAt(Vector3 position, Vector3 normal, float seperationForce = 0.0f)
    {
        Matrix4x4 worldMatrix = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);
        preCutCentroid = worldMatrix.MultiplyPoint(preCutCentroid);

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
            foreach (Face face in collisionBox.faces)
            {
                TriangleSplitState tri1CheckResult = TriangleSplitState.DefaultNoTriangle;
                TriangleSplitState tri2CheckResult = TriangleSplitState.DefaultNoTriangle;

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
                if (isBothTrianglesExist && tri1CheckResult == tri2CheckResult)
                {
                    FindDecisionForFace(worldMatrix, tri1CheckResult, face, position, normal, FacesSplitAbove, FacesSplitBelow);
                }
                //if both triangles exist but do not have the same triangle split state
                else if (isBothTrianglesExist && tri1CheckResult != tri2CheckResult)
                {
                    
                    Vector3 worldV0 = worldMatrix.MultiplyPoint(mesh.vertices[face.tri1.v0]);
                    Vector3 worldV1 = worldMatrix.MultiplyPoint(mesh.vertices[face.tri1.v1]);
                    Vector3 worldV2 = worldMatrix.MultiplyPoint(mesh.vertices[face.tri1.v2]);

                    Vector3 worldV3 = worldMatrix.MultiplyPoint(mesh.vertices[face.tri2.v0]);
                    Vector3 worldV4 = worldMatrix.MultiplyPoint(mesh.vertices[face.tri2.v1]);
                    Vector3 worldV5 = worldMatrix.MultiplyPoint(mesh.vertices[face.tri2.v2]);

                    Vector3[] worldTrianglePointPositions = new Vector3[6];
                    worldTrianglePointPositions[0] = worldV0;
                    worldTrianglePointPositions[1] = worldV1;
                    worldTrianglePointPositions[2] = worldV2;
                    worldTrianglePointPositions[3] = worldV3;
                    worldTrianglePointPositions[4] = worldV4;
                    worldTrianglePointPositions[5] = worldV5;

                    IntersectingFaceSplit(worldMatrix, face, position, normal, worldTrianglePointPositions, FacesSplitBelow, FacesSplitAbove);

                }
                //one of the triangles in the face do not exist
                else if (!isBothTrianglesExist)
                {
                    if (hasTriangle1)
                    {
                        FindDecisionForSingularTriangle(worldMatrix, tri1CheckResult, face.tri1, position, normal, FacesSplitBelow, FacesSplitAbove);
                    }
                    if (hasTriangle2)
                    {
                        FindDecisionForSingularTriangle(worldMatrix, tri2CheckResult, face.tri2, position, normal, FacesSplitBelow, FacesSplitAbove);
                    }
                }




            }

        }

        //do vertex rearrangment
        List<Vector3> vertexPositions = new List<Vector3>();

        Vector3 centerPoint = Vector3.zero;

        //for each li
        foreach (MeshLidPairing lidPairing in lidPairings)
        {
            centerPoint += lidPairing.v0.position;
            centerPoint += lidPairing.v1.position;

            vertexPositions.Add(lidPairing.v0.position);
            vertexPositions.Add(lidPairing.v1.position);
        }

        centerPoint /= (lidPairings.Count * 2);

        IndividualVertex vertex = new IndividualVertex(centerPoint, Vector3.zero, Vector2.zero);

        List<IndividualTriangle> trianglesBelow = new List<IndividualTriangle>();

        //for each lid pairing 
        foreach (MeshLidPairing lidPairing in lidPairings)
        {
            IndividualTriangle tri = lidPairing.CreateTriangle(vertex);

            Vector3 ObjectSpaceCentroid = tri.GetObjectSpaceCentroid();
            Vector3 direction = Vector3.Cross(Vector3.up, ObjectSpaceCentroid - centerPoint).normalized;

            tri.AttemptDirectionOrderingBasedVertexCorrection(ObjectSpaceCentroid, direction);
            tri.SetNormals(normal);
            trianglesBelow.Add(tri);
            FacesSplitBelow.AddFaceFromSingularTriangle(tri);

            IndividualTriangle tri2 = tri.Clone();
            tri2.FlipTriangle(0, 2);
            tri2.SetNormals(-normal);
            FacesSplitAbove.AddFaceFromSingularTriangle(tri2);

        }

        FacesSplitBelow.PopulateMesh(mesh);

        if (meshColliderGenerator)
        {
            meshColliderGenerator.GenerateMeshColliderFromCut(mesh);
        }
        
        GameObject newTree;
            var meshPhysicsManager = InstantiateTreePiece(FacesSplitAbove, out newTree);

        var otherMeshPhysicsManager = InstantiateTreePiece(FacesSplitAbove);

        float highestPoint = float.MinValue;
        Vector3 point = new Vector3(highestPoint, highestPoint, highestPoint);

        //find highest intersectionPoint
        foreach (MeshLidPairing lidPairing in lidPairings)
        {
            Vector3 worldV0 = worldMatrix.MultiplyPoint(lidPairing.v0.position);
            Vector3 worldV2 = worldMatrix.MultiplyPoint(lidPairing.v1.position);

            if (worldV0.y > highestPoint)
            {
                highestPoint = worldV0.y;
                point = worldV0;
            }

            if (worldV2.y > highestPoint)
            {
                highestPoint = worldV2.y;
                point = worldV2;
            }
        }

        DisplaceLeaves(position, normal,gameObject, otherMeshPhysicsManager.gameObject);

        

        otherMeshPhysicsManager.AddForceAt(seperationForce, normal, point);
        return newTree;
    }

    public void DisplaceLeaves(Vector3 planePosition, Vector3 planeNormal, GameObject belowCuttingPlaneObj, GameObject aboveCuttingPlaneObj)
    {
        List<Transform> children = new List<Transform>();

        foreach (Transform child in transform)
        {
            children.Add(child);
        }

        gameObject.transform.DetachChildren();

        foreach (Transform child in children)
        {
            Debug.Log("child is " + child.name);
            if (child.tag != "leaf")
            {
                continue;
            }


            if (Utils.IsPointAbovePlane(child.transform.position,planePosition,planeNormal))
            {
                child.parent = aboveCuttingPlaneObj.transform;
            }
            else
            {
                child.parent = belowCuttingPlaneObj.transform;
            }


        }


        
    }




    private Vector3 WorldPositionToScreenSpace(Vector3 positionOfObject)
    {
        Camera cam = Camera.main;
        Matrix4x4 MVP = cam.projectionMatrix * cam.worldToCameraMatrix
            * transform.localToWorldMatrix;

        return MVP.MultiplyPoint(positionOfObject);
    }

    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.C))
        {
            upperVertices.Clear();
            bottomVertices.Clear();
            DEBUG_intersectionVertices.Clear();

            Debug.Log("Cut");
            CutAt(DebugObjectTest.transform.position, DebugObjectTest.transform.up,20.0f);
        }
    }

    private CuttableMeshPhysicsManager GetMeshColliderGenerator()
    {
        return GetComponent<CuttableMeshPhysicsManager>();
    }

    public Mesh GetMesh()
    {
        return mesh;
    }

    

    public CuttableMeshPhysicsManager InstantiateTreePiece(PrimitiveMesh primitiveMesh, out GameObject newTree)
    {
        GameObject newTree = new GameObject();
        

        var meshRenderer = newTree.AddComponent<MeshRenderer>();
        meshRenderer.material = GetComponent<MeshRenderer>().sharedMaterial;

        var newMeshFilter = newTree.AddComponent<MeshFilter>();

        CuttableTreeScript cuttableTree = newTree.AddComponent<CuttableTreeScript>();
        cuttableTree.isFirstTree = false;


        newTree.transform.position = transform.position;
        newTree.transform.rotation = transform.rotation;
        newTree.transform.localScale = transform.localScale;

        primitiveMesh.PopulateMesh(newMeshFilter.mesh);

        Utils.EnsurePositionIsCentroid(newTree.transform);
        CuttableMeshPhysicsManager cmpm  = newTree.AddComponent<CuttableMeshPhysicsManager>();
        newTree.AddComponent<TreeFallParticle>();
        

        cmpm.GenerateMeshColliderFromCut(newMeshFilter.mesh, true);

        

        return cmpm;

    }

    private TriangleSplitState TriangleToPlaneCheck(Vector3 transformedV0, Vector3 transformedV1,Vector3 transformedV2,Vector3 position,Vector3 normal)
    {
        bool v0AbovePlane = Utils.IsPointAbovePlane(transformedV0, position, normal);
        bool v1AbovePlane = Utils.IsPointAbovePlane(transformedV1, position, normal);
        bool v2AbovePlane = Utils.IsPointAbovePlane(transformedV2, position, normal);

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

                break;



        }
    }

    private void IntersectingFaceSplit(Matrix4x4 world,Face face,Vector3 position,Vector3 normal,Vector3[] worldTrianglePointPositions,PrimitiveMesh lowerPrimitiveMesh,PrimitiveMesh upperPrimitiveMesh )
    {
        List<Vector3> foundIntersectionPoint;
        UnOptimizedGetFaceToPlaneIntersectionPoints(world, face, position, normal, out foundIntersectionPoint);

        int[] faceIndices = face.ToIntArray();
        bool[] triangleState = new bool[6];

        triangleState[0] = Utils.IsPointAbovePlane(worldTrianglePointPositions[0], position, normal);
        triangleState[1] = Utils.IsPointAbovePlane(worldTrianglePointPositions[1], position, normal);
        triangleState[2] = Utils.IsPointAbovePlane(worldTrianglePointPositions[2], position, normal);
        triangleState[3] = Utils.IsPointAbovePlane(worldTrianglePointPositions[3], position, normal);
        triangleState[4] = Utils.IsPointAbovePlane(worldTrianglePointPositions[4], position, normal);
        triangleState[5] = Utils.IsPointAbovePlane(worldTrianglePointPositions[5], position, normal);

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

        if(uniqueTrianglesBelowSplittingPlane.Count < 2 && uniqueTrianglesAboveSplittingPlane.Count < 2) { return; }

        //Debug.Log("---------------- Getting ordering Direction --------------------------");

        basePosition = preCutCentroid;
        baseDirection = Vector3.Cross(Vector3.up, (worldTrianglePointPositions[0] - preCutCentroid)).normalized;

        //Debug.Log("precut centroid " + preCutCentroid.ToString("F2"));
        //Debug.Log("baseDirection" + baseDirection.ToString("F2"));
        //Debug.Log("uniqueIntersectionPoints[0] " + worldTrianglePointPositions[0].ToString("F2"));

       

        IntersectionComparer ic = new IntersectionComparer
            (baseDirection, basePosition,world);

        IndexDirectionComparer idc = new IndexDirectionComparer((uniqueIntersectionPoints[uniqueIntersectionPoints.Count-1]- uniqueIntersectionPoints[0]).normalized
            , basePosition, mesh, world);

        uniqueIntersectionPoints.Sort(ic);

        Vector3 belowTriangleCentroid = GetWorldTriangleCentroid(mesh, uniqueTrianglesBelowSplittingPlane, world);
        Vector3 aboveTriangleCentroid = GetWorldTriangleCentroid(mesh, uniqueTrianglesAboveSplittingPlane, world);

        idc.basePosition = uniqueIntersectionPoints[0];
        uniqueTrianglesBelowSplittingPlane.Sort(idc);

        idc.basePosition = uniqueIntersectionPoints[0];
        uniqueTrianglesAboveSplittingPlane.Sort(idc);


        List<Vector3> intersectionPoints = new List<Vector3>();

        intersectionPoints.Add(uniqueIntersectionPoints[0]);
        intersectionPoints.Add(uniqueIntersectionPoints[uniqueIntersectionPoints.Count-1]);


        //------------------------------- create bottom part----------------------------------------------------//

        Vector3 intersectionDirection = intersectionPoints[1] - intersectionPoints[0];

        Vector3 vertexDirection =
             world.MultiplyPoint(mesh.vertices[uniqueTrianglesBelowSplittingPlane[uniqueTrianglesBelowSplittingPlane.Count - 1]]) -
             world.MultiplyPoint(mesh.vertices[uniqueTrianglesBelowSplittingPlane[0]])
           ;

        if (Vector3.Dot(intersectionDirection, baseDirection) < 0)
        {
            intersectionPoints.Reverse();
        }

        if (Vector3.Dot(vertexDirection, intersectionDirection) < 0)
        {
            uniqueTrianglesBelowSplittingPlane.Reverse();
        }

        // Debug.Log("------------below splitting plane vertex list");
        // DEBUG_logIndicesList(mesh, world, uniqueTrianglesBelowSplittingPlane);

        assembleFacesFromSplitVertices(intersectionPoints, uniqueTrianglesBelowSplittingPlane, false, world, lowerPrimitiveMesh);

        //------------------------------- create above part----------------------------------------------------//

        Vector3 intersectionDirection2 = intersectionPoints[1] - intersectionPoints[0];

        Vector3 vertexDirection2 =
             world.MultiplyPoint(mesh.vertices[uniqueTrianglesAboveSplittingPlane[uniqueTrianglesAboveSplittingPlane.Count - 1]])-
             world.MultiplyPoint(mesh.vertices[uniqueTrianglesAboveSplittingPlane[0]]) 
           ;

        if(Vector3.Dot(intersectionDirection2,baseDirection) < 0)
        {
            intersectionPoints.Reverse();
        }

        if (Vector3.Dot(vertexDirection2, intersectionDirection2) < 0)
        {
            uniqueTrianglesAboveSplittingPlane.Reverse();
        }
        // Debug.Log("------------above splitting plane vertex list");
        // DEBUG_logIndicesList(mesh, world, uniqueTrianglesAboveSplittingPlane);
        

        assembleFacesFromSplitVertices(intersectionPoints, uniqueTrianglesAboveSplittingPlane, true, world, upperPrimitiveMesh);




        //store vertices restoring the hole that will be generated after cutting the mesh
        IndividualVertex v0 = new IndividualVertex(Matrix4x4.Inverse(world).MultiplyPoint(intersectionPoints[0]), Vector3.up, Vector2.zero);
        IndividualVertex v1 = new IndividualVertex(
            Matrix4x4.Inverse(world).MultiplyPoint(intersectionPoints[intersectionPoints.Count - 1]), 
            Vector3.up, Vector2.zero);
        MeshLidPairing lidPairing = new MeshLidPairing(v0, v1);

        lidPairings.Add(lidPairing);

        DEBUG_intersectionVertices.Add(intersectionPoints[0]);
        DEBUG_intersectionVertices.Add(intersectionPoints[intersectionPoints.Count - 1]);
    }

    private void assembleFacesFromSplitVertices(List<Vector3> uniqueIntersectionPoints,List<int> trianglesInSplitPlane,bool isIntersectionPointBottomLeftVertex, Matrix4x4 world,PrimitiveMesh meshToPopulate)
    {
        Matrix4x4 inverseWorld = Matrix4x4.Inverse(world);

        int iterationCount = uniqueIntersectionPoints.Count > trianglesInSplitPlane.Count ? uniqueIntersectionPoints.Count : trianglesInSplitPlane.Count;
        
        List<ConnectionTypeToCentroid> types =new List<ConnectionTypeToCentroid>();
        
        for (int i = 0; i < iterationCount-1; i++)
        {
            int currentItersectionPointI = GetCurrentIndex(uniqueIntersectionPoints.Count, i);
            Vector3 objectSpaceItersectionPoint = inverseWorld.MultiplyPoint(uniqueIntersectionPoints.ElementAt(currentItersectionPointI));

            bool nextIntersectionPointExist = i + 1 < uniqueIntersectionPoints.Count;
            bool nextTrianglePointExist = i + 1 < trianglesInSplitPlane.Count;

            IndividualVertex bottomLeftVertex;
            IndividualVertex upperLeftVertex;

            if (nextTrianglePointExist && nextIntersectionPointExist)
            {
                IndividualVertex bottomRightVertex;
                IndividualVertex upperRightVertex;
 
                Vector3 nextObjectSpaceIntersectionPoint = inverseWorld.MultiplyPoint(uniqueIntersectionPoints.ElementAt(i + 1));

                //all intersection points are bottom, triangles points are upper
                if (isIntersectionPointBottomLeftVertex)
                {
                    upperLeftVertex = new IndividualVertex(mesh, trianglesInSplitPlane.ElementAt(i));
                    upperRightVertex = new IndividualVertex(mesh, trianglesInSplitPlane.ElementAt(i + 1));

                    bottomLeftVertex = new IndividualVertex(objectSpaceItersectionPoint, upperLeftVertex.normal, new Vector2(1, 1));
                    bottomRightVertex = new IndividualVertex(nextObjectSpaceIntersectionPoint, upperRightVertex.normal, new Vector2(1, 1));

                    IndividualTriangle tri1 = new IndividualTriangle(bottomLeftVertex, upperLeftVertex, upperRightVertex);
                    IndividualTriangle tri2 = new IndividualTriangle(bottomLeftVertex, upperRightVertex,bottomRightVertex);

                    if (tri1.AttemptNormalBasedVertexCorrection(bottomLeftVertex.normal, 1, 2))
                    {
                        tri2.V1 = upperLeftVertex;
                    }

                    if (tri2.AttemptNormalBasedVertexCorrection(bottomLeftVertex.normal, 0, 2))
                    {
                        tri2.V1 = upperRightVertex;
                    }

                    meshToPopulate.AddFaceFromTriangles(tri1, tri2);

                    //store its position and connection type
                    ConnectionTypeToCentroid tri1Type;
                    tri1Type.objectSpaceCentroid = tri1.GetObjectSpaceCentroid();
                    tri1Type.tct = TriangleConnectionType.DoubleOriginalPoint;

                    ConnectionTypeToCentroid tri2Type;
                    tri2Type.objectSpaceCentroid = tri2.GetObjectSpaceCentroid();
                    tri2Type.tct = TriangleConnectionType.DoubleIntersection;


                    types.Add(tri1Type);
                    types.Add(tri2Type);

                }
                //all intersection points are upper, triangles points are bottom
                else
                {
                    bottomLeftVertex = new IndividualVertex(mesh, trianglesInSplitPlane.ElementAt(i));

                    bottomRightVertex = new IndividualVertex(mesh, trianglesInSplitPlane.ElementAt(i + 1));

                    upperLeftVertex = new IndividualVertex(objectSpaceItersectionPoint, bottomLeftVertex.normal, new Vector2(1, 1));

                    upperRightVertex = new IndividualVertex(nextObjectSpaceIntersectionPoint, bottomRightVertex.normal, new Vector2(1, 1));

                    IndividualTriangle tri1 = new IndividualTriangle(bottomLeftVertex, upperLeftVertex, upperRightVertex);
                    IndividualTriangle tri2 = new IndividualTriangle(bottomLeftVertex, upperRightVertex, bottomRightVertex);


                    if (tri1.AttemptNormalBasedVertexCorrection(bottomLeftVertex.normal, 1, 2))
                    {
                        tri2.V1 = upperLeftVertex;
                    }

                    if (tri2.AttemptNormalBasedVertexCorrection(bottomLeftVertex.normal, 0, 2))
                    {
                        tri2.V1 = upperRightVertex;
                    }

                    meshToPopulate.AddFaceFromTriangles(tri1, tri2);
                    
                    ConnectionTypeToCentroid tri1Type;
                    tri1Type.objectSpaceCentroid = tri1.GetObjectSpaceCentroid();
                    tri1Type.tct = TriangleConnectionType.DoubleIntersection;

                    ConnectionTypeToCentroid tri2Type;
                    tri2Type.objectSpaceCentroid = tri2.GetObjectSpaceCentroid();
                    tri2Type.tct = TriangleConnectionType.DoubleOriginalPoint;

                    types.Add(tri1Type);
                    types.Add(tri2Type);

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
                        upperLeftVertex = new IndividualVertex(mesh, trianglesInSplitPlane.ElementAt(currentTriangleIndex));

                        bottomLeftVertex = new IndividualVertex();
                        bottomLeftVertex.position = objectSpaceItersectionPoint;
                        bottomLeftVertex.normal = upperLeftVertex.normal;
                        bottomLeftVertex.UV = new Vector2(1, 1);

                        bottomRightVertex = new IndividualVertex();
                        bottomRightVertex.position = nextObjectSpaceIntersectionPoint;
                        bottomRightVertex.normal = upperLeftVertex.normal;
                        bottomRightVertex.UV = new Vector2(1, 1);

                        IndividualTriangle tri1 = new IndividualTriangle(upperLeftVertex, bottomRightVertex, bottomLeftVertex);

                        tri1.AttemptNormalBasedVertexCorrection(upperLeftVertex.normal, 1, 2);

                        meshToPopulate.AddFaceFromSingularTriangle(tri1);

                    }
                    //all intersection points are upper, triangles points are bottom
                    else
                    {
                        bottomLeftVertex = new IndividualVertex(mesh, trianglesInSplitPlane.ElementAt(currentTriangleIndex));

                        upperLeftVertex = new IndividualVertex();
                        upperLeftVertex.position = objectSpaceItersectionPoint;
                        upperLeftVertex.normal = bottomLeftVertex.normal;
                        upperLeftVertex.UV = new Vector2(1, 1);

                        upperRightVertex = new IndividualVertex();
                        upperRightVertex.position = nextObjectSpaceIntersectionPoint;
                        upperRightVertex.normal = upperLeftVertex.normal;
                        upperRightVertex.UV = new Vector2(1, 1);

                        IndividualTriangle tri1 = new IndividualTriangle(bottomLeftVertex, upperLeftVertex, upperRightVertex);

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

                        bottomLeftVertex = new IndividualVertex(mesh, trianglesInSplitPlane.ElementAt(currentTriangleIndex));
                        bottomRightVertex = new IndividualVertex(mesh, trianglesInSplitPlane.ElementAt(currentTriangleIndex + 1));

                        TriangleConnectionType tct = GetClosestConnectionTypeByCentroidProjection(uniqueIntersectionPoints[0], (uniqueIntersectionPoints[1] - uniqueIntersectionPoints[0]).normalized, types);


                        upperRightVertex = new IndividualVertex(mesh, trianglesInSplitPlane.ElementAt(currentTriangleIndex - 1));

                        Debug.Log("found tct " + tct.ToString());//tct.ToString()
                       
                        if (tct == TriangleConnectionType.DoubleOriginalPoint)
                        {
                            //connects to intersection
                            upperRightVertex = new IndividualVertex(mesh, trianglesInSplitPlane.ElementAt(currentTriangleIndex - 1));
                        }
                        else if(tct == TriangleConnectionType.DoubleIntersection)
                        {
                            //connects to previous triangle
                            upperRightVertex = new IndividualVertex(objectSpaceItersectionPoint, bottomLeftVertex.normal, new Vector2(1, 1));

                        }


                        IndividualTriangle tri1 = new IndividualTriangle(bottomLeftVertex, upperRightVertex, bottomRightVertex);


                        tri1.AttemptNormalBasedVertexCorrection(bottomLeftVertex.normal, 0, 2);

                        Debug.Log("----------------------- Creating Triangle leftover Above------------------------------------s ");
                        Debug.Log(" bottomLeftVertex.position " + world.MultiplyPoint(bottomLeftVertex.position).ToString("F2"));
                        Debug.Log(" upperRightVertex.position  " + world.MultiplyPoint(upperRightVertex.position).ToString("F2"));

                        Debug.Log(" bottomRightVertex.position " + world.MultiplyPoint(bottomRightVertex.position).ToString("F2"));

                        meshToPopulate.AddFaceFromSingularTriangle(tri1);

                    }
                    //all intersection points are upper, triangles points are bottom
                    else
                    {

                        upperLeftVertex = new IndividualVertex(mesh, trianglesInSplitPlane.ElementAt(currentTriangleIndex));
                        upperRightVertex = new IndividualVertex(mesh, trianglesInSplitPlane.ElementAt(currentTriangleIndex+1));

                        bottomLeftVertex = new IndividualVertex(objectSpaceItersectionPoint, upperLeftVertex.normal, new Vector2(1, 1));

                        IndividualTriangle tri1 = new IndividualTriangle(bottomLeftVertex, upperLeftVertex, upperRightVertex);


                        tri1.AttemptNormalBasedVertexCorrection(bottomLeftVertex.normal, 1, 2);
                        meshToPopulate.AddFaceFromSingularTriangle(tri1);

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
                    DEBUG_intersectionVertices.Add(intersectionPoint);
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

    //---------------------------------- Helper Functions----------------------------------------//

    private List<Vector3> UnOptimizedFindTriangleToPlaneIntersectionPoint(Vector3 transformedV0, Vector3 transformedV1, Vector3 transformedV2, Vector3 position, Vector3 normal)
    {
        List<Vector3> result = new List<Vector3>();

        Vector3 intersection1;
        if (UnOptimizedFindLineToPlaneIntersection(transformedV0, transformedV1, position, normal, out intersection1) ||
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

    private bool UnOptimizedFindLineToPlaneIntersection(Vector3 transformedV0, Vector3 transformedV1, Vector3 position, Vector3 normal, out Vector3 intersection)
    {
        Vector3 lineToUse = transformedV1 - transformedV0;

        Vector3 P0 = transformedV0;
        Vector3 P1 = lineToUse.normalized;
        Vector3 A = position;

        float t = (Vector3.Dot(A, normal) - Vector3.Dot(P0, normal)) / Vector3.Dot(P1, normal);
        
        intersection = P0 + P1 * t;

        return t > 0.0f && t < lineToUse.magnitude;

    }

    private int GetCurrentIndex(int maxIndex, int requestedIndex)
    {
        //Mat
        return requestedIndex > maxIndex - 1 ? maxIndex - 1 : requestedIndex; 
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

    private Vector3 GetWorldTriangleCentroid(Mesh mesh, List<int> indices,Matrix4x4 world)
    {
        Vector3 result = Vector3.zero;
        foreach(var x in indices)
        {
            result += mesh.vertices[x];
        }

        result /= indices.Count;


        return world.MultiplyPoint(result);
    }

    //base position and desiredDirection is expressed in world space
    private TriangleConnectionType GetClosestConnectionTypeByCentroidProjection(Vector3 basePosition,Vector3 desiredDirection, List<ConnectionTypeToCentroid> cttc)
    {
        TriangleConnectionType tct = TriangleConnectionType.DefaultNoType;
        float closestConnectionLength = float.MinValue;

        Matrix4x4 world = Matrix4x4.TRS(transform.position, transform.rotation, transform.localScale);



        foreach (ConnectionTypeToCentroid singleTypeToCentroid in cttc)
        {
            Vector3 worldSpaceCentroid = world.MultiplyPoint(singleTypeToCentroid.objectSpaceCentroid);
            Vector3 currentDirection = (worldSpaceCentroid - basePosition).normalized;

            float currentFoundClosest = Vector3.Dot(currentDirection, desiredDirection);

            Debug.Log("Found " + singleTypeToCentroid.tct.ToString() + "with val " + currentFoundClosest);
            Debug.Log("The centroid is on " + worldSpaceCentroid.ToString("F2"));

            if (currentFoundClosest > closestConnectionLength)
            {
                closestConnectionLength = currentFoundClosest;
                tct = singleTypeToCentroid.tct;
            }

        }
        return tct;
    }

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

        foreach (var position in DEBUG_intersectionVertices)
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

