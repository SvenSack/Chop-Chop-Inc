using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundMan : MonoBehaviour
{
    public static SoundMan instance = null;
    public AudioClip[] chainsaw = new AudioClip[4];

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

    public void StartCut()
    {
        
    }

    public void HitWood()
    {
        
    }

    public void StopCut()
    {
        
    }
}
