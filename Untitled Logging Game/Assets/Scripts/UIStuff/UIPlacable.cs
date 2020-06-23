using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlacable : MonoBehaviour
{
    [Range(-1.0f, 1.0f)]
    public float shiftPercentageX;
    [Range(-1.0f, 1.0f)]
    public float shiftPercentageY;

    public GameObject canvasObject;
    public GameObject textInstanceObject;

    private Canvas canvas;

    private void Start()
    {

        var rect = gameObject.GetComponent<RectTransform>();
        rect.anchoredPosition = CalculateUIPosition();

    }

    public Vector2 CalculateUIPosition()
    {
        canvas = FindObjectOfType<Canvas>();
        canvasObject = canvas.gameObject;

        RectTransform canvasRect = canvasObject.GetComponent<RectTransform>();
        float width = canvasRect.rect.width;
        float height = canvasRect.rect.height;

        Vector2 topLeft = new Vector2(0, height);

        Vector2 shiftInWidth = new Vector2(width, 0) * shiftPercentageX;
        Vector2 shiftInHeight = new Vector2(0, height) * shiftPercentageY;

        Vector2 position = topLeft + shiftInWidth - shiftInHeight;

        return position;
    }
}
