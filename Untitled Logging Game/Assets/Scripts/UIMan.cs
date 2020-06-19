﻿using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIMan : MonoBehaviour
{

    public int cutTrees;
    public int plantedTrees;
    
    public string mapScene;
    private CutMan cutMan;

    private SceneMan sceneMan;
    [SerializeField] private Slider scoreSlider;
    [SerializeField] private TextMeshProUGUI[] scoreValues;
    [SerializeField] private TextMeshProUGUI locationText;

    private void Awake()
    {
        sceneMan = FindObjectOfType<SceneMan>();
        cutMan = FindObjectOfType<CutMan>();
    }

    // Start is called before the first frame update
    void Start()
    {
        locationText.text = SceneManager.GetActiveScene().name;
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
    }

    public void IncreaseScore(bool isCut)
    {
        if (isCut)
        {
            cutTrees++;
            scoreValues[1].text = "" + cutTrees;
            AdjustSlider();
        }
        else
        {
            plantedTrees++;
            scoreValues[0].text = "" + plantedTrees;
            AdjustSlider();
        }
    }
    
    public void BackToMap()
    {
        sceneMan.prevScene = SceneManager.GetActiveScene().name;
        SceneManager.LoadScene(mapScene, LoadSceneMode.Single);
    }

    public void ClosePopUp(GameObject target)
    {
        target.SetActive(false);
        cutMan.mayCut = true;
    }
}
