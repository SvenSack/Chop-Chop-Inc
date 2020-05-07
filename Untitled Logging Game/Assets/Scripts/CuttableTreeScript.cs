using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;




public struct Triangle
{
    public void Init()
    {
        v0 = -1;
        v1 = -1;
        v2 = -1;
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
}

public struct IndividualTriangle
{
    public IndividualVertex V0;
    public IndividualVertex V1;
    public IndividualVertex V2;
}

public struct IndividualFace
{
    public IndividualTriangle tri1;
    public IndividualTriangle tri2;

    public bool isFaceComplete;
}


public class PrimitiveMesh
{
    public List<IndividualFace> individualFaces;

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


        IndividualTriangle tri1;
        tri1.V0 = V0;
        tri1.V1 = V1;
        tri1.V2 = V2;

        IndividualTriangle tri2;
        tri2.V0 = V3;
        tri2.V1 = V4;
        tri2.V2 = V5;

        IndividualFace newFace;
        newFace.tri1 = tri1;
        newFace.tri2 = tri2;
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


        IndividualTriangle tri1;
        tri1.V0 = V0;
        tri1.V1 = V1;
        tri1.V2 = V2;

        IndividualTriangle tri2;
        tri2.V0 = V3;
        tri2.V1 = V4;
        tri2.V2 = V5;

        IndividualFace newFace;
        newFace.tri1 = tri1;
        newFace.tri2 = tri2;
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

        mesh.vertices = verticesList.ToArray();

        for (int i = 0; i < mesh.vertices.Length; i++)
        {
            indicesList.Add(i);
        }


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
                if(tri1CheckResult == tri2CheckResult)
                {
                    FindDecisionForFace(worldMatrix, tri1CheckResult, face, position, normal,FacesSplitAbove,FacesSplitBelow);
                }
                //if both triangles exist but do not have the same triangle split state
                else if(isBothTrianglesExist && tri1CheckResult != tri2CheckResult)
                {
                    //if first triangle is split and one is not
                        //send split first triangle as a face
                        //send leftover geometry as second face


                    //if second triangle is split and one is not
                        //send split second triangle as a face
                        //send leftover geometry as  second face

                }
                //one of the triangles in the face do not exist
                else if (!isBothTrianglesExist)
                {
                    if(hasTriangle1)
                    {
                        FindDecisionForSingularTriangle(worldMatrix, tri1CheckResult, face.tri1, position, normal);
                    }
                    if(hasTriangle2)
                    {
                        FindDecisionForSingularTriangle(worldMatrix, tri2CheckResult, face.tri2, position, normal);
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
        bool v0AbovePlane = Vector3.Dot(transformedV0 - position,normal) > 0;
        bool v1AbovePlane = Vector3.Dot(transformedV1 - position, normal) > 0;
        bool v2AbovePlane = Vector3.Dot(transformedV2 - position, normal) > 0;

        if(v0AbovePlane && v1AbovePlane && v2AbovePlane)
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
                UnOptimizedGetFaceToPlaneIntersectionPoints(world, face, position, normal);

                break;
        }
    }

    private void FindDecisionForSingularTriangle(Matrix4x4 world, TriangleSplitState state, Triangle tri, Vector3 position, Vector3 normal)
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



                break;

            case TriangleSplitState.BelowPlane:

                bottomVertices.Add(worldV0);
                bottomVertices.Add(worldV1);
                bottomVertices.Add(worldV2);

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
    private void UnOptimizedGetFaceToPlaneIntersectionPoints(Matrix4x4 world, Face face, Vector3 position,Vector3 normal)
    {
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
            intersectionVertices.Add(intersectionPoint);
        }

        foreach (var intersectionPoint in secondTriangleIntersectionPoints)
        {
            intersectionVertices.Add(intersectionPoint);
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

        return t > 0.0f && t < 1.0f;

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

            if (tri1Normal.Equals(tri2Normal))
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

}

//store effected Collision Boxes

//store un-effected Collision Boxes

//from un-effected, store collision boxes below splitting plane 

//from un-effected, store collision boxes above splitting plane 

//for each effected collision box
    //for each face
