using System;
using TMPro;
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
    public int seedCombo;
    public TextMeshProUGUI seedComboText;
    private float seedComboTimeOut;

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
            }
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
