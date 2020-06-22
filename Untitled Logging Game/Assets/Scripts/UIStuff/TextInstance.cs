using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextInstance : MonoBehaviour
{
    [SerializeField] private Text name;
    [SerializeField] private Text comboCutCount;
    [SerializeField] private Text comboPlantCount;
    [SerializeField] private Text plantCount;
    [SerializeField] private Text cutCount;

    public float score;

    public Text GetName()
    {
        return name;
    }

    public Text GetComboCut()
    {
        return comboCutCount;
    }

    public Text GetComboPlant()
    {
        return comboPlantCount;
    }

    public Text GetCutCount()
    {
        return cutCount;
    }

    public Text GetPlantCount()
    {
        return plantCount;
    }

}
