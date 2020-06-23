using System.Collections;
using System.Collections.Generic;
using Animal;
using Unity.Mathematics;
using Unity.Properties;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class PlantMan : MonoBehaviour
{
    
    [SerializeField] float[] leafScaleValues = new []{1f,1f,1f,1f,1f,1f};
    [SerializeField] Color[] leafColorValues = new Color[8];
    [SerializeField] private GameObject[] regrowableTrees = new GameObject[8];
    [SerializeField] private GameObject squirrelPrefab;
    public Sprite[] leafParticles = new Sprite[8];
    
    public GameObject nutPrefab;
    public Sprite[] nutSprites = new Sprite[8];
    public GameObject treeShakeParticles;
    private int groundMask;
    private float shakeTimer;

    public List<Transform> currentTreeSpots;
    
    [SerializeField] GameObject fallParticleObject;
    [SerializeField] private GameObject leafFallParticleObject;

    [SerializeField] private GameObject plantPopUp;

    private CutMan cutMan;
    private CameraMan camMan;
    private Camera mainCam;
    private UIMan uiMan;
    
    
    void Start()
    {
        cutMan = GetComponent<CutMan>();
        groundMask = LayerMask.GetMask("Ground");
        mainCam = Camera.main;
        camMan = FindObjectOfType<CameraMan>();
        plantPopUp.SetActive(false);
        uiMan = FindObjectOfType<UIMan>();
    }

    void Update()
    {
        if (shakeTimer > 0)
            shakeTimer -= Time.deltaTime;
    }

    public void plantSeed(int treeIndex)
    {
        if (currentTreeSpots.Count == 0 && camMan.currentLocation == 3)
        {
            plantPopUp.SetActive(true);
            cutMan.mayCut = false;
        }
        GameObject newTree = Instantiate(regrowableTrees[treeIndex], currentTreeSpots[0]);
        newTree.transform.localScale = new Vector3(.1f,.1f,.1f);
        newTree.transform.LeanScale(new Vector3(.4f, .4f, .4f), 2f);
        // spawn sound and particles
        currentTreeSpots.RemoveAt(0);
    }

    public void TreeFell(GameObject newTreePiece, CuttableTreeScript target)
    {
        TreeFallParticle temp = newTreePiece.GetComponentInChildren<TreeFallParticle>();
        if (target.leafParticleIndex == 3 || target.leafParticleIndex == 5)
            temp.isPalm = true;
        Transform[] Leaves = newTreePiece.GetComponentsInChildren<Transform>();
        List<ParticleSystem> leafs = new List<ParticleSystem>();
        foreach (var leaf in Leaves)
        {
            if (leaf.gameObject.CompareTag("Leaves"))
            {
                if (Random.Range(0, 4) == 0)
                {
                    GameObject newParter = Instantiate(leafFallParticleObject, leaf);
                    ParticleSystem newPart = newParter.transform.GetChild(0).GetComponent<ParticleSystem>();
                    int newPartIndex = target.leafParticleIndex;
                    newPart.textureSheetAnimation.SetSprite(0,leafParticles[newPartIndex]);
                    var mainModule = newPart.main;
                    mainModule.startColor = leafColorValues[newPartIndex];
                    if (leafScaleValues[newPartIndex] != 1 && !temp.isPalm)
                    {
                        mainModule.startSizeMultiplier = leafScaleValues[newPartIndex];
                    }
                    var newPartShape = newPart.shape;
                    if (!temp.isPalm)
                    {
                        newPartShape.mesh = leaf.GetComponent<MeshFilter>().mesh;
                        newPartShape.scale = newTreePiece.transform.localScale;
                        newPart.Play();
                        mainModule.stopAction = ParticleSystemStopAction.Destroy;
                    }
                    else
                    {
                        newPart.Pause();
                    }
                    leafs.Add(newPart);
                }
            }
        }
        temp.leaves = leafs.ToArray();
        ParticleSystem newSyst = Instantiate(fallParticleObject, newTreePiece.transform)
            .GetComponentInChildren<ParticleSystem>();
        var newSystShape = newSyst.shape;
        if(SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Finland"))
            newSyst.transform.parent.localScale = new Vector3(8,8,8);
        newSystShape.scale = newSystShape.scale * newTreePiece.transform.localScale.x;
        temp.dust = newSyst;
        

        if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Finland") && Random.Range(0,4) == 3)
        {
            Transform[] leaves = newTreePiece.transform.GetComponentsInChildren<Transform>();
            Transform maybeLeaf = leaves[Random.Range(0, leaves.Length)];
            if (maybeLeaf.gameObject.CompareTag("Leaves"))
            {
                GameObject squirrel = Instantiate(squirrelPrefab);
                squirrel.transform.position = maybeLeaf.position;
                Squirrel squirsquirrelrel = squirrel.GetComponentInChildren<Squirrel>();
                Physics.Raycast(squirrel.transform.position, Vector3.down, out var hit, 1000, groundMask);
                Debug.DrawLine(squirrel.transform.position,hit.point,Color.green, 1000);
                squirsquirrelrel.floorHeight = hit.point.y;
                uiMan.TryVoiceLine(7);
            }
        }
    }
    
    public IEnumerator SeedSpawn(Transform tree, GraphicRaycaster gRaycaster)
    {
        if (cutMan.currentTargetIndices.Contains(int.Parse(tree.parent.name)) && shakeTimer <= 0 && currentTreeSpots.Count > 0)
        {
            shakeTimer = 1;
            Transform[] leaves = tree.gameObject.GetComponentsInChildren<Transform>();
            if (leaves.Length != 1)
            {
                // Debug.Log(leaves.Length);
                float currentZ = tree.rotation.z;
                tree.LeanRotateZ(currentZ + 2, .4f);
                yield return new WaitForSeconds(.4f);
                Transform nutLocation = leaves[Random.Range(0, leaves.Length)];
                Vector3 nutPosition = nutLocation.position;
                Transform[] locations = new[]
                {
                    nutLocation, leaves[Random.Range(0, leaves.Length)],
                    leaves[Random.Range(0, leaves.Length)], leaves[Random.Range(0, leaves.Length)],
                    leaves[Random.Range(0, leaves.Length)], leaves[Random.Range(0, leaves.Length)],
                    leaves[Random.Range(0, leaves.Length)]
                };
                int newPartIndex = 0;
                foreach (var loc in locations)
                {
                    if (loc.gameObject.CompareTag("Leaves"))
                    {
                        GameObject tempObj = Instantiate(treeShakeParticles, loc);
                        ParticleSystem tempPart = tempObj.GetComponentInChildren<ParticleSystem>();
                        newPartIndex = tree.GetComponent<CuttableTreeScript>().leafParticleIndex;
                        tempPart.textureSheetAnimation.SetSprite(0, leafParticles[newPartIndex]);
                        var mainModule = tempPart.main;
                        mainModule.startColor = leafColorValues[newPartIndex];
                        if (leafScaleValues[newPartIndex] != 1)
                        {
                            var tempPartMain = tempPart.main;
                            tempPartMain.startSizeMultiplier = leafScaleValues[newPartIndex];
                        }

                        var tempPartShape = tempPart.shape;
                        tempPartShape.mesh = loc.gameObject.GetComponent<MeshFilter>().mesh;
                        tempPart.Play();
                    }
                }

                GameObject newNut = Instantiate(nutPrefab, gRaycaster.transform);
                newNut.transform.position = mainCam.WorldToScreenPoint(nutPosition);
                NutMover newNutMove = newNut.GetComponent<NutMover>();
                newNutMove.treeIndex = newPartIndex;
                newNut.GetComponent<Image>().sprite = nutSprites[newPartIndex];
                Physics.Raycast(nutPosition, Vector3.down, out var hit, 1000, groundMask);
                // Debug.DrawLine(nutPosition,hit.point,Color.green, 1000);
                newNutMove.floorHeight = mainCam.WorldToScreenPoint(hit.point).y;
                tree.LeanRotateZ(currentZ - 4, .3f);
                yield return new WaitForSeconds(.3f);
                tree.LeanRotateZ(currentZ, .2f);
            }
        }
    }
}
