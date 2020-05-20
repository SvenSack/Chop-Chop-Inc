using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeFallParticle : MonoBehaviour
{
    private ParticleSystem leaves;
    private ParticleSystem dust;

    public AudioSource fallSound;
    private SoundMan soundMan;
    
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem[] temp = transform.GetComponentsInChildren<ParticleSystem>();
        leaves = temp[0];
        dust = temp[1];
        soundMan = FindObjectOfType<SoundMan>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ground")
        {
            leaves.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            dust.Play();
            fallSound.time = 0;
            fallSound.clip = soundMan.treeFall[1];
            fallSound.Play();
        }
    }
}
