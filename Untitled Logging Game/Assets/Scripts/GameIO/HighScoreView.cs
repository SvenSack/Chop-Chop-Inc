using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// using UnityEditor.UI;

public class HighScoreView : MonoBehaviour
{
    public GameObject canvasObject;
    public GameObject textInstanceObject;

    public HighScoreManager highScoreManager;

    [Range(0,1.0f)]
    public float shiftPercentageX;
    [Range(0, 1.0f)]
    public float shiftPercentageY;

    [SerializeField] private string scoreText = "Score : ";
    [SerializeField] private string nameText = "Name : ";

    void Start()
    {
        
    }

    public void Initialize()
    {
        Debug.Log("Initialize HighScoreView");

        Canvas canvas = FindObjectOfType<Canvas>();
        canvasObject = canvas.gameObject;

        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
        float width = canvasRect.rect.width;
        float height = canvasRect.rect.height;

        Debug.Log("Canvas width and height " + width + "," + height);

        highScoreManager = FindObjectOfType<HighScoreManager>();

        Vector2 topLeft = new Vector2(0, height);

        Vector2 shiftInWidth = new Vector2(width, 0) * shiftPercentageX;
        Vector2 shiftInHeight = new Vector2(0, height) * shiftPercentageY;


        Vector2 position = topLeft + shiftInWidth - shiftInHeight;

        Vector2 shiftAmount = new Vector2(0, -100);

        foreach(PlayerGameData data in highScoreManager.GetScoreData())
        {
            Debug.Log("loaded score data");

            GameObject textInstanceObj = Instantiate(textInstanceObject);
            TextInstance textInstance = textInstanceObj.GetComponent<TextInstance>();
            textInstance.Intialize();

            textInstance.GetName().text = data.Name;
            textInstance.GetScore().text = data.Score.ToString();

            RectTransform textRect = textInstanceObj.GetComponent<RectTransform>();
            textRect.anchoredPosition = position;
            Debug.Log("textRect.anchoredPosition " + textRect.anchoredPosition);

            position += shiftAmount;

            textInstance.gameObject.transform.parent = canvasObject.gameObject.transform;
        }
    }

}
