using System;
using System.Collections;
using System.Collections.Generic;
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
    public GameObject[] mainMenu = new GameObject[3];

    private void Awake()
    {
        sceneMan = FindObjectOfType<SceneMan>();
        foreach (var but in mapButtons)
        {
            but.SetActive(false);
        }
    }

    public void Zoom1()
    {
        mainCam.transform.LeanMove(zooms[0].transform.position, zoomTime);
        mainCam.transform.LeanRotate(zooms[0].rotation.eulerAngles, zoomTime);
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

    public void ZoomMap()
    {
        mainCam.transform.LeanMove(defaultZoom.transform.position, zoomTime);
        mainCam.transform.LeanRotate(defaultZoom.rotation.eulerAngles, zoomTime);
        foreach (var but in mapButtons)
        {
            but.SetActive(true);
        }

        foreach (var part in mainMenu)
        {
         part.SetActive(false);   
        }
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
        if (sceneMan.prevScene == sceneLoads[0])
        {
            mainCam.transform.position = zooms[0].position;
            mainCam.transform.rotation = zooms[0].rotation;
        }
        else if (sceneMan.prevScene == sceneLoads[1])
        {
            mainCam.transform.position = zooms[1].position;
            mainCam.transform.rotation = zooms[1].rotation;
        }
        else if (sceneMan.prevScene == sceneLoads[2])
        {
            mainCam.transform.position = zooms[2].position;
            mainCam.transform.rotation = zooms[2].rotation;
        }
        if(sceneMan.prevScene != "")
            ZoomOut();
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
}
