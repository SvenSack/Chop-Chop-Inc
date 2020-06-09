using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMan : MonoBehaviour
{
    public Transform[] zoomLocations;
    [SerializeField] private List<int> treesToCut0;
    [SerializeField] private List<int> treesToCut1;
    [SerializeField] private List<int> treesToCut2;
    [SerializeField] private List<int> treesToCut3;
    public List<int>[] treesToCutEach;
    private int currentLocation = -1;
    public Camera mainCam;
    public float zoomTime = 5f;
    private CutMan cutMan;

    void Start()
    {
        mainCam = Camera.main;
        cutMan = FindObjectOfType<CutMan>();
        treesToCutEach = new[] {treesToCut0, treesToCut1, treesToCut2, treesToCut3};
    }

    public void MoveOn()
    {
        currentLocation++;
        if(currentLocation <= zoomLocations.Length)
        {
            mainCam.transform.LeanMove(zoomLocations[currentLocation].position, zoomTime);
            mainCam.transform.LeanRotate(zoomLocations[currentLocation].rotation.eulerAngles, zoomTime);
            StartCoroutine(SetTargets(zoomTime));
        }
        
    }

    IEnumerator SetTargets(float delay)
    {
        yield return new WaitForSeconds(delay);
        cutMan.currentTargetIndices = treesToCutEach[currentLocation];
    }
}
