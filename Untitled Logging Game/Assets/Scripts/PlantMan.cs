using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlantMan : MonoBehaviour
{
    
    [SerializeField] float[] leafScaleValues = new []{1f,1f,1f,1f,1f,1f};
    [SerializeField] Color[] leafColorValues = new Color[8];
    public Sprite[] leafParticles;
    
    public GameObject nutPrefab;
    public Sprite[] nutSprites;
    public GameObject treeShakeParticles;
    private int groundMask;
    private float shakeTimer;
    
    
    [SerializeField] GameObject fallParticleObject = null;

    private CutMan cutMan;
    private Camera mainCam;
    
    
    // Start is called before the first frame update
    void Start()
    {
        cutMan = GetComponent<CutMan>();
        groundMask = LayerMask.GetMask("Ground");
        mainCam = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (shakeTimer > 0)
            shakeTimer -= Time.deltaTime;
    }

    public void TreeFell(GameObject newTreePiece, CuttableTreeScript target)
    {
        GameObject newPartI = Instantiate(fallParticleObject, newTreePiece.transform);
        ParticleSystem newPart = newPartI.transform.GetChild(0).GetComponent<ParticleSystem>();
        int newPartIndex = target.leafParticleIndex;
        newPart.textureSheetAnimation.SetSprite(0,leafParticles[newPartIndex]);
        var mainModule = newPart.main;
        mainModule.startColor = leafColorValues[newPartIndex];
        if (leafScaleValues[newPartIndex] != 1)
        {
            var newPartMain = newPart.main;
            newPartMain.startSizeMultiplier = leafScaleValues[newPartIndex];
        }
        var newPartShape = newPart.shape;
        newPartShape.mesh = newTreePiece.GetComponent<MeshFilter>().mesh;
        newPart.Play();
    }
    
    public IEnumerator SeedSpawn(Transform tree, GraphicRaycaster gRaycaster)
    {
        if (cutMan.currentTargetIndices.Contains(int.Parse(tree.parent.name)) && shakeTimer <= 0)
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
