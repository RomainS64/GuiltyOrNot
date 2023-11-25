using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScenarioView : MonoBehaviour
{
    public static Action OnScenarioViewSkiped;
        
    [SerializeField] private TMP_Text scenarioTextView;
    private bool isDisplayingText;

    public void DisplayText(string _text)
    {
        isDisplayingText = true;
        scenarioTextView.text = _text;
    }
}
