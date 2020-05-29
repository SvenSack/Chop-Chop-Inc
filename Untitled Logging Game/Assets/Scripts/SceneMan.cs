
using UnityEngine;

public class SceneMan : MonoBehaviour
{
    
    private static SceneMan instance;

    public string prevScene = "";

    void Awake() {
        if(!instance )
            instance = this;
        else {
            Destroy(gameObject) ;
            return;
        }

        DontDestroyOnLoad(gameObject) ;
    }
}
