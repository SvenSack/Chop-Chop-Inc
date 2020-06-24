
using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneMan : MonoBehaviour
{
    
    private static SceneMan instance;

    public string prevScene = "";
    public string playerName = "";

    private float idleTimer;
    public float idleTime = 40f;
    private UIMan uiMan;
    

    void Awake() {
        if(!instance )
            instance = this;
        else {
            Destroy(gameObject) ;
            return;
        }

        DontDestroyOnLoad(gameObject) ;
    }

    private void Update()
    {
        if (Input.GetMouseButton(0))
            idleTimer = 0;

        if (uiMan == null && prevScene != "")
            uiMan = FindObjectOfType<UIMan>();
        
        if (idleTimer >= idleTime)
        {
            idleTimer = 0;
            Debug.Log("back to menu with you, Y E E T");
            FindObjectOfType<HighScoreManager>().ResetLevelDataInFile();
            // the following is a manual method call because f.u. I am lazy right now
            GameObject[] objects = GameObject.FindGameObjectsWithTag("DDOL");

            foreach(GameObject toDestroy in objects)
            {
                Destroy(toDestroy);
            }
            SceneManager.LoadScene("Map");
        }
        else
        {
            idleTimer += Time.deltaTime;
            if(idleTimer > idleTime/2 && prevScene != "")
                uiMan.TryVoiceLine(0);
        }
        
    }
}
