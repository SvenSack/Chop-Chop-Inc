using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapZoomIn : MonoBehaviour
{

    public Transform zoom1;
    public Camera mainCam;
    public float zoomTime;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Zoom1()
    {
        mainCam.transform.LeanMove(zoom1.transform.position, zoomTime);
        mainCam.transform.LeanRotate(zoom1.rotation.eulerAngles, zoomTime);
    }
}
