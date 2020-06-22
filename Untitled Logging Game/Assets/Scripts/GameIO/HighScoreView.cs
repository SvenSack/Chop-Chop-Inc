using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor.UI;

public class HighScoreView : UIPlacable
{
    

    public HighScoreManager highScoreManager;

    

    [SerializeField] private string scoreText = "Score : ";
    [SerializeField] private string nameText = "Name : ";

    void Start()
    {
        
    }

    public void Initialize()
    {
        Debug.Log("Initialize HighScoreView");

        highScoreManager = FindObjectOfType<HighScoreManager>();

        Vector2 position = CalculateUIPosition();

        Vector2 shiftAmount = new Vector2(0, -100);

        foreach(PlayerGameData data in highScoreManager.GetScoreData())
        {
            Debug.Log("loaded score data");

            GameObject textInstanceObj = Instantiate(textInstanceObject);
            TextInstance textInstance = textInstanceObj.GetComponent<TextInstance>();
          

            textInstance.GetName().text = data.Name;
            textInstance.GetComboCut().text = data.highestComboCut.ToString();
            textInstance.GetComboPlant().text = data.highestComboPlant.ToString();
            textInstance.GetCutCount().text = data.treesCut.ToString();
            textInstance.GetPlantCount().text = data.treesPlanted.ToString();





            RectTransform textRect = textInstanceObj.GetComponent<RectTransform>();
            textRect.anchoredPosition = position;
            Debug.Log("textRect.anchoredPosition " + textRect.anchoredPosition);

            position += shiftAmount;

            textInstance.gameObject.transform.parent = canvasObject.gameObject.transform;
        }
    }

}
