using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CurrentHighScoreViewer : MonoBehaviour
{
    public HighScoreManager highScoreManager;
    public PlayerLevelTeleporter levelTeleporter;
    public GameObject textInstanceObject;

    public float teleportTime = 1.2f;

    public Button button;

    public TextMeshProUGUI plantCount;
    public TextMeshProUGUI cutCount;
    public TextMeshProUGUI plantCombo;
    public TextMeshProUGUI cutCombo;
    

    public void Initialize()
    {
        highScoreManager = FindObjectOfType<HighScoreManager>();

        // Vector2 position = CalculateUIPosition();

        LevelScoreData data = highScoreManager.CalculateTotalScoreData();

        //instantiate score preview
        // GameObject highScoreViewerInstance = Instantiate(textInstanceObject);
        // this.highScoreViewerInstance = highScoreViewerInstance;

        /* TextInstance textInstance = highScoreViewerInstance.GetComponent<TextInstance>();

        

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
             
        }*/

        cutCombo.text = data.highestComboCut.ToString();
        plantCombo.text = data.highestComboPlant.ToString();
        cutCount.text = data.treesCut.ToString();
        plantCount.text = data.treesPlanted.ToString();

    }

    public void MoveViewerInstanceUp()
    {
        textInstanceObject.transform.LeanMove(new Vector3(textInstanceObject.transform.position.x, Screen.height+textInstanceObject.GetComponent<RectTransform>().rect.height,0), 3f);
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
