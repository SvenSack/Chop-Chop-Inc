using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeFallParticle : MonoBehaviour
{
    private ParticleSystem leaves;
    private ParticleSystem dust;
    
    
    
    // Start is called before the first frame update
    void Start()
    {
        ParticleSystem[] temp = transform.GetComponentsInChildren<ParticleSystem>();
        leaves = temp[0];
        dust = temp[1];
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
        }
    }
}
