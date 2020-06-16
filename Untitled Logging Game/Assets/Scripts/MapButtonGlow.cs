using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapButtonGlow : MonoBehaviour
{

    public GameObject glowObject;
    private float glowScale = 1;

    private bool isGrowing;
    // Start is called before the first frame update
    void Start()
    {
        glowObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (glowObject.activeSelf)
        {
            if (isGrowing && glowScale < 1.2f)
            {
                glowScale += Time.deltaTime * .1f;
            }
            else if (isGrowing)
                isGrowing = !isGrowing;

            if (!isGrowing && glowScale > 1)
            {
                glowScale -= Time.deltaTime * .1f;
            }
            else if (!isGrowing)
                isGrowing = !isGrowing;
        
            glowObject.transform.localScale = new Vector3(glowScale,glowScale,glowScale);
        }
        
    }

    public void StartGlow()
    {
        glowObject.SetActive(true);
    }
}
