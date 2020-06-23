using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
delegate void OnNewSliderValueFound(float newValue);

public class MoodSetter : MonoBehaviour,IObserver
{
    private Volume volume;

    public UIMan uiman;
    
    public bool reverseVolumeSet = false;

    private OnNewSliderValueFound onNewSliderValueFound;

    public void ObserverUpdate()
    {
        SetVolumeValue(uiman.scoreSlider.value);
    }

    private void SetVolumeValue(float value)
    {
        onNewSliderValueFound?.Invoke(value);
    }

    public void Start()
    {
        volume = GetComponent<Volume>();
        uiman.AddObserver(this);

        if(reverseVolumeSet)
        {
            onNewSliderValueFound += ReverseVolumeToValue;
        }
        else
        {
            onNewSliderValueFound += SetVolumeToValue;
        }
    }

    private void SetVolumeToValue(float weight)
    {
        if(volume)
        {
            volume.weight = weight;
        }
        else
        {
            Debug.LogError("MoodSetter does not have a volume set");
        }
    }

    private void ReverseVolumeToValue(float weight)
    {
        if (volume)
        {
            volume.weight =1.0f - weight;
        }
        else
        {
            Debug.LogError("MoodSetter does not have a volume set");
        }
    }

}
