using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(TerrainLayerSwitcher),typeof(ViewBasedObjectPlacer))]
public class BurnDownActivator : MonoBehaviour
{
    private delegate void OnBurnDownActivated();
    OnBurnDownActivated BurnDownActivated;

    public TerrainLayer terrainLayerOnSwitch;
    public TerrainLayer oldTerrainLayer;

    TerrainLayerSwitcher layerSwitcher;
    ViewBasedObjectPlacer viewBasedObjectPlacer;

    public int[] indexToSwitch;

    private GameObject[] unCuttableTrees;

    public GameObject[] stumpReplacementOptions;

    public float[] stumpShiftAmount;

    [Range(0, 1.0f)] public float burnDownDetailDensity = 0.0f;

    private float defaultDetailDensity = 1.0f;


    private void Start()
    {
        

        viewBasedObjectPlacer = GetComponent<ViewBasedObjectPlacer>();
        layerSwitcher = GetComponent<TerrainLayerSwitcher>();

        FindUncuttableTrees();

        defaultDetailDensity = layerSwitcher.terrain.detailObjectDensity;

        BurnDownActivated += ReplaceGroundTexture;
        BurnDownActivated += ReplaceUnCuttableTreesWithStump;
        BurnDownActivated += setNewTerrainDensity;
        BurnDownActivated += viewBasedObjectPlacer.RandomViewBasedObjectPlace;
    }

    public void ActivateBurnDown()
    {
        BurnDownActivated.Invoke();
    }

    private void ReplaceGroundTexture()
    {
        int i = 0;


            if (i > indexToSwitch.Length) { return; }
            layerSwitcher.SwitchTerrainAtIndexWith(indexToSwitch[i], terrainLayerOnSwitch);

            

    }

    private void ReplaceUnCuttableTreesWithStump()
    {
        foreach(GameObject tree in unCuttableTrees)
        {
            int selectIndex;
            GameObject stumpToInstantiate = Utils.SelectRandomObjectFromCollection
                <GameObject,GameObject[]>(stumpReplacementOptions,out selectIndex);

            var instantiatedStump = Instantiate(stumpToInstantiate, tree.transform.position, tree.transform.rotation);
            instantiatedStump.transform.localScale = tree.transform.localScale;

            Vector3 newPosition = instantiatedStump.transform.position;
            newPosition.y = layerSwitcher.terrain.SampleHeight(instantiatedStump.transform.position) ;
            instantiatedStump.transform.position = newPosition;
            instantiatedStump.transform.position += instantiatedStump.transform.up * stumpShiftAmount[selectIndex];


            GameObjectActivator objectActivator;
            instantiatedStump.TryGetComponent(out objectActivator);

            if(objectActivator)
            {
                objectActivator.ActivateObject();
            }

            tree.SetActive(false);
        }
    }



    private void FindUncuttableTrees()
    {
       unCuttableTrees= GameObject.FindGameObjectsWithTag("nocut");
    }

    private void OnApplicationQuit()
    {
        layerSwitcher.terrain.detailObjectDensity = defaultDetailDensity;
        int i = 0;

        if (i > indexToSwitch.Length) { return; }
        layerSwitcher.SwitchTerrainAtIndexWith(indexToSwitch[i], oldTerrainLayer);
        i++;



    }

    private void setNewTerrainDensity()
    {
        layerSwitcher.terrain.detailObjectDensity = burnDownDetailDensity;
    }

}
