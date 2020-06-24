using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TagBasedObjectDestroyer : MonoBehaviour
{
    public string tag = "foliage";

    private GameObject[] gameObjectsWithTag;

    // Start is called before the first frame update
    void Start()
    {
        gameObjectsWithTag = GameObject.FindGameObjectsWithTag(tag);
    }

    public void DestroyObjectsWithSetTag()
    {
        foreach(GameObject taggedObject in gameObjectsWithTag)
        {
            Destroy(taggedObject);
        }
    }
}
