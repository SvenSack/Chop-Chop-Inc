using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = System.Random;

public class CutMan : MonoBehaviour
{
    private GameObject currentCut;
    private Vector2 cutStart;
    private Vector2 cutUpdate;
    private bool isCutting;
    
    public GraphicRaycaster gRaycaster;
    public EventSystem eventSystem;
    public float marginOfError;
    private int trunkMask;

    public GameObject cutParticleObject;
    public GameObject fallParticleObject;
    private GameObject cutParticleInstance;

    public CuttableTreeScript[] trees;
    public int[] treeHps;
    private CutTarget[] cutTargets;
    public GameObject cutTargetPrefab;

    private SoundMan soundMan;

    public float[] leafScaleValues;
    public Sprite[] leafParticles;

    public float cutForce;
    
    
    private void Awake()
    {
        soundMan = FindObjectOfType<SoundMan>();
        trunkMask = LayerMask.GetMask("Trunks");
        trees = FindObjectsOfType<CuttableTreeScript>();
        treeHps = new int[trees.Length];
        cutTargets = new CutTarget[trees.Length];
        // ### the following should be replaced once the placement is algorithmically determined,
        // then we could just look it up there
        for (int i = 0; i < treeHps.Length; i++)
        {
            treeHps[i] = UnityEngine.Random.Range(1, 3);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            List<RaycastResult> castHits = new List<RaycastResult>();
            PointerEventData eventPoint = new PointerEventData(eventSystem);
            eventPoint.position = Input.mousePosition;
            gRaycaster.Raycast(eventPoint, castHits);
            if (castHits.Count > 0)
            {
                for (int i = 0; i < castHits.Count; i++)
                {
                    // Debug.Log("I hit " + castHits[i].gameObject.name);
                    if (castHits[i].gameObject.GetComponent<CutTarget>() != null)
                    {
                        RectTransform rec = castHits[i].gameObject.GetComponent<RectTransform>();
                        Vector3[] corners = new Vector3[4];
                        rec.GetWorldCorners(corners);
                        float width = Vector3.Distance(Vector3.Lerp(corners[2],corners[3],0.5f),Vector3.Lerp(corners[0],corners[1],0.5f));
                        switch (castHits[i].gameObject.GetComponent<CutTarget>().goesLeft)
                        { 
                            case true:
                                float dist = Vector2.Distance(Vector3.Lerp(corners[2],corners[3],0.5f),
                                    Input.mousePosition);
                                if (dist < width / 5)
                                {
                                    soundMan.StartCut();
                                    currentCut = castHits[i].gameObject;
                                    soundMan.chainsawSoundObject.transform.position = GetMouseWorld();
                                }
                                Debug.DrawLine(corners[2],corners[3],Color.cyan,1000);
                                break;
                            case false:
                                float dist1 = Vector2.Distance(Vector3.Lerp(corners[0],corners[1],0.5f),
                                    Input.mousePosition);
                                if (dist1 < width / 5)
                                {
                                    soundMan.StartCut();
                                    currentCut = castHits[i].gameObject;
                                    soundMan.chainsawSoundObject.transform.position = GetMouseWorld();
                                }
                                Debug.DrawLine(corners[0],corners[1],Color.cyan,1000);
                                break;
                        }
                    }
                }
            }
        }

        if (currentCut != null)
        {
            if (Input.GetMouseButtonUp(0))
            {
                // Debug.Log("Stopped cutting because click lift");
                soundMan.StopCut();
                if(cutParticleInstance != null)
                    foreach (var part in cutParticleInstance.GetComponentsInChildren<ParticleSystem>())
                    {
                        part.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                    }
                soundMan.chainsawSoundObject.transform.position = GetMouseWorld();
                currentCut = null;
                isCutting = false; 
            }
            else
            {
                List<RaycastResult> castHits = new List<RaycastResult>();
                PointerEventData eventPoint = new PointerEventData(eventSystem);
                eventPoint.position = Input.mousePosition;
                gRaycaster.Raycast(eventPoint, castHits);
                bool hit = false;
                if (castHits.Count > 0)
                {
                    for (int i = 0; i < castHits.Count; i++)
                    {
                        if (castHits[i].gameObject == currentCut)
                        {
                            // Debug.Log("Cut continues");
                            hit = true;
                        }
                    }
                }

                if (!hit)
                {
                    Debug.Log("Stopped cutting because click left");
                    soundMan.StopCut();
                    if(cutParticleInstance != null)
                        foreach (var part in cutParticleInstance.GetComponentsInChildren<ParticleSystem>())
                        {
                            part.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                        }
                    soundMan.chainsawSoundObject.transform.position = GetMouseWorld();
                    currentCut = null;
                    isCutting = false;
                }
            }
            
        }

        if (currentCut != null)
        {
            RaycastHit hit; 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 100000, trunkMask))
            {
                if (hit.collider.gameObject.GetComponent<CuttableTreeScript>() ==
                    currentCut.GetComponent<CutTarget>().target)
                {
                    Debug.DrawRay (ray.origin, ray.direction * 50000000, Color.green, 100f);
                    if (!isCutting)
                    {
                        // Debug.Log("hit tree");
                        isCutting = true;
                        soundMan.ToggleWood();
                        cutParticleInstance = Instantiate(cutParticleObject);
                        if(currentCut.GetComponent<CutTarget>().goesLeft)
                            cutParticleInstance.transform.rotation = Quaternion.Euler(0,0,180);
                        foreach (var part in cutParticleInstance.GetComponentsInChildren<ParticleSystem>())
                        {
                            part.Play();
                        }
                        cutParticleInstance.transform.position = hit.point;
                        soundMan.chainsawSoundObject.transform.position = hit.point;
                        cutStart = Input.mousePosition;
                    }
                    else
                    {
                        // Debug.Log("still hitting tree");
                        cutUpdate = Input.mousePosition;
                        cutParticleInstance.transform.position = hit.point;
                        soundMan.chainsawSoundObject.transform.position = hit.point;
                    }
                }
                else
                {
                    
                } 
            }
            else
            {
                Debug.DrawRay (ray.origin, ray.direction * 50000000, Color.red, 100f);
                if (isCutting)
                {
                    float dist = Vector2.Distance(cutStart, Input.mousePosition);
                    Debug.Log("stopped hitting tree at distance " + dist);
                    if (Mathf.Abs(dist) > marginOfError)
                    {
                        GameObject cutPlane = InitiateCut(cutStart, cutUpdate);
                        CuttableTreeScript target = currentCut.GetComponent<CutTarget>().target;
                        GameObject newTreePiece = target.CutAt(cutPlane.transform.position, cutPlane.transform.up, cutForce);
                        Destroy(cutPlane); // you can comment this for debugging of cut plane
                        GameObject newPartI = Instantiate(fallParticleObject, newTreePiece.transform);
                        ParticleSystem newPart = newPartI.transform.GetChild(0).GetComponent<ParticleSystem>();
                        int newPartIndex = target.leafParticleIndex;
                        newPart.textureSheetAnimation.SetSprite(0,leafParticles[newPartIndex]);
                        if (leafScaleValues[newPartIndex] != 1)
                        {
                            var newPartMain = newPart.main;
                            newPartMain.startSizeMultiplier = leafScaleValues[newPartIndex];
                        }
                        var newPartShape = newPart.shape;
                        newPartShape.mesh = newTreePiece.GetComponent<MeshFilter>().mesh;
                        newPart.Play();
                        for (int i = 0; i < trees.Length; i++)
                        {
                            if (trees[i] == currentCut.GetComponent<CutTarget>().target)
                                treeHps[i]--;
                        }
                        Destroy(currentCut);
                    }
                    foreach (var part in cutParticleInstance.GetComponents<ParticleSystem>())
                    {
                        part.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                    }
                    soundMan.StopCut();
                    foreach (var part in cutParticleInstance.GetComponentsInChildren<ParticleSystem>())
                    {
                        part.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                    }
                    soundMan.chainsawSoundObject.transform.position = GetMouseWorld();
                    currentCut = null;
                    isCutting = false;
                }
            }
        }
        
        PlaceCutSpots();
    }

    private GameObject InitiateCut(Vector2 start, Vector2 finish)
    {
        RaycastHit hit; 
        
        Ray ray = Camera.main.ScreenPointToRay(start);
        Physics.Raycast(ray, out hit);
        // ### add mask and layer for cut-able trees to avoid bug
        Vector3 startPoint = hit.point;
        
        Ray ray2 = Camera.main.ScreenPointToRay(finish);
        Physics.Raycast(ray2, out hit);
        // ### add mask and layer for cut-able trees to avoid bug
        Vector3 finishPoint = hit.point;
        
        Vector3 targetLocation = Vector3.Lerp(startPoint, finishPoint, 0.5f);
        float targetZRotation = Vector3.Angle(startPoint, finishPoint);
        Vector3 targetRotation = Vector3.zero;
        targetRotation.z = targetZRotation;

        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
        plane.transform.position = targetLocation;
        plane.transform.Rotate(targetRotation);
        return plane;
    }

    private Vector3 GetMouseWorld()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        float dist = Vector3.Distance(Camera.main.transform.position,
            currentCut.GetComponent<CutTarget>().target.transform.position);
        Debug.DrawRay(ray.origin, ray.direction * dist, Color.yellow, 10f);
        return ray.GetPoint(dist);
    }

    private void PlaceCutSpots()
    {
        for (int i = 0; i < cutTargets.Length; i++)
        {
            if (cutTargets[i] == null && treeHps[i] > 0)
            {
                cutTargets[i] = Instantiate(cutTargetPrefab, gRaycaster.transform).GetComponent<CutTarget>();
                cutTargets[i].target = trees[i];
                if (UnityEngine.Random.Range(0, 1) == 0)
                    cutTargets[i].goesLeft = true;
                else
                    cutTargets[i].goesLeft = false;
                Transform tempTrans = cutTargets[i].transform;
                BoxCollider[] boxes = trees[i].transform.parent.GetComponentsInChildren<BoxCollider>();
                Vector3 targetPosition = Vector3.zero;
                switch (treeHps[i])
                {
                    case 3:
                        Vector3 roof = boxes[3].transform.position + new Vector3(0, .5f * boxes[3].transform.localScale.y, 0);
                        Vector3 floor = boxes[3].transform.position - new Vector3(0, .5f * boxes[3].transform.localScale.y, 0);
                        targetPosition = boxes[3].transform.position;
                        Debug.DrawLine(roof,floor,Color.red,100f);
                        targetPosition.y = UnityEngine.Random.Range(roof.y, floor.y);
                        break;
                    case 2:
                        Vector3 roof1 = boxes[2].transform.position + new Vector3(0, .5f * boxes[2].transform.localScale.y, 0);
                        Vector3 floor1 = boxes[2].transform.position - new Vector3(0, .5f * boxes[2].transform.localScale.y, 0);
                        targetPosition = boxes[2].transform.position;
                        Debug.DrawLine(roof1,floor1,Color.red,100f);
                        targetPosition.y = UnityEngine.Random.Range(roof1.y, floor1.y);
                        break;
                    case 1:
                        Vector3 roof2 = boxes[1].transform.position + new Vector3(0, .5f * boxes[1].transform.localScale.y, 0);
                        Vector3 floor2 = boxes[1].transform.position - new Vector3(0, .5f * boxes[1].transform.localScale.y, 0);
                        targetPosition = boxes[1].transform.position;
                        Debug.DrawLine(roof2,floor2,Color.red,100f);
                        targetPosition.y = UnityEngine.Random.Range(roof2.y, floor2.y);
                        break;
                }
                tempTrans.position = Camera.main.WorldToScreenPoint(targetPosition);
                tempTrans.rotation = Quaternion.Euler(0,0,UnityEngine.Random.Range(-20f,20f));
                
            }
        }
    }
    
    
}
