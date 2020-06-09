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
    private UIMan uiMan;
    
    // Start is called before the first frame update
    void Start()
    {
        nut = GetComponent<Image>();
        uiMan = FindObjectOfType<UIMan>();
        transform.LeanRotate(new Vector3(0, 0, Random.Range(-3,2)*360f+(Random.Range(0,2)*2-1)*90), (transform.position.y - floorHeight) / speed);
    }

    // Update is called once per frame
    void Update()
    {
        var nutPosition = transform.position;
        if (nutPosition.y > floorHeight)
        {
            transform.position += new Vector3(0,-speed*Time.deltaTime,0);
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
        uiMan.IncreaseScore(false);
        Destroy(gameObject);
    }
}
