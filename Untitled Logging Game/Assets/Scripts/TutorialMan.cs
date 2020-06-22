using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialMan : MonoBehaviour
{
    private int tutorialIndex;
    public GameObject[] tutorialSlides = new GameObject[4];
    [SerializeField] private GameObject zoe;
    [SerializeField] private Transform zoeGoal;
    [SerializeField] private GameObject button;
    public bool isFirstLevel;
    private bool lastStep;
    private bool active;
    private Vector3 zoeOrigin;

    private void Start()
    {
        foreach (var slide in tutorialSlides)
        {
            slide.SetActive(false);
        }

        zoeOrigin = zoe.transform.position;
        zoe.SetActive(false);
        button.SetActive(false);
        
        if(isFirstLevel)
            TutorialActivate();
    }

    public void TutorialActivate()
    {
        if (!active)
        {
            zoe.SetActive(true);
            zoe.LeanMove(zoeGoal.position, 3f);
            StartCoroutine(ZoeDelay(3f));
            active = true;
        }
        else
        {
            zoe.LeanMove(zoeOrigin, 3f);
            StartCoroutine(ZoeDelay(3f));
            active = false;
        }
    }

    IEnumerator ZoeDelay(float delay)
    {
        if (!active)
        {
            yield return new WaitForSeconds(delay);
            tutorialSlides[tutorialIndex].SetActive(true);
            button.SetActive(true);
        }
        else
        {
            tutorialSlides[tutorialIndex].SetActive(false);
            button.SetActive(false);
            yield return new WaitForSeconds(delay);
            zoe.SetActive(false);
        }
    }

    public void Next()
    {
        tutorialSlides[tutorialIndex].SetActive(false);
        tutorialIndex++;
        if(tutorialIndex < tutorialSlides.Length-1)
            tutorialSlides[tutorialIndex].SetActive(true);
        else
        {
            if (!lastStep)
            {
                tutorialSlides[tutorialIndex].SetActive(true);
                lastStep = true;
            }
            else
            {
                lastStep = false;
                tutorialSlides[tutorialIndex-1].SetActive(false);
                tutorialIndex = 0;
                TutorialActivate();
            }
        }
    }
}
