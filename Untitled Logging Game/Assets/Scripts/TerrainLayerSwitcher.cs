using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TerrainLayerSwitcher : MonoBehaviour
{
    public Terrain terrain;

    public void Start()
    {
        if(!terrain)
        {
            terrain = Terrain.activeTerrain;
        }
         
    }

    public void SwitchTerrainAtIndexWith(int switchIndex,TerrainLayer terrainLayer)
    {
        TerrainLayer[] layers = terrain.terrainData.terrainLayers;

        if(switchIndex > layers.Length) { return; }

        layers[switchIndex] = terrainLayer;

        terrain.terrainData.terrainLayers = layers;

    }

    public void SwitchTerrain(TerrainLayer terrainLayer, TerrainLayer newterrainLayer)
    {
        Debug.LogError("Switch Terain Not Implemented");
    }

}
