using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TextTest : MonoBehaviour
{
    public Text textComp;
    public int charIndex;
    public Canvas canvas;

    void PrintPos()
    {
        //get text
        string text = textComp.text;

        if (charIndex >= text.Length)
            return;

        TextGenerator textGen = new TextGenerator(text.Length);
        Vector2 extents = textComp.gameObject.GetComponent<RectTransform>().rect.size;
        Debug.Log("extents " + extents.ToString("F2"));
        textGen.Populate(text, textComp.GetGenerationSettings(extents));

        int newLine = text.Substring(0, charIndex).Split('\n').Length - 1;
        int whiteSpace = text.Substring(0, charIndex).Split(' ').Length - 1;
        int indexOfTextQuad = (charIndex * 4) + (newLine * 4) - 4;

        if (indexOfTextQuad < textGen.vertexCount)
        {
            Vector3 avgPos = (textGen.verts[indexOfTextQuad].position +
                textGen.verts[indexOfTextQuad + 1].position +
                textGen.verts[indexOfTextQuad + 2].position +
                textGen.verts[indexOfTextQuad + 3].position) / 4f;

            print(avgPos);
            PrintWorldPos(avgPos);
        }
        else
        {
            Debug.LogError("Out of text bound");
        }
    }

    void PrintWorldPos(Vector3 testPoint)
    {
        Vector3 worldPos = textComp.transform.TransformPoint(testPoint);
        print("PrintWorldPos " + worldPos);
        new GameObject("point").transform.position = worldPos;
        Debug.DrawRay(worldPos, Vector3.up, Color.red, 50f);
    }

    void OnGUI()
    {
        if (GUI.Button(new Rect(10, 10, 100, 80), "Test"))
        {
            PrintPos();
        }
    }
}
