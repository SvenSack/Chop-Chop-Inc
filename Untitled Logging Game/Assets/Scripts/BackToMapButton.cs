using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToMapButton : MonoBehaviour
{
    
    public string mapScene;
    private SceneMan sceneMan;

    private void Awake()
    {
        sceneMan = FindObjectOfType<SceneMan>();
    }
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void BackToMap()
    {
        sceneMan.prevScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(mapScene, LoadSceneMode.Single);
    }
}
