using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MapZoomIn : MonoBehaviour
{

    public Transform[] zooms = new Transform[3];
    public Camera mainCam;
    public float zoomTime;
    public string[] sceneLoads = new string[3];
    public Transform defaultZoom;
    private SceneMan sceneMan;
    public GameObject[] mapButtons = new GameObject[3];
    public GameObject[] mapPins = new GameObject[3];
    public GameObject[] mainMenu = new GameObject[3];
    public GameObject nameSelector;
    public Transform nameZoom;
    public TextMeshProUGUI nameField;

    private void Awake()
    {
        sceneMan = FindObjectOfType<SceneMan>();
        foreach (var but in mapButtons)
        {
            but.SetActive(false);
        }
        nameSelector.SetActive(false);
        mapPins[1].SetActive(false);
        mapPins[2].SetActive(false);
    }

    public void Zoom1()
    {
        mainCam.transform.LeanMove(zooms[0].transform.position, zoomTime);
        mainCam.transform.LeanRotate(zooms[0].rotation.eulerAngles, zoomTime);
        mapButtons[2].SetActive(false);
        StartCoroutine(LoadScene(sceneLoads[0]));
    }
    
    public void Zoom2()
    {
        mainCam.transform.LeanMove(zooms[1].transform.position, zoomTime);
        mainCam.transform.LeanRotate(zooms[1].rotation.eulerAngles, zoomTime);
        StartCoroutine(LoadScene(sceneLoads[1]));
    }
    
    public void Zoom3()
    {
        mainCam.transform.LeanMove(zooms[2].transform.position, zoomTime);
        mainCam.transform.LeanRotate(zooms[2].rotation.eulerAngles, zoomTime);
        StartCoroutine(LoadScene(sceneLoads[2]));
    }

    public void ZoomName()
    {
        mainCam.transform.LeanMove(nameZoom.transform.position, zoomTime);
        mainCam.transform.LeanRotate(nameZoom.rotation.eulerAngles, zoomTime);
        

        foreach (var part in mainMenu)
        {
            part.SetActive(false);   
        }

        StartCoroutine(CompleteNameLoad(zoomTime));
    }

    IEnumerator CompleteNameLoad(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        nameSelector.SetActive(true);
    }

    public void ZoomMap()
    {
        mainCam.transform.LeanMove(defaultZoom.transform.position, zoomTime);
        mainCam.transform.LeanRotate(defaultZoom.rotation.eulerAngles, zoomTime);
        mapButtons[0].SetActive(true);
        mapPins[0].SetActive(true);
        FlipFocusMaterial(mapPins[0]);
        
        nameSelector.SetActive(false);
    }

    public void ZoomOut()
    {
        Debug.Log("Old position was " + mainCam.transform.position.x + "  " + mainCam.transform.position.y + "  " + mainCam.transform.position.z);
        mainCam.transform.LeanMove(defaultZoom.transform.position, zoomTime);
        mainCam.transform.LeanRotate(defaultZoom.rotation.eulerAngles, zoomTime);
    }
    
    IEnumerator LoadScene(string sceneName)
    {
        float timeElapsed = 0;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Single);
        asyncLoad.allowSceneActivation = false;
        sceneMan.prevScene = SceneManager.GetActiveScene().name;
        while(!asyncLoad.isDone && timeElapsed < zoomTime+.5f)
        {
            // Debug.Log(" Time remaining is " + (zoomTime-timeElapsed));
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        asyncLoad.allowSceneActivation = true;
    }

    IEnumerator ReturnToMap(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);
        if(sceneMan.prevScene != "")
        {
            foreach (var but in mapButtons)
            {
                but.SetActive(true);
            }
            foreach (var part in mainMenu)
            {
                part.SetActive(false);   
            }

            foreach (var pin in mapPins)
            {
                pin.SetActive(true);
            }
            
            nameField.text = sceneMan.playerName;
            nameSelector.SetActive(false);
            
            ZoomOut();
        }
        if (sceneMan.prevScene == sceneLoads[0])
        {
            mainCam.transform.position = zooms[0].position;
            mainCam.transform.rotation = zooms[0].rotation;
            mapButtons[0].SetActive(false);
            FlipFocusMaterial(mapPins[1]);
            mapButtons[2].SetActive(false);
            mapPins[2].SetActive(false);
        }
        else if (sceneMan.prevScene == sceneLoads[1])
        {
            mainCam.transform.position = zooms[1].position;
            mainCam.transform.rotation = zooms[1].rotation;
            mapButtons[0].SetActive(false);
            mapButtons[1].SetActive(false);
            FlipFocusMaterial(mapPins[2]);
        }
        else if (sceneMan.prevScene == sceneLoads[2])
        {
            mainCam.transform.position = zooms[2].position;
            mainCam.transform.rotation = zooms[2].rotation;
        }
        
        // Debug.Log("Zoom me out, scotty !");
    }
    
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
         
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }
         
    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        StartCoroutine(ReturnToMap(.01f));
    }

    private void FlipFocusMaterial(GameObject pinParent)
    {
        pinParent.GetComponentInChildren<Light>().intensity += 0.003f;
    }
}
