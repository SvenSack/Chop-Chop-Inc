using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class LanguageSwitcher : MonoBehaviour
{
    private TextMeshProUGUI TMPToCheck;
    private Text textToCheck;

    public string DutchReplacementText = "[DUTCH]";
    public string GermanReplacementText = "[GERMAN]";

    private string englishReplacementText;

    public bool checkTextSwitchOnStartOnly = false;

     // Start is called before the first frame update
     void Start()
    {
        TMPToCheck = GetComponent<TextMeshProUGUI>();
        textToCheck = GetComponent<Text>();

        if (TMPToCheck)
        {
            englishReplacementText = TMPToCheck.text;
        }

        if (textToCheck)
        {

            englishReplacementText = textToCheck.text;
        }


        SwitchTextLanguage(); 

    }

    // Update is called once per frame
    void Update()
    {
        if(checkTextSwitchOnStartOnly) { return; }

        SwitchTextLanguage();
    }

    public void SwitchTextLanguage()
    {
        Debug.Log("switch");
        Language language = (Language)PlayerPrefs.GetInt("language", 0);
        Debug.Log("language " + language);
        string switchResult = "";

        switch(language)
        {
            case Language.Dutch:
                switchResult = DutchReplacementText;
                break;
            case Language.German:
                switchResult = GermanReplacementText;
                break;
            case Language.English:
                switchResult = englishReplacementText;
                break;

            default:
                break;
        }

        if(TMPToCheck)
        {
            TMPToCheck.text = switchResult;
        }

        if(textToCheck)
        {
            textToCheck.text = switchResult;
        }


    }
}
