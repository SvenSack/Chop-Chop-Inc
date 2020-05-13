using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class CutMechanic : MonoBehaviour
{

    public CuttableTreeScript[] trees = new CuttableTreeScript[1];
    public GameObject[] cutIndicators = new GameObject[1];
    
    private GameObject currentCut;
    private Vector2 cutStart;
    private Vector2 cutUpdate;
    private bool isCutting;
    
    public GraphicRaycaster gRaycaster;
    public EventSystem eventSystem;
    public float marginOfError;

    private SoundMan soundMan;
    
    private void Awake()
    {
        soundMan = FindObjectOfType<SoundMan>();
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
                // ### stop cut particles
                soundMan.StopCut();
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
                    // Debug.Log("Stopped cutting because click left");
                    // ### stop cut particles
                    soundMan.StopCut();
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
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.collider.gameObject.GetComponent<CuttableTreeScript>() ==
                    currentCut.GetComponent<CutTarget>().target)
                {
                    Debug.DrawRay (ray.origin, ray.direction * 50000000, Color.green, 10f);
                    if (!isCutting)
                    {
                        // Debug.Log("hit tree");
                        // ### start cut particles
                        isCutting = true;
                        soundMan.ToggleWood();
                        soundMan.chainsawSoundObject.transform.position = GetMouseWorld();
                        cutStart = Input.mousePosition;
                    }
                    else
                    {
                        // Debug.Log("still hitting tree");
                        cutUpdate = Input.mousePosition;
                        soundMan.chainsawSoundObject.transform.position = GetMouseWorld();
                    }
                }
                else
                {
                    
                } 
            }
            else
            {
                Debug.DrawRay (ray.origin, ray.direction * 50000000, Color.red, 10f);
                if (isCutting)
                {
                    float dist = Vector2.Distance(cutStart, Input.mousePosition);
                    Debug.Log("stopped hitting tree at distance " + dist);
                    if (Mathf.Abs(dist) > marginOfError)
                    {
                        GameObject cutPlane = InitiateCut(cutStart, cutUpdate);
                        CuttableTreeScript target = currentCut.GetComponent<CutTarget>().target;
                        target.CutAt(cutPlane.transform.position, cutPlane.transform.up);
                        Destroy(cutPlane); // you can comment this for debugging
                    }
                    // ### stop cut particles
                    soundMan.StopCut();
                    soundMan.chainsawSoundObject.transform.position = GetMouseWorld();
                    currentCut = null;
                    isCutting = false;
                }
            }
        }
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
}
