using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(TerrainLayerSwitcher))]
public class BurnDownActivator : MonoBehaviour
{
    private delegate void OnBurnDownActivated();
    OnBurnDownActivated BurnDownActivated;

    public TerrainLayer terrainLayerOnSwitch;
    public TerrainLayer oldTerrainLayer;

    public TerrainLayerSwitcher[] layerSwitchers;

    public int[] indexToSwitch;

    private GameObject[] unCuttableTrees;

    public GameObject[] stumpReplacementOptions;


    private void Start()
    {
        BurnDownActivated += ReplaceGroundTexture;
        BurnDownActivated += ReplaceUnCuttableTreesWithStump;
      
        layerSwitchers = GetComponents<TerrainLayerSwitcher>();

        FindUncuttableTrees();

    }

    public void ActivateBurnDown()
    {
        BurnDownActivated.Invoke();
    }

    private void ReplaceGroundTexture()
    {
        int i = 0;

        foreach (var layerSwitcher in layerSwitchers)
        {
            if (i > indexToSwitch.Length) { return; }
            layerSwitcher.SwitchTerrainAtIndexWith(indexToSwitch[i], terrainLayerOnSwitch);
            i++;
        }

    }

    private void ReplaceUnCuttableTreesWithStump()
    {
        foreach(GameObject tree in unCuttableTrees)
        {
            GameObject stumpToInstantiate = Utils.SelectRandomObjectFromCollection
                <GameObject,GameObject[]>(stumpReplacementOptions);

            var instantiatedStump = Instantiate(stumpToInstantiate, tree.transform.position, tree.transform.rotation);
            instantiatedStump.transform.localScale = tree.transform.localScale;

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
        int i = 0;

        foreach (var layerSwitcher in layerSwitchers)
        {
            if(i > indexToSwitch.Length) { return; }
            layerSwitcher.SwitchTerrainAtIndexWith(indexToSwitch[i], oldTerrainLayer);
            i++;
        }

    }

}
