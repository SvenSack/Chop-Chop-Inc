using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreboardSceneManager : MonoBehaviour
{
    HighScoreManager scoreboardManager;
    HighScoreView scoreboardViewer;

    // Start is called before the first frame update
    void Start()
    {
        scoreboardManager = FindObjectOfType<HighScoreManager>();
        scoreboardViewer = FindObjectOfType<HighScoreView>();

        scoreboardManager.Initialize();

        scoreboardViewer.Initialize();

    }

}
