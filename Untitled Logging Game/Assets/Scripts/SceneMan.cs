
using System;
using UnityEngine;

public class SceneMan : MonoBehaviour
{
    
    private static SceneMan instance;

    public string prevScene = "";
    public string playerName = "";

    private float idleTimer;
    public float idleTime = 30f;
    private bool isIdle;

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
        if (isIdle)
        {
            if (idleTimer >= idleTime)
            {
                // reset game
            }
            else
            {
                idleTimer += Time.deltaTime;
            }
        }
    }

    public void setIdle(bool newValue)
    {
        if (newValue)
        {
            isIdle = newValue;
        }
        else
        {
            isIdle = newValue;
            idleTimer = 0;
        }
    }
}
