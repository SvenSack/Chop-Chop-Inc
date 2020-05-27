using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerOrganizer : MonoBehaviour
{
    private List<Transform> animals;
    private Transform camTransform;


    private void Awake()
    {
        AnimalSprite[] temp = FindObjectsOfType<AnimalSprite>();
        foreach (var animal in temp)
        {
            animals.Add(animal.transform.parent);
        }

        camTransform = Camera.main.transform;
    }

    // Start is called before the first frame update
    void Start()
    {
        SortObjects();
    }
    
    public void SortNewObject(GameObject obj, bool includeChildren)
    {
        // sort this newly created object
    }

    public void AddAnimal(Transform animal)
    {
        animals.Add(animal);
        SortObjects(animal);
    }

    private void SortObjects()
    {
        // sort all objects into foreground that appear in front of any animal
        MeshRenderer[] allMeshes = FindObjectsOfType<MeshRenderer>();
        foreach (var mesh in allMeshes)
        {
            if (!mesh.gameObject.CompareTag("Default"))
            {
                foreach (var animal in animals)
                {
                    if (Vector3.Distance(animal.position, camTransform.position) >
                        Vector3.Distance(mesh.transform.position, camTransform.position))
                    {
                        // perform magic/raycasts to figure out if the animal is actually in a location reasonably possibly behind
                        // if so, then assign it to the layer "ForeGround" and break
                    }
                }
            }
        }
    }

    private void SortObjects(Transform animal)
    {
        // sort all objects into foreground that appear in front of a specified animal
    }
}
