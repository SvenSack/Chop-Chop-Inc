using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioCleanup : MonoBehaviour
{
    private AudioSource audio;
    public bool enabled;
    
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!audio.isPlaying && enabled)
        {
            Destroy(gameObject);
        }
    }
}
