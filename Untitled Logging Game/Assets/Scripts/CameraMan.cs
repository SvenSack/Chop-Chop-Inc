using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMan : MonoBehaviour
{
    public Transform[] zoomLocations;
    public float[] cutDepths;
    private int currentLocation = -1;
    public Camera mainCam;
    public float zoomTime = 5f;
    private CutMan cutMan;

    void Start()
    {
        mainCam = Camera.main;
        cutMan = FindObjectOfType<CutMan>();
    }

    public void MoveOn()
    {
        currentLocation++;
        if(currentLocation <= zoomLocations.Length)
        {
            mainCam.transform.LeanMove(zoomLocations[currentLocation].position, zoomTime);
            mainCam.transform.LeanRotate(zoomLocations[currentLocation].rotation.eulerAngles, zoomTime);
            cutMan.cutTargetDistance = 0;
            StartCoroutine(SetDepth(zoomTime));
        }
        
    }

    IEnumerator SetDepth(float delay)
    {
        yield return new WaitForSeconds(delay);
        cutMan.cutTargetDistance = cutDepths[currentLocation];
    }
}
