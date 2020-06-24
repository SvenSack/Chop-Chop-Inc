using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor.UI;

public class HighScoreView : MonoBehaviour
{


    public HighScoreManager highScoreManager;

    public Transform dad;

    float slotEntryTime = 2.0f;

    public int maxDisplayCount = 9;

    public GameObject textInstanceObject;


    public Transform[] spots = new Transform[10];

    void Start()
    {

    }

    public void Initialize()
    {
        Debug.Log("Initialize HighScoreView");

        highScoreManager = FindObjectOfType<HighScoreManager>();

        int i = 0;
        foreach (PlayerGameData data in highScoreManager.GetScoreData())
        {
            if (i > maxDisplayCount)
            {
                break;
            }

            Debug.Log("loaded score data");

            GameObject textInstanceObj = Instantiate(textInstanceObject);
            TextInstance textInstance = textInstanceObj.GetComponent<TextInstance>();

            textInstance.score = data.Score;
            textInstance.GetName().text = data.Name;
            textInstance.GetComboCut().text = data.highestComboCut.ToString();
            textInstance.GetComboPlant().text = data.highestComboPlant.ToString();
            textInstance.GetCutCount().text = data.treesCut.ToString();
            textInstance.GetPlantCount().text = data.treesPlanted.ToString();

            Debug.Log("data.treesCut" + data.treesCut);

            textInstance.gameObject.transform.SetParent(dad);
            textInstance.transform.position = spots[i].position;
            i++;
        }
    }

    private int SortByScore(Transform t1, Transform t2)
    {
        return t1.GetComponent<TextInstance>().score.CompareTo(t2.GetComponent<TextInstance>().score);
    }

    public void SlotCurrentScore()
    {
        LevelScoreData data = highScoreManager.CalculateTotalScoreData();

        float score = data.score;

        //------------------------- Instantiate new score block-----------------------//

        GameObject newScoreBlock = Instantiate(textInstanceObject);
        TextInstance textInstance = newScoreBlock.GetComponent<TextInstance>();
        textInstance.score = data.score;
        textInstance.GetName().text = highScoreManager.currentPlayerName;
        textInstance.GetComboCut().text = data.highestComboCut.ToString();
        textInstance.GetComboPlant().text = data.highestComboPlant.ToString();
        textInstance.GetCutCount().text = data.treesCut.ToString();
        textInstance.GetPlantCount().text = data.treesPlanted.ToString();
        newScoreBlock.transform.position = new Vector3(spots[0].position.x, Screen.height+newScoreBlock.GetComponent<RectTransform>().rect.height,0);
        newScoreBlock.transform.SetParent(dad);


        //---------------------- Get all score blocks------------------//
        List<Transform> scoreBlocks = new List<Transform>();
        foreach (Transform child in dad)
        {
            if (child.tag == "scoreBlock")
            {
                scoreBlocks.Add(child);
            }
        }

        //------------------------ Find which index to slot to -----------------------//
        scoreBlocks.Sort(SortByScore);
        int slot = 0;
        for (int j = 0; j < scoreBlocks.Count; j++)
        {
            if (scoreBlocks[j] == newScoreBlock.transform)
            {
                slot = j;
                break;
            }
        }




        //----------------------------- Move to correct positions---------------------//
        for (int i = slot; i < scoreBlocks.Count; i++)
        {
            scoreBlocks[i].LeanMove(spots[i].position, slotEntryTime);
        }
    }
}
