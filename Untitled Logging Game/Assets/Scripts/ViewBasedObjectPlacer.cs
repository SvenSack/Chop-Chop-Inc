using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public delegate void OnObjectInstantiated(GameObject instantiatedObj);

public class ViewBasedObjectPlacer : MonoBehaviour
{
    public GameObject gameObjectToPlace;
    public float screenSpaceShiftInX = 50.0f;
    public float screenSpaceShiftInY = 20.0f;
    public float screenSpaceShiftInZ = 50.0f;

    public Vector3 allowedWorldShift = new Vector3(20,0,20);

    public float objectSpecificYShift = -4.0f;

    public float maxZDistance = 800.0f;

    [Range(0,1.0f)]public float density = 0.08f;

    private Camera camera;

    [SerializeField]private Terrain terrain;

    private OnObjectInstantiated onObjectInstantiated;

    public float minChildScale = 0.5f;
    public float maxChildScale = 3.5f;

    private void Start()
    {
        camera = Camera.main;
        onObjectInstantiated += scaleChildren;

    }

    public void RandomViewBasedObjectPlace()
    {


        //---------------------- Create a cube representing the position
        //of the objects in screen space and fill it with positions where we would want to put the objects
        //---------------------------------//

        float cameraWidth = camera.pixelWidth;
        float cameraHeight = camera.pixelHeight;

        List<Vector3> placementSpots = new List<Vector3>();

        Vector3 currentPlacementSpot = Vector3.zero;

        while(currentPlacementSpot.z < maxZDistance && currentPlacementSpot.y < cameraHeight)
        {
            int placedInX = (int)(cameraWidth / screenSpaceShiftInX);

            for (int i = 0; i < placedInX; i++)
            {
                //check if should place here or not
                if(CalculateIfShouldPlaceHere())
                {
                    placementSpots.Add(currentPlacementSpot);
                }

                //shift in x 

                currentPlacementSpot.x += screenSpaceShiftInX;

            }

            //shift in y and z
            currentPlacementSpot.y += screenSpaceShiftInY;
            currentPlacementSpot.z += screenSpaceShiftInZ;

            //set x back to zero
            currentPlacementSpot.x = 0;
        }




        //-----------------shift positions into world space and to be on terrain---------------------------------//

        for (int i = 0; i < placementSpots.Count; i++)
        {
            placementSpots[i] = camera.ScreenToWorldPoint(placementSpots[i]);

            placementSpots[i] = new Vector3(
                placementSpots[i].x + Random.Range(-allowedWorldShift.x, allowedWorldShift.x),
                placementSpots[i].y ,
                placementSpots[i].z + Random.Range(-allowedWorldShift.z, allowedWorldShift.z)
                );


            placementSpots[i] = new Vector3(
                placementSpots[i].x ,
                terrain.SampleHeight(placementSpots[i]) + terrain.GetPosition().y + objectSpecificYShift,
                placementSpots[i].z) ;



        }


        //-------------Actually start Instantiating the objects------------------------------//

        foreach(Vector3 spot in placementSpots)
        {
            var gameObj = Instantiate(gameObjectToPlace, spot, gameObjectToPlace.transform.rotation);

            onObjectInstantiated?.Invoke(gameObj);
        }

    }

    private bool CalculateIfShouldPlaceHere()
    {
        return Random.Range(0, 1.0f) < density;
    }

    

    private void scaleChildren(GameObject obj)
    {
        foreach(Transform child in obj.transform)
        {
            float newScale = Random.Range(minChildScale, maxChildScale);
            child.localScale = new Vector3(newScale,newScale,newScale);
        }
    }

    private void Update()
    {
        
    }

}
