
using System;
using UnityEngine;
using UnityEngine.UI;

public class CutTarget : MonoBehaviour
{

    public CuttableTreeScript target;

    public bool goesLeft;
    private float lifeTime;
    public float setLifeTime;
    private Image img;

    private void Start()
    {
        img = GetComponent<Image>();
        lifeTime = setLifeTime;
    }

    void Update()
    {
        if(Input.GetKeyDown("f"))
            lifeTime = Single.PositiveInfinity;
        
        if (lifeTime > 0)
        {
            lifeTime -= Time.deltaTime;
            Color newCol = img.color;
            newCol.a = (.5f * lifeTime) / setLifeTime;
            img.color = newCol;
        }
        else
            Destroy(gameObject.transform.parent.gameObject);
    }
}
