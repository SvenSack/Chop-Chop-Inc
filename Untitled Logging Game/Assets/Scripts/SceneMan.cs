
using System;
using UnityEngine;

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
            idleTime = 0;

        if (uiMan == null && prevScene != "")
            uiMan = FindObjectOfType<UIMan>();
        
        if (idleTimer >= idleTime)
        {
            // ask raphael how exactly we are doing the reset, then do that
        }
        else
        {
            idleTimer += Time.deltaTime;
            if(idleTimer > idleTime/2 && prevScene != "")
                uiMan.TryVoiceLine(0);
        }
        
    }
}
