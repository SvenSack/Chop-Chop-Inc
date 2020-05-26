using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NutMover : MonoBehaviour
{
    public float speed;
    public int treeIndex;
    public float floorHeight;
    private float fade = 1;
    private Image nut;
    
    // Start is called before the first frame update
    void Start()
    {
        nut = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        var nutPosition = transform.position;
        if (nutPosition.y > floorHeight)
        {
            transform.Translate(0,-speed*Time.deltaTime,0);
            // Debug.Log(nutPosition.y);
            if (nutPosition.y < floorHeight)
                nutPosition.y = floorHeight;
        }
        else
        {
            if (fade > 0)
            {
                fade -= 1 * Time.deltaTime;
                nut.color = new Color(1,1,1,fade);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void GrabNut()
    {
        Debug.Log("Nut grabbed");
        Destroy(gameObject);
    }
}
