using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ProceduralObjectPlacer))]
public class ProceduralPlacementEditor : Editor
{

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();

        ProceduralObjectPlacer objectPlacer = (ProceduralObjectPlacer)target;

        objectPlacer.Init();

        GUI.backgroundColor = Color.green;
        if (GUILayout.Button("Seeded Place Objects"))
        {
            

            objectPlacer.ClearDebugInfo();

            objectPlacer.DestroyChildren();

            objectPlacer.ProcedurallyPlaceObjects();
            

        }


        GUI.backgroundColor = Color.red;
        if (GUILayout.Button("Destroy Placed Object"))
        {
            objectPlacer.ClearDebugInfo();
            objectPlacer.DestroyChildren();
        }

        GUI.backgroundColor = Color.white;
        DrawDefaultInspector();

    }

}
