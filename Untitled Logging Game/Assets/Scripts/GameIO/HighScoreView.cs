using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor.UI;

public class HighScoreView : UIPlacable
{
    

    public HighScoreManager highScoreManager;

    Vector2 shiftAmount = new Vector2(0, -100);
    Vector2 initialPosition;

    [SerializeField] private string scoreText = "Score : ";
    [SerializeField] private string nameText = "Name : ";

    float slotEntryTime = 1.0f;

    void Start()
    {
        
    }

    public void Initialize()
    {
        Debug.Log("Initialize HighScoreView");

        highScoreManager = FindObjectOfType<HighScoreManager>();

        Vector2 position = CalculateUIPosition();
        initialPosition = position;



        foreach (PlayerGameData data in highScoreManager.GetScoreData())
        {
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

            RectTransform textRect = textInstanceObj.GetComponent<RectTransform>();
            textRect.anchoredPosition = position;
            Debug.Log("textRect.anchoredPosition " + textRect.anchoredPosition);

            position += shiftAmount;

            textInstance.gameObject.transform.parent = canvasObject.gameObject.transform;
        }
    }

    public void SlotCurrentScore()
    {
        LevelScoreData data = highScoreManager.CalculateTotalScoreData();

        float score = data.score;
        //---------------------- Get all score blocks------------------//
        List<Transform> scoreBlocks = new List<Transform>();
        foreach(Transform child in canvasObject.transform)
        {
            if(child.tag == "scoreBlock")
            {
                scoreBlocks.Add(child);
            }
        }



        //------------------------ Find which index to slot to -----------------------//
        bool isInBetweenBlocks = false;
        int slotIndex = 0;

        for (int i = 0; i < scoreBlocks.Count-1; i++)
        {
            TextInstance currenttTextInstance = scoreBlocks[i].GetComponent<TextInstance>();

            if (score < currenttTextInstance.score)
            {
                slotIndex = i + 1;
                isInBetweenBlocks = true;
                break;
            }
        }
        Debug.Log("score " + score);
        Debug.Log("isInBetweenBlocks " + isInBetweenBlocks);

        //------------------------- Instantiate new score block-----------------------//
        RectTransform rect = scoreBlocks[slotIndex].GetComponent<RectTransform>();

        GameObject newScoreBlock = Instantiate(textInstanceObject);
        TextInstance textInstance = newScoreBlock.GetComponent<TextInstance>();
        textInstance.score = data.score;
        textInstance.GetName().text = highScoreManager.currentPlayerName;
        textInstance.GetComboCut().text = data.highestComboCut.ToString();
        textInstance.GetComboPlant().text = data.highestComboPlant.ToString();
        textInstance.GetCutCount().text = data.treesCut.ToString();
        textInstance.GetPlantCount().text = data.treesPlanted.ToString();






        //----------------------------- Move to correct positions---------------------//
        var newScoreBlockRect = newScoreBlock.GetComponent<RectTransform>();
        newScoreBlockRect.anchoredPosition = GetNewScoreBlockInitialPosition();
        newScoreBlock.transform.SetParent(canvasObject.transform);


        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();

        float height = canvasRect.rect.height;


        Vector2 defaultPosition = Vector2.zero;

        defaultPosition.x = initialPosition.x;
        defaultPosition.y = -(height - initialPosition.y) + (shiftAmount.y * scoreBlocks.Count);


        Vector2 targetPosition = isInBetweenBlocks ? rect.anchoredPosition : defaultPosition;

        LeanTween.move(newScoreBlockRect, targetPosition, slotEntryTime);

        //------------------Shift all score blocks below new scoe block-----------------//

        if (!isInBetweenBlocks) { return; }

        for (int i = slotIndex; i < scoreBlocks.Count; i++)
        {
            RectTransform belowSlotRect = scoreBlocks[i].GetComponent<RectTransform>();
            Vector2 position = belowSlotRect.anchoredPosition;


            LeanTween.move(belowSlotRect, position + shiftAmount, slotEntryTime);
        }





    }

    public Vector2 GetNewScoreBlockInitialPosition()
    {
        return initialPosition - shiftAmount * 2;
    }


}
