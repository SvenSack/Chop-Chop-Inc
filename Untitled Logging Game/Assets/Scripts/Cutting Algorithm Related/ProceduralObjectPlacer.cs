using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;


public class ObjectToSpawnChance
{
    public float min;
    public float max;

    public GameObject objPtr;

    public void ToLog()
    {
        Debug.Log("ObjectToSpawnChance " + objPtr.ToString());
        Debug.Log("min " + min);
        Debug.Log("max " + max);
    }
}

[RequireComponent(typeof(DebugDrawerInterface))]
public class ProceduralObjectPlacer : MonoBehaviour
{
    [Tooltip("The seed that will be used for procedural placement")]
    public int seed = 0;

    [Tooltip("The smallest distance in which one tree should be compared to another tree")]
    [Range(0, 20.0f)] public float CollisionRing = 2.0f;

    [Tooltip("How many trees you will see for every x width")]
    [Range(0, 100.0f)] public float TreePerTerrainWidth = 3.0f;

    [Tooltip("How many trees you will see for every x height")]
    [Range(0, 100.0f)] public float TreePerTerrainLength = 3.0f;

    [Tooltip("The chance of a certain square not having a tree")]
    [Range(0, 1.0f)] public float EmptySpaceChance = 0.05f;

    //[Tooltip("The maximumAngle in which a tree can be placed")]
    //public float maxAngle = 30.0f;
    //[ReadOnly]
    //public Vector3 up = new Vector3(0, 1, 0);

    [Tooltip("Place the objects you want to spawn here")]
    public GameObject[] ObjectSpawnList;

    [Tooltip("Write the spawn chance of those Objects")]
    public float[] spawnChance;

    private Terrain terrain;

    List<Vector3> positions;

    PositionFinder positionFinder;

    List<Vector3> DEBUG_positionDraw = new List<Vector3>();

    [SerializeField] float debugSphereSize;

    private List<ObjectToSpawnChance> objectsToSpawnList = new List<ObjectToSpawnChance>();

    DebugDrawerInterface debugDrawer;

    public bool rotateObjectOnPlacement;

    // Start is called before the first frame update
    void Start()
    {
        terrain = GetComponent<Terrain>();
        debugDrawer = GetComponent<DebugDrawerInterface>();
    }

    public void Init()
    {
        objectsToSpawnList.Clear();
        DEBUG_positionDraw.Clear();
        terrain = GetComponent<Terrain>();
        debugDrawer = GetComponent<DebugDrawerInterface>();
    }

    public void ProcedurallyPlaceObjects()
    {

        

        Random.InitState(seed);
        seed++;

        initializeObjectsToSpawnList();

        float terrainWidth = terrain.terrainData.size.x;
        float terrainLength = terrain.terrainData.size.z;

        Debug.Log("--------------- Procedurally Place Trees ----------------");
        Debug.Log("terrain Width " + terrainWidth);
        Debug.Log("terrain Length " + terrainLength);

        int treeCountInWidth = (int)Mathf.Floor(terrainWidth / TreePerTerrainWidth);
        int treeCountInLength = (int)Mathf.Floor(terrainLength / TreePerTerrainLength);

        
        Vector3 startPoint = transform.position;

        GameObject[] objects = new GameObject[treeCountInWidth * treeCountInLength];

        for(int i =0; i < treeCountInWidth; i++)
        {
            startPoint = transform.position +  Vector3.right  * TreePerTerrainWidth * i;

            for (int j = 0; j < treeCountInLength; j++)
            {
                //get start point
                PositionFinder positionFinder = new FindPositionThroughRandomNumberGeneration(
                    startPoint,
                    treeCountInLength,
                    treeCountInWidth,
                    TreePerTerrainWidth,
                    TreePerTerrainLength,
                    objects,
                    i * treeCountInLength + j,
                    CollisionRing
                    );

                Vector3 oneSquareToForward = Vector3.forward * TreePerTerrainLength;
                //add original start point for next tree
                debugDrawer.AddLine(startPoint, startPoint + oneSquareToForward, Color.red);
                debugDrawer.AddLine(startPoint, startPoint + Vector3.right + Vector3.right * TreePerTerrainWidth, Color.red);

                startPoint += oneSquareToForward;
                
                //get chosen tree to spawn
                GameObject objSpawned = Instantiate(GetWeightedObjectsRandom(ObjectSpawnList, spawnChance));
                
                objSpawned.transform.position = positionFinder.GetObjectPosition();
                
                float y = terrain.SampleHeight(objSpawned.transform.position);

                objSpawned.transform.position = new Vector3(objSpawned.transform.position.x,y, objSpawned.transform.position.z);

                DEBUG_positionDraw.Add(objSpawned.transform.position);

                
                objects[i * treeCountInLength + j] = objSpawned;

                objSpawned.transform.parent = gameObject.transform;

                if(rotateObjectOnPlacement)
                {
                    objSpawned.transform.Rotate(Vector3.up, Random.Range(0, 360));
                }

                //terrain.
            }
        }
    }

    public void DestroyChildren()
    {
        while (transform.childCount > 0)
        {
            foreach (Transform child in transform)
            {
                DestroyImmediate(child.gameObject);
            }
        }
    }

    public void ClearDebugInfo()
    {
        debugDrawer.ClearLines();
        DEBUG_positionDraw.Clear();
    }


    GameObject GetWeightedObjectsRandom(GameObject[] objects, float[] spawnChance)
    {

        float randomNumber = Random.Range(0, 1.0f);


        foreach(var obj in objectsToSpawnList)
        {
            if (randomNumber > obj.min && randomNumber < obj.max)
            {
                return obj.objPtr;
            }
        }

        return objects[0];
    }

    private void initializeObjectsToSpawnList()
    {
        objectsToSpawnList.Clear();
        Debug.Log("GetWeightedObjectsRandom");
        float currentMin = 0.0f;

        for (int i = 0; i < ObjectSpawnList.Length; i++)
        {
            ObjectToSpawnChance objectToSpawnChance = new ObjectToSpawnChance();
            objectToSpawnChance.objPtr = ObjectSpawnList[i];
            objectToSpawnChance.min = currentMin;
            objectToSpawnChance.max = objectToSpawnChance.min + spawnChance[i];
            objectsToSpawnList.Add(objectToSpawnChance);
            currentMin += spawnChance[i];

            objectToSpawnChance.ToLog();

        }
    }


    Vector3 GetTreePosition()
    {
        float width = Random.Range(0, TreePerTerrainWidth);
        float length = Random.Range(0, TreePerTerrainLength);
        
        return Vector3.zero;
    }





    //--------------------------------- Debug stuff---------------------------------//


    private void OnDrawGizmos()
    {
        if (DEBUG_positionDraw != null)
        {
            foreach(var positionDraw in DEBUG_positionDraw)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(positionDraw, debugSphereSize);
            }
        }
    }

}
