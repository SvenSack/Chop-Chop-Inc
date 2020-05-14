using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VectorDebugShow : MonoBehaviour
{

    public Vector3 debugShow;



    public Vector3 position1;
    public Vector3 position2;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        Debug.DrawLine(position1, position1 + debugShow);
        Debug.DrawLine(position1,position2);
    }
}
