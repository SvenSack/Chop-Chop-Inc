using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CurrentHighScoreViewer : UIPlacable
{
    public HighScoreManager highScoreManager;
    public HighScoreView highScoreView;
    public PlayerLevelTeleporter levelTeleporter;

    private GameObject highScoreViewerInstance;

    public Vector2 shiftAmount = new Vector2(500, 0);
    public float shiftTime = 1.0f;

    public float teleportTime = 1.2f;

    private Button button;

    public void Initialize()
    {
        highScoreManager = FindObjectOfType<HighScoreManager>();

        Vector2 position = CalculateUIPosition();

        LevelScoreData data = highScoreManager.CalculateTotalScoreData();

        //instantiate score preview
        GameObject highScoreViewerInstance = Instantiate(textInstanceObject);
        this.highScoreViewerInstance = highScoreViewerInstance;

        TextInstance textInstance = highScoreViewerInstance.GetComponent<TextInstance>();

        textInstance.GetComboCut().text = data.highestComboCut.ToString();
        textInstance.GetComboPlant().text = data.highestComboPlant.ToString();
        textInstance.GetCutCount().text = data.treesCut.ToString();
        textInstance.GetPlantCount().text = data.treesPlanted.ToString();

        var rect = highScoreViewerInstance.GetComponent<RectTransform>();

        rect.anchoredPosition = position;

        textInstance.gameObject.transform.SetParent(canvasObject.gameObject.transform);


        button = highScoreViewerInstance.GetComponentInChildren<Button>();

        if(button)
        {
            button.onClick.AddListener(DeactivateButton);
            button.onClick.AddListener(highScoreView.SlotCurrentScore);
            button.onClick.AddListener(MoveViewerInstanceUp);

            button.onClick.AddListener(highScoreManager.AddCurrentPlayerScoreDataToDisk);

            button.onClick.AddListener(highScoreManager.ResetLevelDataInFile);
            button.onClick.AddListener(DestroyAllDontDestroyOnLoad);
            button.onClick.AddListener(TeleportBackToMap);
             
        }



    }

    public void MoveViewerInstanceUp()
    {
        var rect = highScoreViewerInstance.GetComponent<RectTransform>();
        Vector2 oldPosition = rect.anchoredPosition;

        LeanTween.move(rect, oldPosition + shiftAmount, shiftTime);

    }

    public void DestroyAllDontDestroyOnLoad()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("DDOL");

        foreach(GameObject toDestroy in objects)
        {
            Destroy(toDestroy);
        }
    }

    public void TeleportBackToMap()
    {
        levelTeleporter.TeleportPlayerIn(teleportTime);
    }


    public void DeactivateButton()
    {
        button.interactable = false;
    }

}
