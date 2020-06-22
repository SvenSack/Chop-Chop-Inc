using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardSceneManager : MonoBehaviour
{
    HighScoreManager scoreboardManager;
    CurrentHighScoreViewer currentHighScoreViewer;
    HighScoreView scoreboardViewer;

    // Start is called before the first frame update
    void Start()
    {
        scoreboardManager = FindObjectOfType<HighScoreManager>();
        currentHighScoreViewer = FindObjectOfType<CurrentHighScoreViewer>();
        scoreboardViewer = FindObjectOfType<HighScoreView>();

        scoreboardManager.Initialize();
        scoreboardViewer.Initialize();
        currentHighScoreViewer.Initialize();



    }

}
