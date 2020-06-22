using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Language
{
    English,
    Dutch,
    German
}

public class LanguageSwitchManager : MonoBehaviour
{

    void Start()
    {
        if(!PlayerPrefs.HasKey("language"))
        {
            PlayerPrefs.SetInt("language",(int)Language.Dutch);
        }
    }

    public void SwitchLanguageTo(Language language)
    {
        PlayerPrefs.SetInt("language", (int)language);
    }

    public void SwitchLanguageToDutch()
    {
        SwitchLanguageTo(Language.Dutch);
    }

    public void SwitchLanguageToGerman()
    {
        SwitchLanguageTo(Language.German);
    }

    public void SwitchLanguageToEnglish()
    {
        SwitchLanguageTo(Language.English);
    }
}
