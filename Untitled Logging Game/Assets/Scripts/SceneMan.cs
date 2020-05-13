using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneMan : MonoBehaviour
{
    
    public static SceneMan instance = null;

    public string prevScene = "";

    void Awake() {
        if(!instance )
            instance = this;
        else {
            Destroy(this.gameObject) ;
            return;
        }

        DontDestroyOnLoad(this.gameObject) ;
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
