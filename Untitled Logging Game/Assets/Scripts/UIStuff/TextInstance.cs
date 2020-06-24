using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TextInstance : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI name;
    [SerializeField] private TextMeshProUGUI comboCutCount;
    [SerializeField] private TextMeshProUGUI comboPlantCount;
    [SerializeField] private TextMeshProUGUI plantCount;
    [SerializeField] private TextMeshProUGUI cutCount;

    public float score;

    public TextMeshProUGUI GetName()
    {
        return name;
    }

    public TextMeshProUGUI GetComboCut()
    {
        return comboCutCount;
    }

    public TextMeshProUGUI GetComboPlant()
    {
        return comboPlantCount;
    }

    public TextMeshProUGUI GetCutCount()
    {
        return cutCount;
    }

    public TextMeshProUGUI GetPlantCount()
    {
        return plantCount;
    }

}
