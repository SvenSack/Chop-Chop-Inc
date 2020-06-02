﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using System.Linq;

public class SortingGroupComparer : IComparer<ObjectToScreenSpaceZ>
{
    public int Compare(ObjectToScreenSpaceZ A, ObjectToScreenSpaceZ B)
    {
        return B.ScreenSpaceZ.CompareTo(A.ScreenSpaceZ);
    }
}

public class ObjectToScreenSpaceZ
{
    public ObjectToScreenSpaceZ(SortingGroup sortingGroup,float ScreenToSpaceZ)
    {
        this.sortingGroup = sortingGroup;
        this.ScreenSpaceZ = ScreenToSpaceZ;
    }

    public SortingGroup sortingGroup;
    public float ScreenSpaceZ;
}


public class ObjectRenderOrderer : MonoBehaviour
{
    public List<ObjectToScreenSpaceZ> objectsToBeSorted = new List<ObjectToScreenSpaceZ>();

    // Start is called before the first frame update
    void Start()
    {
        SortingGroup[] sortingGroups = FindObjectsOfType<SortingGroup>();

        foreach (var sortingGroup in sortingGroups)
        {
            objectsToBeSorted.Add(new ObjectToScreenSpaceZ(sortingGroup,0.0f));
        }
    }

    private void Update()
    {
        SortObjectRenderOrder();
    }

    public void SortObjectRenderOrder()
    {
        //set sorting order of all objects based on z screen space values
        foreach(ObjectToScreenSpaceZ obj in objectsToBeSorted)
        {
            float z = Camera.main.WorldToScreenPoint(obj.sortingGroup.transform.position).z;
            obj.ScreenSpaceZ = z;

        }
        //sort objects based on screen space z
        objectsToBeSorted.Sort(new SortingGroupComparer());

        //set sorting order based on index of object in list
        for (int i = 0; i < objectsToBeSorted.Count; i++)
        {
            objectsToBeSorted[i].sortingGroup.sortingOrder = i;
        }

    }
}
