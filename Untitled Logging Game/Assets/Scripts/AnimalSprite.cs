using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimalSprite : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        Vector3 lookAtPosition = new Vector3(cameraPosition.x, transform.position.y, cameraPosition.y);

        transform.LookAt(lookAtPosition);
    }
}
