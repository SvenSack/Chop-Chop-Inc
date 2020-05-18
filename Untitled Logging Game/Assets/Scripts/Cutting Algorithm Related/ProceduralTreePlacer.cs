using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public class ProceduralTreePlacer : MonoBehaviour
{
    [Tooltip("How many trees you will see for every x width")]
    [Range(0,100.0f)] public float TreePerTerrainWidth = 3.0f;

    [Tooltip("How many trees you will see for every x height")]
    [Range(0, 100.0f)] public float TreePerTerrainLength = 3.0f;

    [Tooltip("The chance of a certain square not having a tree")]
    [Range(0, 1.0f)] public float emptySpaceChance = 0.05f;

    //[Tooltip("The maximumAngle in which a tree can be placed")]
    //public float maxAngle = 30.0f;
    //[ReadOnly]
    //public Vector3 up = new Vector3(0, 1, 0);

    [Tooltip("Place the objects you want to spawn here")]
    public GameObject[] trees;

    [Tooltip("Write the spawn chance of those Objects")]
    public float[] spawnChance;

    private Terrain terrain;

    List<Vector3> positions;


    void ProcedurallyPlaceObjects()
    {
        float terrainWidth = terrain.terrainData.size.x;
        float terrainLength = terrain.terrainData.size.z;

        Debug.Log("--------------- Procedurally Place Trees ----------------");
        Debug.Log("terrain Width " + terrainWidth);
        Debug.Log("terrain Length " + terrainLength);

        int treeCountInWidth = (int)(terrainWidth / TreePerTerrainWidth);
        int treeCountInHeight = (int)(terrainWidth / TreePerTerrainWidth);


        Vector3 startPoint = transform.position;

        for(int i =0; i < treeCountInWidth; i++)
        {
            for (int j = 0; j < treeCountInHeight; j++)
            {
                //get start point


                //choose tree
            }
        }




    }
    // Start is called before the first frame update
    void Start()
    {
        terrain = GetComponent<Terrain>();

        ProcedurallyPlaceObjects();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
