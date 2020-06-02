using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LayerOrganizer : MonoBehaviour
{
    [SerializeField] List<Transform> animals; // a list of all the animals in the scene
    private List<Transform> foregroundObjects; // a list of all the objects that are in the foreground of any animals
    private Dictionary<Transform, Transform> objectByAnimal; // a dictionary of all the objects in the foreground, sorted
                                        // by the animal they are obscuring (so we can re-evaluate them when animals leave)
    private int groundMask = 12; // the layer mask of the ground to raycast points for our occlusion box
    public bool debugGizmos = true; // wether or not to display gizmos when selecting the object (the main camera)
    [SerializeField] private float gizmoSphereSize = 1;


    private void Awake()
    {
        /*AnimalSprite[] temp = FindObjectsOfType<AnimalSprite>();
        foreach (var animal in temp)
        {
            animals.Add(animal.transform.parent);
        }*/ // currently commented because of the gizmo skipping awake and thus needing manual serialization

        groundMask = LayerMask.GetMask("Ground");
    }

    public void AddAnimal(Transform animal)
    {
        // sort this newly created animal, used later on when add animals during runtime, might be smarter to pre-place
        // them beforehand, and do this in editor time aswell, depending on how heavy this gets
        animals.Add(animal);
        SortObjects(animal);
    }

    private void SortObjects()
    {
        List<GameObject> possibleObjects = GrabEligibleObjects();
        foreach (var pObj in possibleObjects)
        {
            Vector3[] occluderPoints = CreateOccluderPoints(pObj.GetComponent<BoxCollider>());
            // check against the created box with a quick sphere and then a slower more accurate cube,
            // then assign it to the layer "ForeGround", and store the transform in foregroundobjects
            // aswell as create the dictionary entry and break
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (debugGizmos)
        {
            List<GameObject> targets = GrabEligibleObjects();
            foreach (var obj in targets)
            {
                Vector3[] occluderPoints = CreateOccluderPoints(obj.GetComponent<BoxCollider>());
                foreach (var point in occluderPoints)
                {
                    // draw spheres on the points
                    Gizmos.color = Color.magenta;
                    Gizmos.DrawSphere(point, gizmoSphereSize);
                }
                for (int i = 0; i < 8; i++)
                {
                    // draw the lines between points to form the box
                    if(i!=7)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawLine(occluderPoints[i], occluderPoints[i + 1]);
                    }
                    if(i < 4)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawLine(occluderPoints[i], occluderPoints[i + 4]);
                    }
                }
            }
            //Debug.Log("Drawing Gizmos for camera layer ordering");
        }
    }

    private void SortObjects(Transform animal)
    {
        // sort all objects into foreground that appear in front of a specified animal, using the same approach as in 
        // sort objects, but only for one instead of all of the animals
    }

    private Vector3[] CreateOccluderPoints(BoxCollider boundingBox)
    {
        // create a box defined by 8 points, 0 being the top left corner of the original boxcollider, 3 being its bottom left corner
        // and 4-7 being the points you get when you project that shape through the camera perspective until you hit the ground or the
        // edge of the camera frustum. This function does not currently work correctly, due to (presumably) the way that box colliders
        // are affected by scaling and rotations on their game object
        Vector3[] points = new Vector3[8];
        Transform bBTransform = boundingBox.transform;
        Bounds col = boundingBox.bounds;
        Vector3 min = col.center - col.extents;
        Vector3 max = col.center + col.extents;
        points[0] = bBTransform.TransformPoint(new Vector3(min.x, max.y, max.z));
        points[1] = bBTransform.TransformPoint(new Vector3(max.x, max.y, max.z));
        points[2] = bBTransform.TransformPoint(new Vector3(max.x, min.y, max.z));
        points[3] = bBTransform.TransformPoint(new Vector3(min.x, min.y, max.z));
        Camera cam = Camera.main;
        for (int i = 0; i < 4; i++)
        {
            Ray ray = cam.ScreenPointToRay(points[i]);
            Debug.DrawLine(transform.position, points[i], Color.red);
            if (Physics.Raycast(ray, out var hit, 100000, groundMask))
            {
                points[i + 4] = hit.point;
            }
            else
            {
                Plane[] cameraPlanes = GeometryUtility.CalculateFrustumPlanes(cam);
                for(int j = 0; j < 6; j++)
                {
                    if(j != 4)
                        if (cameraPlanes[j].Raycast(ray, out float intersectionDistance))
                        {
                            points[i + 4] = ray.GetPoint(intersectionDistance);
                            Debug.Log(j);
                            break;
                        }
                }
            }
        }
        return points;
    }


    private List<GameObject> GrabEligibleObjects()
    {
        // grabs all objects with a mesh, then discards the ones that have a layer (presumably for functionality purposes,
        // aswell as those without a box collider (since they are presumed to not be part of what this is trying to do,
        // aswell as those that are not closer to the camera than any animal, then returns the list
        MeshRenderer[] allMeshes = FindObjectsOfType<MeshRenderer>();
        List<GameObject> eligibleObjects = new List<GameObject>();
        foreach (var mesh in allMeshes)
        {
            if (mesh.gameObject.layer == 0 && mesh.gameObject.TryGetComponent(out BoxCollider box))
                // Bug potential with the way that the cutting placer also uses boxcolliders, can be easily avoided by
                // replacing the collider in that script, or can be reworked into a way to also consider the trees
            {
                // Debug.Log(mesh.name + "was considered due to collider and layer");
                foreach (var animal in animals)
                {
                    if (Vector3.Distance(animal.position, transform.position) >
                        Vector3.Distance(mesh.transform.position, transform.position))
                    {
                        // Debug.Log(mesh.name + "was considered eligible due to proximity");
                        eligibleObjects.Add(mesh.gameObject);
                    }
                }
            }
        }

        return eligibleObjects;
    }
}
