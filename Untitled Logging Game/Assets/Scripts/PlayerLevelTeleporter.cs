using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class PlayerLevelTeleporter : MonoBehaviour
{
    public string SceneName;

    // Start is called before the first frame update
    public void TeleportPlayerIn(float seconds)
    {
        StartCoroutine(TeleportWithDelay(seconds));
    }


    IEnumerator TeleportWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        Debug.Log("SceneManager.LoadScene");
        SceneManager.LoadScene(SceneName);
    }
}
