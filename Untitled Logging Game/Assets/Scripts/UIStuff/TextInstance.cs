using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextInstance : MonoBehaviour
{
   [SerializeField] private Text score;
    [SerializeField] private Text name;

    public void Intialize()
    {
        foreach(Transform child in transform)
        {
            if(child.tag == "score")
            {
                score = child.GetComponent<Text>();
            }
            else if(child.tag == "name")
            {
                name = child.GetComponent<Text>();
            }
        }
    }

    public Text GetName()
    {
        return name;
    }

    public Text GetScore()
    {
        return score;
    }
}
