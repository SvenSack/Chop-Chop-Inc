using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameObjectActivator : MonoBehaviour
{
    public GameObject objectToActivate;

    public void ActivateObject()
    {
        objectToActivate.SetActive(true);
    }
  
}
