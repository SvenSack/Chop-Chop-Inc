﻿using System;
using System.Collections.Generic;
using System.Collections;
using TMPro;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMan : MonoBehaviour,IObservable
{

    public int cutTrees;
    public int plantedTrees;
    private int cutTreesThisLvl;
    private int plantedTreesThisLvl;
    private int highestCutCombo;
    private int highestPlantCombo;
    private float score;
    
    public string mapScene;
    private CutMan cutMan;

    private SceneMan sceneMan;
    [SerializeField] public Slider scoreSlider;
    [SerializeField] private TextMeshProUGUI[] scoreValues;
    [SerializeField] private TextMeshProUGUI locationText;
    [SerializeField] private GameObject[] walkieTalkiePrefabs;
    private int[] voiceLineLicenses;
    [SerializeField] private Transform walkieTalkiePlace;
    [SerializeField] private GameObject walkieTalkieInstance;
    [SerializeField] private AudioSource talkie;
    public int seedCombo;
    public TextMeshProUGUI seedComboText;
    private float seedComboTimeOut;
    [SerializeField] private GameObject keepMovingButton;
    public bool allowMovement = true;

    List<IObserver> observers = new List<IObserver>();

    public void AddObserver(IObserver observer)
    {
        observers.Add(observer);
    }

    public void Notify()
    {
        Debug.Log("Notify");
        foreach(var observer in observers)
        {
            observer.ObserverUpdate();
        }
    }

    private void Update()
    {
        if (seedComboText.fontSize != 0)
        {
            seedComboTimeOut += Time.deltaTime;
            if (seedComboTimeOut > 4)
            {
                seedComboText.text = seedComboText.text.Replace((seedCombo) + "x", 0 + "x");
                seedCombo = 0;
                seedComboTimeOut = 0;
                seedComboText.fontSize = 0;
            }
        }
    }

    private void Awake()
    {
        sceneMan = FindObjectOfType<SceneMan>();
        cutMan = FindObjectOfType<CutMan>();
    }

    // Start is called before the first frame update
    void Start()
    {
        locationText.text = SceneManager.GetActiveScene().name;
        voiceLineLicenses = new[] {20, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1,1 };
        HighScoreManager hSM = FindObjectOfType<HighScoreManager>();
        LevelScoreData scoreData = hSM.CalculateTotalScoreData();
        cutTrees = scoreData.treesCut;
        plantedTrees = scoreData.treesPlanted;
        scoreValues[1].text = "" + cutTrees;
        scoreValues[0].text = "" + plantedTrees;
        AdjustSlider();
    }

    private void AdjustSlider()
    {
        float result;
        if (plantedTrees - cutTrees > 0)
        {
            result = (-.025f * (plantedTrees - cutTrees))+.5f;
            if (result > 1)
                result = 1;
        }
        else
        {
            result = (-.025f * (plantedTrees - cutTrees))+.5f;
            if (result < 0)
                result = 0;
        }
        scoreSlider.value = result;
        Notify();

    }

    public void IncreaseScore(bool isCut)
    {
        if (isCut)
        {
            cutTrees++;
            cutTreesThisLvl++;
            score += 1 * cutMan.comboCount*cutMan.cutDifficulty;
            scoreValues[1].text = "" + cutTrees;
            AdjustSlider();
            if (cutMan.comboCount > highestCutCombo)
                highestCutCombo = cutMan.comboCount;
        }
        else
        {
            plantedTrees++;
            plantedTreesThisLvl++;
            score += 2 * seedCombo;
            scoreValues[0].text = "" + plantedTrees;
            seedCombo += 1;
            seedComboTimeOut = 0;
            AdjustSlider();
            if (seedCombo > 1)
            {
                if (seedComboText.fontSize == 0)
                    seedComboText.fontSize = 36;
                string temp = seedComboText.text;
                temp = temp.Replace((seedCombo - 1) + "x", seedCombo + "x");
                seedComboText.text = temp;
                if (seedCombo > highestPlantCombo)
                    highestPlantCombo = seedCombo;
                TryVoiceLine(1);
            }
        }
    }
    
    public void BackToMap()
    {
        HighScoreManager hSM = FindObjectOfType<HighScoreManager>();
        hSM.SetPlayerScore(cutTreesThisLvl,plantedTreesThisLvl,score,highestCutCombo,highestPlantCombo,cutMan.cutDifficulty);
        hSM.SaveCurrentLevelDataToFile();
        if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("Savannah"))
        {
            sceneMan.prevScene = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(mapScene, LoadSceneMode.Single);
        }
        else
        {
            SceneManager.LoadScene("Scoreboard", LoadSceneMode.Single);
        }
    }

    public void ClosePopUp(GameObject target)
    {
        target.SetActive(false);
        cutMan.mayCut = true;
    }

    public void TryVoiceLine(int voiceLineIndex)
    {
        if (voiceLineLicenses[voiceLineIndex] > 0)
        {
            voiceLineLicenses[voiceLineIndex] -= 1;
            if (walkieTalkieInstance != null)
            {
                Destroy(walkieTalkieInstance);
                StopCoroutine("RemoveVoiceLine");
            }

            walkieTalkieInstance = Instantiate(walkieTalkiePrefabs[voiceLineIndex], walkieTalkiePlace.parent);
            walkieTalkieInstance.transform.position = walkieTalkiePlace.position;
            StartCoroutine(RemoveVoiceLine(6f));
            talkie.time = 0;
            talkie.Play();
        }
    }
    
    public void TryVoiceLine(int voiceLineIndex, float displayTime)
    {
        if (voiceLineLicenses[voiceLineIndex] > 0)
        {
            voiceLineLicenses[voiceLineIndex] -= 1;
            if (walkieTalkieInstance != null)
            {
                Destroy(walkieTalkieInstance);
                StopCoroutine("RemoveVoiceLine");
            }

            walkieTalkieInstance = Instantiate(walkieTalkiePrefabs[voiceLineIndex], walkieTalkiePlace.parent);
            walkieTalkieInstance.transform.position = walkieTalkiePlace.position;
            StartCoroutine(RemoveVoiceLine(displayTime));
            talkie.time = 0;
            talkie.Play();
        }
    }

    IEnumerator RemoveVoiceLine(float displayTime)
    {
        yield return new WaitForSeconds(displayTime);
        Destroy(walkieTalkieInstance);
    }

    public void KeepWalking()
    {
        if (allowMovement)
        {
            cutMan.MoveArea();
        }
    }
}
