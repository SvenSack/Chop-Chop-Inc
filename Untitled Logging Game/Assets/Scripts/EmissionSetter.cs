using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmissionSetter : MonoBehaviour,IObserver
{
    [SerializeField] Material material;

    [SerializeField]private UIMan uiMan;
    private OnNewSliderValueFound onNewSliderValueFound;

    public bool reverseEmisssion = false;
    private Color defaultColor;

    public void ObserverUpdate()
    {
        SetVolumeValue(uiMan.scoreSlider.value);
    }

    private void SetVolumeValue(float value)
    {
        onNewSliderValueFound?.Invoke(value);
    }

    public void Start()
    {

        defaultColor = material.GetColor("_EmissionColor");


        uiMan.AddObserver(this);

        if (reverseEmisssion)
        {
            onNewSliderValueFound += SetReverseEmissionValue;
        }
        else
        {
            onNewSliderValueFound += SetEmissionValue;
        }
    }

    private void SetEmissionValue(float value)
    {
        Color color = defaultColor;
        color.a = value;
        material.SetColor("_EmissionColor", defaultColor * value);
    }

    private void SetReverseEmissionValue(float value)
    {
        Color color = defaultColor;
        color.a = (1.0f - value);
        material.SetColor("_EmissionColor", color);
    }

    private void OnApplicationQuit()
    {
        material.SetColor("_EmissionColor", defaultColor);
    }

    private void Update()
    {
        Debug.Log("color "  + material.GetColor("_EmissionColor"));
    }
}
