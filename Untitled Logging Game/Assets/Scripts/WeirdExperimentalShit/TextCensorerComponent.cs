using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Linq;

public class TextCensorerComponent : MonoBehaviour
{
    
    private static string fileName = "CensoredWords.txt";

    public Text textToCheck;
    public InputField inputField;

    public bool CensorCheckOnUpdate =false;

    private TextCensorer textCensorer;
    

    public void Start()
    {
        textCensorer = new TextCensorer(Path.Combine(Application.persistentDataPath, fileName));
        SanitizeText();
    }

    public void SanitizeText()
    {
        if(textToCheck)
        {
            textToCheck.text = textCensorer.SanitizeWord(textToCheck.text);
        }

        if(inputField)
        {
            inputField.text = textCensorer.SanitizeWord(inputField.text);
        }
    }

    private void Update()
    {
        if(CensorCheckOnUpdate)
        {
            SanitizeText();
        }
    }

}

public class TextCensorer
{
    private List<string> wordsToCheck = new List<string>();
    private string replacementOptions;

    public delegate void OnBeforeTextSanitizationIndexFind(ref string toEdit);

    OnBeforeTextSanitizationIndexFind beforeTextSanitizationIndexFind;

    static private string defaultReplacement = "$!?@%&";

    Dictionary<char, char> numberCharToLetterChar = new Dictionary<char, char>();

   

    public TextCensorer(string censoredWordsFilePath,string replacementOptions = "$!?@%&")
    {
        LoadCensoredWords(censoredWordsFilePath);
        this.replacementOptions = replacementOptions;

        beforeTextSanitizationIndexFind += setToLowercase;
        beforeTextSanitizationIndexFind += doCharReplacement;

        numberCharToLetterChar.Add('0', 'O');
        numberCharToLetterChar.Add('1', 'i');
        numberCharToLetterChar.Add('3', 'e');
        numberCharToLetterChar.Add('7', 'T');
        numberCharToLetterChar.Add('!', 'i');
        numberCharToLetterChar.Add('8', 'b');
        numberCharToLetterChar.Add('l', 'i');
    }

    public void LoadCensoredWords(string censoredWordsFilePath)
    {
        if(!File.Exists(censoredWordsFilePath))
        {
            var stream = File.Create(censoredWordsFilePath);
            stream.Close();
            wordsToCheck.Concat(defaultCensored.ToList());
        }

        using (var reader = new StreamReader(File.Open(censoredWordsFilePath, FileMode.Open)))
        {
            int counter = 0;
            string ln;

            while ((ln = reader.ReadLine()) != null)
            {
               wordsToCheck.Add(ln.Trim().ToLower());
               counter++;
            }
            reader.Close();
            Debug.Log("There are " + counter + "censoredWords");
        }
    }

    public string SanitizeWord(string Text)
    {
        string sanitizedText = Text;

        foreach(string word in wordsToCheck)
        {

            beforeTextSanitizationIndexFind?.Invoke(ref Text);

            int indexOfWord = Text.IndexOf(word);

            if (indexOfWord != -1)
            {
                char[] charArrayText = sanitizedText.ToCharArray();
                for (int i = indexOfWord; i < indexOfWord + word.Length; i++)
                {
                    charArrayText[i] = RandomlySelectReplacementOption();
                }

                sanitizedText = new string(charArrayText);

            }
        }

        return sanitizedText;
    }

    private char RandomlySelectReplacementOption()
    {
        if(replacementOptions.Length == 0)
        {
            replacementOptions = defaultReplacement;
        }

        return replacementOptions[Random.Range(0, replacementOptions.Length)];

    }

    private void setToLowercase(ref string toSanitize)
    {
        toSanitize = toSanitize.ToLower();
    }

    private void doCharReplacement(ref string toSanitize)
    {
        //for each char in toSanitize
        char[] charArraySanitize = toSanitize.ToCharArray();

        for (int i = 0; i < charArraySanitize.Length; i++)
        {
            char foundVal;
            
            if(numberCharToLetterChar.TryGetValue(charArraySanitize[i],out foundVal))
            {
                Debug.Log("charArraySanitize[i] is " + charArraySanitize[i]);
                Debug.Log("foundVal is " + foundVal);
                charArraySanitize[i] = foundVal;
            }

        }

        toSanitize = new string(charArraySanitize);

    }

    private static string[] defaultCensored = {"anal",
"arse",
"ass",
"ballsack",
"balls",
"bastard",
"bitch",
"biatch",
"bloody",
"blowjob",
"blow job",
"bollock",
"bollok",
"boner",
"boob",
"bugger",
"bum",
"butt",
"buttplug",
"clitoris",
"cock",
"coon",
"crap",
"cunt",
"damn",
"dick",
"dildo",
"dyke",
"fag",
"feck",
"fellate",
"fellatio",
"felching",
"fuck",
"f u c k",
"fudgepacker",
"fudge packer",
"flange",
"Goddamn",
"God damn",
"hell",
"homo",
"jerk",
"jizz",
"knobend",
"knob end",
"labia",
"lmao",
"lmfao",
"muff",
"nigger",
"nigga",
"omg",
"penis",
"piss",
"poop",
"prick",
"pube",
"pussy",
"queer",
"scrotum",
"sex",
"shit",
"s hit",
"sh1t",
"slut",
"smegma",
"spunk",
"tit",
"tosser",
"turd",
"twat",
"vagina",
"wank",
"whore",
"wtf",};

}

