using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Debug = UnityEngine.Debug;

public class KeyboardMan : MonoBehaviour
{
    public enum ShiftState{Low, FirstCase, Caps}

    public ShiftState currentShift;
    [SerializeField] private TextMeshProUGUI[] buttons;
    [SerializeField] private Color pressedColor;
    [SerializeField] private Color unPressedColor;
    [SerializeField] private Button shiftButton;
    public TextMeshProUGUI targetTextBox;
    [SerializeField] private Sprite pressedShiftSprite;
    [SerializeField] private Sprite unPressedShiftSprite;

    private void Start()
    {
        currentShift = ShiftState.FirstCase;
    }

    public void keyBoardLetterKey(Button button)
    {
        targetTextBox.text = targetTextBox.text + button.GetComponentInChildren<TextMeshProUGUI>().text;
        if (currentShift == ShiftState.FirstCase)
        {
            KeyBoardToggleShift(shiftButton);
            KeyBoardToggleShift(shiftButton);
        }
    }

    public void KeyBoardToggleShift(Button button)
    {
        currentShift++;
        if ((int) currentShift > 2)
            currentShift = ShiftState.Low;
        switch (currentShift)
        {
            case ShiftState.Low:
            {
                foreach (var txt in buttons)
                {
                    txt.text = txt.text.ToLowerInvariant();
                }
                button.GetComponent<Image>().sprite = unPressedShiftSprite;
                break;
            }

            case ShiftState.FirstCase:
            {
                foreach (var txt in buttons)
                {
                    txt.text = txt.text.ToUpperInvariant();
                }
                
                button.transform.GetChild(1).GetComponent<Image>().color = pressedColor;
                break;
            }
            
            case ShiftState.Caps:
            {
                button.transform.GetChild(1).GetComponent<Image>().color = unPressedColor;
                button.GetComponent<Image>().sprite = pressedShiftSprite;
                break;
            }
        }
    }

    public void KeyBoardSpaceKey()
    {
        targetTextBox.text = targetTextBox.text.Substring(0, targetTextBox.text.Length - 1);
    }
}
