using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class CutMan : MonoBehaviour
{
    public GameObject currentCut;
    private CutTarget currentTarget;
    private Vector2 currentL;
    private Vector2 currentR;
    private Vector2 cutStart;
    private Vector2 cutUpdate;
    private bool isCutting;
    
    [SerializeField] GraphicRaycaster gRaycaster = null;
    [SerializeField] EventSystem eventSystem = null;
    [SerializeField] float marginOfError = 0.1f;
    private int trunkMask;

    [SerializeField] GameObject cutParticleObject = null;
    [SerializeField] GameObject fallParticleObject = null;
    private GameObject cutParticleInstance;

    public CuttableTreeScript[] trees;
    public int[] treeHps;
    private CutTarget[] cutTargets;
    [SerializeField] GameObject cutTargetPrefab = null;
    public float cutTargetDistance = 5f;
    [SerializeField] private float cutForce;
    
    private SoundMan soundMan;
    private UIMan uiMan;

    [SerializeField] float[] leafScaleValues = new []{1f,1f,1f,1f,1f,1f};
    [SerializeField] Color[] leafColorValues = new Color[8];
    public Sprite[] leafParticles;

    public GameObject nutPrefab;
    public Sprite[] nutSprites;
    public GameObject treeShakeParticles;
    private int groundMask;
    private float shakeTimer;

    public float forgivingness = 1f;
    private IEnumerator cutStopper;
    private bool cutFailing;
    
    public bool isInCombo;
    public int comboCount = 0;
    [SerializeField] private TextMeshProUGUI comboText;
    
    [Range(0,1.0f)]public float cutDifficulty;
    [Range(0, 40.0f)] public float maxRot;

    private Camera mainCam;
    
    [SerializeField] private bool multiCam;
    private CameraMan camMan;
    public List<int> currentTargetIndices = new List<int>();

    public bool debugMode;

    public float cutDelay = .5f;
    private Coroutine cutRoutine;

    private GameObject trailMan;

    private void Awake()
    {
        soundMan = FindObjectOfType<SoundMan>();
        uiMan = FindObjectOfType<UIMan>();
        trunkMask = LayerMask.GetMask("Trunks");
        groundMask = LayerMask.GetMask("Ground");
        if(!multiCam)
            trees = FindObjectsOfType<CuttableTreeScript>();
        treeHps = new int[trees.Length];
        cutTargets = new CutTarget[trees.Length];
        for (int i = 0; i < treeHps.Length; i++)
        {
            treeHps[i] = Random.Range(1, 4);
        }

        mainCam = Camera.main;
        cutStopper = InitiateStopCut();
    }

    private void Start()
    {
        if (multiCam)
            camMan = FindObjectOfType<CameraMan>();

        trailMan = GameObject.FindGameObjectWithTag("TrailMan");
    }

    // Update is called once per frame
    void Update()
    {
        //Debug.Log("Input.mousePosition" + Input.mousePosition.ToString("F2"));

        if (debugMode && Input.GetKeyDown(KeyCode.Return))
        {
            foreach (var tar in 
                FindObjectsOfType<CutTarget>())
            {
                Destroy(tar.transform.parent.gameObject);
            }

            camMan.MoveOn();
            currentTargetIndices.Clear();
        }
        
        
        if (Input.GetMouseButtonDown(0))
        {
            if(cutRoutine == null)
                cutRoutine = StartCoroutine(Cut());
            else
            {
                StopCoroutine(cutRoutine);
                cutRoutine = StartCoroutine(Cut());
            }
        }

        if (isInCombo)
        {
            if (Input.GetMouseButtonUp(0))
            {
                Debug.Log("Stopped cutting because click lift");
                StopCut();
                StopCoroutine(cutStopper);
            }
            else if(Input.GetMouseButton(0))
            {
                List<RaycastResult> castHits = new List<RaycastResult>();
                PointerEventData eventPoint = new PointerEventData(eventSystem) {position = Input.mousePosition};
                gRaycaster.Raycast(eventPoint, castHits);
                bool hit = false;
                if (castHits.Count > 0)
                {
                    foreach (var t in castHits)
                    {
                        CutTarget targ = t.gameObject.GetComponentInChildren<CutTarget>();
                        if (targ != null)
                        {
                            // Debug.Log("Cut continues");
                            GameObject targO = targ.transform.parent.gameObject;
                            if (currentCut == null || ( targO != currentCut && StartCut(targO)))
                            {
                                if (currentCut == null)
                                {
                                    currentCut = targO;
                                    currentTarget = targ;
                                    cutFailing = false;
                                    StartCut(targO);
                                }
                                else
                                {
                                    currentCut = targO;
                                    currentTarget = targ;
                                    cutFailing = false;
                                }
                            }
                            hit = true;
                        }
                    }
                }
                if (!hit && !cutFailing)
                {
  
                    StartCoroutine(cutStopper);
                }
            
            }
        }
        
        //isStopCutNotCalled
        if (isInCombo)
        {

            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100000, trunkMask) && currentCut != null && hit.collider.gameObject.GetComponent<CuttableTreeScript>() ==
                currentTarget.target)
            {
                bool isCutStartSet = false;
                // Debug.DrawRay (ray.origin, ray.direction * 50000000, Color.green, 100f);
                if (!isCutting && CheckCutSpot(Input.mousePosition))
                {
                    Debug.Log("hit tree");
                    isCutting = true;
                    soundMan.ToggleWood();
                    cutParticleInstance = Instantiate(cutParticleObject);
                    if(currentTarget.goesLeft)
                        cutParticleInstance.transform.rotation = Quaternion.Euler(0,0,180);
                    foreach (var part in cutParticleInstance.GetComponentsInChildren<ParticleSystem>())
                    {
                        part.Play();
                    }
                    cutParticleInstance.transform.position = hit.point;
                    soundMan.chainsawSoundObject.transform.position = hit.point;
                    Debug.Log("cutStart set at time " + Time.frameCount);
                    Debug.Log("cutStart set at value " + Input.mousePosition);
                    cutStart = Input.mousePosition;
                    isCutStartSet = true;

                }

                if (isCutting)
                {
                    //Debug.Log("IsCuttingCombo");
                    //Debug.Log("currentL " + currentL.ToString("F2"));
                    //Debug.Log("currentR " + currentR.ToString("F2"));

                    //Debug.Log("Input.mousePosition" + Input.mousePosition.ToString("F2"));
         

                    if ((Vector2.Distance(currentL, Input.mousePosition) > Vector2.Distance(currentL, currentR) &&
                         !currentTarget.goesLeft)
                        || (Vector2.Distance(currentR, Input.mousePosition) >
                            Vector2.Distance(currentL, currentR) && currentTarget.goesLeft))
                    {
  
                        GameObject newTreePiece = InitiateCut(cutStart, cutUpdate);
                        CuttableTreeScript target = currentTarget.target;
                        FellTree(newTreePiece,
                            target); // this does the visual and auditory stuff for the tree falling
                        for (int i = 0; i < trees.Length; i++)
                        {
                            if (trees[i] == target)
                            {
                                treeHps[i]--;
                                if (treeHps[i] == 0)
                                {
                                    uiMan.IncreaseScore(true);
                                    if (multiCam)
                                    {
                                        bool checker = false;
                                        foreach (var index in currentTargetIndices)
                                        {
                                            if (treeHps[index] != 0)
                                                checker = true;
                                        }

                                        if (!checker)
                                        {
                                            camMan.MoveOn();
                                            currentTargetIndices.Clear();
                                        }
                                    }
                                }
                            }
                        }

                        Destroy(currentCut);
                        ComboCut();
                        StartCoroutine(cutStopper);
                        isCutting = false;
                        foreach (var part in cutParticleInstance.GetComponents<ParticleSystem>())
                        {
                            part.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                        }
                    }
                    else
                    {


                        if(!isCutStartSet)
                        {
                            cutUpdate = Input.mousePosition;
                        }
                        
                        cutParticleInstance.transform.position = hit.point;
                        soundMan.chainsawSoundObject.transform.position = hit.point;
                    }
                }
            }
            else //isStopCutNotCalled
            {
                // Debug.DrawRay (ray.origin, ray.direction * 50000000, Color.red, 100f);
                if (isCutting)
                {
                    Debug.Log("IsCutting NonCombo");
                    float dist = Vector2.Distance(cutStart, Input.mousePosition);
                    // Debug.Log("stopped hitting tree at distance " + dist);
                    if (Mathf.Abs(dist) > marginOfError && CheckCutSpotCut(cutStart, Input.mousePosition))
                    {
                        Debug.Log("IsCutting NonCombo Suceeded");

                        Debug.Log("cutStart " + cutStart.ToString("F2"));
                        Debug.Log("cutEnd " + cutUpdate.ToString("F2"));
                        GameObject newTreePiece = InitiateCut(cutStart, cutUpdate);
                        CuttableTreeScript target = currentTarget.target;
                        FellTree(newTreePiece, target); // this does the visual and auditory stuff for the tree falling
                        for (int i = 0; i < trees.Length; i++)
                        {
                            if (trees[i] == target)
                            {
                                treeHps[i]--;
                                if(treeHps[i] == 0)
                                {
                                    uiMan.IncreaseScore(true);
                                    if (multiCam)
                                    {
                                        bool checker = false;
                                        foreach (var index in currentTargetIndices)
                                        {
                                            if (treeHps[index] != 0)
                                                checker = true;
                                        }

                                        if (!checker)
                                        {
                                            camMan.MoveOn();
                                            currentTargetIndices.Clear();
                                        }
                                    }
                                }
                            }
                        }
                        Destroy(currentCut);
                        ComboCut();
                        StartCoroutine(cutStopper);
                        isCutting = false;
                        foreach (var part in cutParticleInstance.GetComponents<ParticleSystem>())
                        {
                            part.Stop(false, ParticleSystemStopBehavior.StopEmitting);
                        }
                    }
                }
            }
        }
        
        PlaceCutSpots();
        if (shakeTimer > 0)
            shakeTimer -= Time.deltaTime;
        
        
        trailMan.transform.position = GetMouseWorld();
    }

    private GameObject InitiateCut(Vector2 start, Vector2 finish)
    {
        CuttableTreeScript target = currentTarget.target;
        Ray ray = mainCam.ScreenPointToRay(start);
        Physics.Raycast(ray, out var hit, trunkMask);
        Vector3 startPoint = hit.point;
        
        Ray ray2 = mainCam.ScreenPointToRay(finish);
        Physics.Raycast(ray2, out hit, trunkMask);
        Vector3 finishPoint = hit.point;
        
        Vector3 targetLocation = Vector3.Lerp(startPoint, finishPoint, 0.5f);
        Debug.Log("targetLocation " + targetLocation.ToString("F2"));
        Vector3 cutLine = startPoint - finishPoint;
        Vector3 cutRight = Vector3.Cross(cutLine, Vector3.up);
        Vector3 cutNormal = Vector3.Cross(cutRight, cutLine);
        
        Debug.DrawLine(new Vector3(start.x, start.y , 100), new Vector3(start.x, start.y , -100), Color.magenta, 1000f);
        Debug.DrawLine(new Vector3(finish.x, finish.y , 100), new Vector3(finish.x, finish.y , -100), Color.green, 1000f);

        Debug.Log("cutNormal " + cutNormal.ToString("F2"));
        Debug.Log("cutForce " + cutForce);
        GameObject newTree = target.CutAt(targetLocation, cutNormal, cutForce);
        return newTree;
    }

    private Vector3 GetMouseWorld()
    {
        Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
        float dist;
        if(currentCut != null)
            dist = Vector3.Distance(mainCam.transform.position, currentTarget.target.transform.position);
        else
        {
            dist = 5f;
        }
        // Debug.DrawRay(ray.origin, ray.direction * dist, Color.yellow, 10f);
        return ray.GetPoint(dist);
    }

    private bool CheckCutSpotCut(Vector2 pos1, Vector2 pos2)
    {
        List<RaycastResult> castHits1 = new List<RaycastResult>();
        List<RaycastResult> castHits2 = new List<RaycastResult>();
        PointerEventData eventPoint1 = new PointerEventData(eventSystem) {position = pos1};
        PointerEventData eventPoint2 = new PointerEventData(eventSystem) {position = pos2};
        gRaycaster.Raycast(eventPoint1, castHits1);
        gRaycaster.Raycast(eventPoint2, castHits2);
        List<GameObject> targ1 = new List<GameObject>();
        List<GameObject> targ = new List<GameObject>();
        foreach (var hit in castHits1)
        {
            targ1.Add(hit.gameObject);
        }

        foreach (var check in castHits2)
        {
            if (targ1.Contains(check.gameObject))
            {
                targ.Add(check.gameObject);
            }
        }
        return targ.Contains(currentCut);
    }
    
    private bool CheckCutSpot(Vector2 pos)
    {
        List<RaycastResult> castHits = new List<RaycastResult>();
        PointerEventData eventPoint = new PointerEventData(eventSystem) {position = pos};
        gRaycaster.Raycast(eventPoint, castHits);
        List<GameObject> targ = new List<GameObject>();
        foreach (var hit in castHits)
        {
            targ.Add(hit.gameObject);
        }
        return targ.Contains(currentCut);
    }

    private void PlaceCutSpots()
    {
        List<CutSpriteInfo> cutSpritesToModify = new List<CutSpriteInfo>();

        bool defaultCutDirection = Random.Range(0, 2) == 0;
        for (int i = 0; i < cutTargets.Length; i++)
        {
            if (cutTargets[i] == null && treeHps[i] > 0)
            {
                if ((Vector3.Distance(trees[i].transform.position, mainCam.transform.position) < cutTargetDistance
                    && !multiCam ) || currentTargetIndices.Contains(i))
                {
                    cutTargets[i] = Instantiate(cutTargetPrefab, gRaycaster.transform).GetComponentInChildren<CutTarget>();
                    cutTargets[i].target = trees[i];
                    cutTargets[i].goesLeft = defaultCutDirection;
                    cutTargets[i].setLifeTime = forgivingness * 3;
                    Transform tempTrans = cutTargets[i].transform.parent;
                    BoxCollider[] boxes = trees[i].transform.parent.GetComponentsInChildren<BoxCollider>();
                    Vector3 targetPosition = Vector3.zero;
                    switch (treeHps[i])
                    {
                        case 3:
                            Vector3 roof = boxes[2].transform.position + new Vector3(0, .5f * boxes[2].transform.localScale.y, 0);
                            Vector3 floor = boxes[2].transform.position - new Vector3(0, .5f * boxes[2].transform.localScale.y, 0);
                            targetPosition = boxes[2].transform.position;
                            // Debug.DrawLine(roof,floor,Color.red,100f);
                            targetPosition.y = Random.Range(roof.y, floor.y);
                            break;
                        case 2:
                            Vector3 roof1 = boxes[1].transform.position + new Vector3(0, .5f * boxes[1].transform.localScale.y, 0);
                            Vector3 floor1 = boxes[1].transform.position - new Vector3(0, .5f * boxes[1].transform.localScale.y, 0);
                            // Debug.DrawLine(roof1,floor1,Color.red,100f);
                            targetPosition = boxes[1].transform.position;
                                                targetPosition.y = Random.Range(roof1.y, floor1.y);
                            break;
                        case 1:
                            Vector3 roof2 = boxes[0].transform.position + new Vector3(0, .5f * boxes[0].transform.localScale.y, 0);
                            Vector3 floor2 = boxes[0].transform.position - new Vector3(0, .5f * boxes[0].transform.localScale.y, 0);
                            targetPosition = boxes[0].transform.position;
                            // Debug.DrawLine(roof2,floor2,Color.red,100f);
                            targetPosition.y = Random.Range(roof2.y, floor2.y);
                            break;
                    }
                    tempTrans.position = mainCam.WorldToScreenPoint(targetPosition);
                    CutSpriteInfo cutSpriteInfo = new CutSpriteInfo(i, targetPosition.x);
                    cutSpritesToModify.Add(cutSpriteInfo);
                    //float offSet = Random.Range(-20f, 20f);
                    
                }
            }
        }

        CutSpriteInfoComparer comparer = new CutSpriteInfoComparer();
        cutSpritesToModify.Sort(comparer);

        for (int i = 1; i < cutSpritesToModify.Count; i++)
        {
            int currentIndex = cutSpritesToModify[i].indexInArray;
            int previousIndex = cutSpritesToModify[i - 1].indexInArray;

            // Debug.Log("iterating on object " + cutTargets[currentIndex].target.transform.parent.name);

            bool doDifficultySwitch = Random.Range(0, 1.0f) < cutDifficulty ? true : false;

            if (doDifficultySwitch)
            {
                // Debug.Log("DifficultySwitch");
                cutTargets[currentIndex].goesLeft = !cutTargets[previousIndex].goesLeft;
            }

            Transform tempTrans = cutTargets[currentIndex].transform.parent;


            float offSet = Random.Range(maxRot, -maxRot);
            tempTrans.rotation = Quaternion.Euler(0, 0, offSet);

            if (!cutTargets[currentIndex].goesLeft)
            {
                tempTrans.GetChild(0).GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 180);
            }
        }

    }

    private bool StartCut(GameObject target)
    {

        RectTransform rec = target.GetComponent<RectTransform>();
        Vector3[] corners = new Vector3[4];
        rec.GetWorldCorners(corners);
        currentR = Vector3.Lerp(corners[2], corners[3], 0.5f);
        currentL = Vector3.Lerp(corners[0], corners[1],0.5f);
        float width = Vector3.Distance(currentR,currentL);
        switch (target.GetComponentInChildren<CutTarget>().goesLeft)
        { 
            case true:
                float dist = Vector2.Distance(currentR, Input.mousePosition);
                if (dist < width / 3)
                {
                    soundMan.StartCut();
                    currentCut = target;
                    soundMan.chainsawSoundObject.transform.position = GetMouseWorld();
                    isInCombo = true;
                    Debug.DrawLine(currentR, Input.mousePosition,Color.cyan,1000);
                    return true;
                }
                Debug.Log("Discarded left with width of " + width + " and distance of " + dist);
                break;
            case false:
                float dist1 = Vector2.Distance(currentL, Input.mousePosition);
                if (dist1 < width / 3)
                {
                    soundMan.StartCut();
                    currentCut = target;
                    soundMan.chainsawSoundObject.transform.position = GetMouseWorld();
                    isInCombo = true;
                    Debug.DrawLine(currentL, Input.mousePosition, Color.cyan,1000);
                    return true;
                }
                Debug.Log("Discarded right with width of " + width + " and distance of " + dist1);
                break;
        }

        return false;
    }

    private void StopCut()
    {
        soundMan.StopCut();
        if(cutParticleInstance != null)
            foreach (var part in cutParticleInstance.GetComponentsInChildren<ParticleSystem>())
            {
                part.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }
        currentCut = null;
        isCutting = false;
        isInCombo = false;
        comboCount = 0;
        comboText.fontSize = 0;
        trailMan.GetComponent<TrailRenderer>().emitting = false;
    }

    IEnumerator InitiateStopCut()
    {
        cutFailing = true;
        yield return new WaitForSeconds(forgivingness);
        if (cutFailing)
        {
            
            StopCut();
        }
    }

    private void FellTree(GameObject newTreePiece, CuttableTreeScript target)
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
        newTreePiece.GetComponent<TreeFallParticle>().fallSound = soundMan.TreeFall(newTreePiece);
    }

    private void ComboCut()
    {
        comboCount++;
        if (comboText.fontSize == 0 && comboCount > 1)
            comboText.fontSize = 36;
        string temp = comboText.text;
        temp = temp.Replace((comboCount - 1) + "x", comboCount + "x");
        comboText.text = temp;
        if (comboCount > 3)
        {
            cutDifficulty += .05f;
            maxRot += +1;
            forgivingness -= .1f;
        }
    }

    IEnumerator SeedSpawn(Transform tree)
    {
        if (currentTargetIndices.Contains(int.Parse(tree.parent.name)))
        { 
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

    IEnumerator Cut()
    {
        yield return new WaitForSeconds(cutDelay);
        if (Input.GetMouseButton(0))
        {
            StopCoroutine(cutStopper);
            soundMan.StartCut();
            soundMan.chainsawSoundObject.transform.position = GetMouseWorld();
            isInCombo = true;
            trailMan.GetComponent<TrailRenderer>().emitting = true;
        }
        else
        {
            RaycastHit hit;
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit,Mathf.Infinity, trunkMask) && shakeTimer <= 0)
            {
                shakeTimer = 1;
                if (hit.collider.gameObject.GetComponent<CuttableTreeScript>().isFirstTree)
                {
                    StartCoroutine(SeedSpawn(hit.collider.transform));
                }
            }
        }
    }
}





public class CutSpriteInfo
{
    public int indexInArray;
    public float locationInScreen;

    public CutSpriteInfo(int indexInArray,float locationInScreen)
    {
        this.indexInArray = indexInArray;
        this.locationInScreen = locationInScreen;
    }

}

public class CutSpriteInfoComparer : IComparer<CutSpriteInfo>
{
    public int Compare(CutSpriteInfo a,CutSpriteInfo b)
    {
        return b.locationInScreen.CompareTo(a.locationInScreen);
    }
}