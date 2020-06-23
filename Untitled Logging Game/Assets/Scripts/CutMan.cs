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
    private GameObject currentCut;
    private CutTarget currentTarget;
    private Vector2 currentL;
    private Vector2 currentR;
    private Vector2 cutStart;
    private Vector2 cutUpdate;
    private bool isCutting;

    public bool mayCut = true;
    
    [SerializeField] GraphicRaycaster gRaycaster = null;
    [SerializeField] EventSystem eventSystem = null;
    [SerializeField] float marginOfError = 0.1f;
    private int trunkMask;

    [SerializeField] GameObject cutParticleObject = null;
    private GameObject cutParticleInstance;

    public CuttableTreeScript[] trees;
    public int[] treeHps;
    private CutTarget[] cutTargets;
    [SerializeField] GameObject cutTargetPrefab = null;
    public float cutTargetDistance = 5f;
    [SerializeField] private float cutForce;
    
    private SoundMan soundMan;
    private UIMan uiMan;

    public float forgivingness = 1f;
    private bool cutFailing;
    
    public bool chainsawOn;
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
    private PlantMan plantMan;

    [SerializeField] private GameObject burnPopUp;


    private void Awake()
    {
        soundMan = FindObjectOfType<SoundMan>();
        uiMan = FindObjectOfType<UIMan>();
        trunkMask = LayerMask.GetMask("Trunks");
        //Debug.Log("trunkMask is at " + trunkMask);
        if(!multiCam)
            trees = FindObjectsOfType<CuttableTreeScript>();
        treeHps = new int[trees.Length];
        cutTargets = new CutTarget[trees.Length];
        for (int i = 0; i < treeHps.Length; i++)
        {
            treeHps[i] = Random.Range(1, 4);
        }

        mainCam = Camera.main;
    }

    private void Start()
    {
        if (multiCam)
            camMan = FindObjectOfType<CameraMan>();

        trailMan = GameObject.FindGameObjectWithTag("TrailMan");
        plantMan = GetComponent<PlantMan>();
        burnPopUp.SetActive(false);
        HighScoreManager hsM = FindObjectOfType<HighScoreManager>();
        cutDifficulty = hsM.GetLevelDifficulty(hsM.currentLevel -1 > 0 ? hsM.currentLevel - 1 : 0);
    }

    // Update is called once per frame
    void Update()
    {

        if (debugMode && Input.GetKeyDown(KeyCode.Return)) // debug cam mover
        {
            MoveArea();
        }
        
        
        if (Input.GetMouseButtonDown(0) && mayCut) // try initiating cut
        {
            
            if(cutRoutine == null)
                cutRoutine = StartCoroutine(InitiateCut());
            else
            {
                StopCoroutine(cutRoutine);
                cutRoutine = StartCoroutine(InitiateCut());
            }
        }

        if (chainsawOn) // pre-update for when the chainsaw is running, basically mostly checking to make sure target is correct
        {
            
            if (!cutFailing)
            {
                StartCoroutine("InitiateStopCut");
            }
            
            if (Input.GetMouseButtonUp(0))
            {
                StopCut();
                StopCoroutine("InitiateStopCut");
                cutFailing = false;
            }
            else if(Input.GetMouseButton(0))
            {
                SelectTarget();
            }
        }
        
        if (chainsawOn) // main update loop for chainsaw
        {

            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out var hit, 100000, trunkMask) && currentCut != null && hit.collider.gameObject.GetComponent<CuttableTreeScript>() ==
                currentTarget.target) // check if you are hitting wood
            {
                bool isCutStartSet = false;
                // Debug.DrawRay (ray.origin, ray.direction * 50000000, Color.green, 100f);
                if (!isCutting && CheckCutSpot(Input.mousePosition)) // start hitting wood
                {
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
                    cutStart = Input.mousePosition;
                    isCutStartSet = true;

                }
                if (isCutting)
                {
                    if ((Vector2.Distance(currentL, Input.mousePosition) > Vector2.Distance(currentL, currentR) &&
                         !currentTarget.goesLeft)
                        || (Vector2.Distance(currentR, Input.mousePosition) >
                            Vector2.Distance(currentL, currentR) && currentTarget.goesLeft))
                    {
                        MakeCut();
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
            else // make sure that you cut irregular mesh colliders
            {
                if (isCutting)
                {
                    float dist = Vector2.Distance(cutStart, Input.mousePosition);
                    if (Mathf.Abs(dist) > marginOfError && CheckCutSpotCut(cutStart, Input.mousePosition))
                    {
                        MakeCut();
                    }
                }
            }
        }
        
        PlaceCutSpots();
        
        
        trailMan.transform.position = GetMouseWorld();
    }

    private GameObject PerformCut(Vector2 start, Vector2 finish) // performs the cutting and returns the new tree part
    {
        CuttableTreeScript target = currentTarget.target;
        Ray ray = mainCam.ScreenPointToRay(start);
        Physics.Raycast(ray, out var hit, trunkMask);
        Vector3 startPoint = hit.point;
        
        Ray ray2 = mainCam.ScreenPointToRay(finish);
        Physics.Raycast(ray2, out hit, trunkMask);
        Vector3 finishPoint = hit.point;
        
        Vector3 targetLocation = Vector3.Lerp(startPoint, finishPoint, 0.5f);
        Vector3 cutLine = startPoint - finishPoint;
        Vector3 cutRight = Vector3.Cross(cutLine, Vector3.up);
        Vector3 cutNormal = Vector3.Cross(cutRight, cutLine);
        
        Debug.DrawLine(new Vector3(start.x, start.y , 100), new Vector3(start.x, start.y , -100), Color.magenta, 1000f);
        Debug.DrawLine(new Vector3(finish.x, finish.y , 100), new Vector3(finish.x, finish.y , -100), Color.green, 1000f);

        
        GameObject newTree = target.CutAt(targetLocation, cutNormal, cutForce);
        cutFailing = false;
        return newTree;
    }

    private Vector3 GetMouseWorld() // returns the mouse in world space
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

    private bool CheckCutSpotCut(Vector2 pos1, Vector2 pos2) // helper function that checks if both points of a cut are on target
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
    
    private bool CheckCutSpot(Vector2 pos) // helper function that checks the mouse position against the cut target
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

    private bool StartCut(GameObject target) // the start of the tree-contact cut, returns true if on target
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
                    chainsawOn = true;
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
                    chainsawOn = true;
                    Debug.DrawLine(currentL, Input.mousePosition, Color.cyan,1000);
                    return true;
                }
                Debug.Log("Discarded right with width of " + width + " and distance of " + dist1);
                break;
        }

        return false;
    }
    
    IEnumerator InitiateStopCut() // tries to stop cut if not prevented by cutting a tree
    {
        // Debug.Log("query for stop");
        cutFailing = true;
        yield return new WaitForSeconds(forgivingness);
        if (cutFailing)
        {
            // Debug.Log("query assessed true");
            StopCut();
        }
        else
        {
            // Debug.Log("query assessed false");
        }

        cutFailing = false;
    }
    
    private void StopCut() // actually stops the cut
    {
        if (soundMan.chainsawSoundObject != null)
        {
            soundMan.StopCut();
        }
        if(cutParticleInstance != null)
            foreach (var part in cutParticleInstance.GetComponentsInChildren<ParticleSystem>())
            {
                part.Stop(false, ParticleSystemStopBehavior.StopEmitting);
            }
        currentCut = null;
        isCutting = false;
        chainsawOn = false;
        comboText.text = comboText.text.Replace((comboCount) + "x", 0 + "x");
        if (comboCount < 3)
        {
            cutDifficulty -= .05f;
            maxRot = cutDifficulty*20f;
            forgivingness = 5-cutDifficulty*2;
        }
        comboCount = 0;
        comboText.fontSize = 0;
        trailMan.GetComponent<TrailRenderer>().emitting = false;
    }

    private void UpdateScoreCombo() // the score updater
    {
        comboCount++;
        if (comboText.fontSize == 0 && comboCount > 1)
            comboText.fontSize = 36;
        string temp = comboText.text;
        temp = temp.Replace((comboCount - 1) + "x", comboCount + "x");
        comboText.text = temp;
        if (comboCount > 3)
        {
            uiMan.TryVoiceLine(2);
            cutDifficulty += .05f;
            maxRot = cutDifficulty*20f;
            forgivingness = 5-cutDifficulty*2;
        }
    }

    IEnumerator InitiateCut() // tries to initiate cut, or tree-shake, the default click behavior
    {
        yield return new WaitForSeconds(cutDelay);
        if (Input.GetMouseButton(0))
        {
            StopCoroutine("InitiateStopCut");
            cutFailing = false;
            soundMan.StartCut();
            soundMan.chainsawSoundObject.transform.position = GetMouseWorld();
            chainsawOn = true;
            trailMan.GetComponent<TrailRenderer>().emitting = true;
        }
        else
        {
            RaycastHit hit;
            Ray ray = mainCam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit,Mathf.Infinity, trunkMask))
            {
                if (hit.collider.gameObject.GetComponent<CuttableTreeScript>().isFirstTree)
                {
                    StartCoroutine(plantMan.SeedSpawn(hit.collider.transform, gRaycaster));
                }
            }
        }
    }

    private void MakeCut()
    {
        GameObject newTreePiece = PerformCut(cutStart, cutUpdate);
        CuttableTreeScript target = currentTarget.target;
        plantMan.TreeFell(newTreePiece, target);
        newTreePiece.GetComponent<TreeFallParticle>().fallSound = soundMan.TreeFall(newTreePiece);
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
                            if (camMan.currentLocation == 3)
                            {
                                burnPopUp.SetActive(true);
                                uiMan.TryVoiceLine(Random.Range(4,6), 10f);
                                mayCut = false;
                            }
                            camMan.MoveOn();
                            currentTargetIndices.Clear();
                        }
                    }
                }
            }
        }

        Destroy(currentCut);
        UpdateScoreCombo();
        StartCoroutine("InitiateStopCut");
        isCutting = false;
        foreach (var part in cutParticleInstance.GetComponentsInChildren<ParticleSystem>())
        {
            part.Stop(false, ParticleSystemStopBehavior.StopEmitting);
        }
    }

    private void SelectTarget()
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
                    GameObject targO = targ.transform.parent.gameObject;
                    if (currentCut == null || ( targO != currentCut && StartCut(targO)))
                    {
                        if (currentCut == null)
                        {
                            currentCut = targO;
                            currentTarget = targ;
                            StartCut(targO);
                        }
                        else
                        {
                            currentCut = targO;
                            currentTarget = targ;
                        }
                    }
                    hit = true;
                }
            }
        }
    }
    
    private void PlaceCutSpots() // places the cut locations
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
                cutTargets[currentIndex].goesLeft = !cutTargets[previousIndex].goesLeft;
            }

            Transform tempTrans = cutTargets[currentIndex].transform.parent;


            float offSet = Random.Range(maxRot+5, -(maxRot+5));
            tempTrans.rotation = Quaternion.Euler(0, 0, offSet);

            if (!cutTargets[currentIndex].goesLeft)
            {
                tempTrans.GetChild(0).GetComponent<RectTransform>().rotation = Quaternion.Euler(0, 0, 180);
            }
        }

    }

    public void MoveArea()
    {
        foreach (var tar in 
            FindObjectsOfType<CutTarget>())
        {
            Destroy(tar.transform.parent.gameObject);
        }

        camMan.MoveOn();
        currentTargetIndices.Clear();
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