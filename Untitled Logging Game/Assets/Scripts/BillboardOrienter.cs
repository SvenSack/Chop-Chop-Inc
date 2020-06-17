using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BillboardOrienter : MonoBehaviour
{
    private Vector3 cameraPosition;
    
    // Start is called before the first frame update
    void Start()
    {
        cameraPosition = Camera.main.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        transform.LookAt(cameraPosition);
    }
}
