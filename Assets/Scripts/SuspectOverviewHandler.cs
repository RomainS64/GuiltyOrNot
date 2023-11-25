using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SuspectOverviewHandler : MonoBehaviour
{
    private const string CanvasDisplayParameter = "isDisplayed";
    private static readonly int IsDisplayed = Animator.StringToHash(CanvasDisplayParameter);
    
    [SerializeField] private Animator canvasAnimator;
    private bool isCurrentlyDisplayed = false;
    
    public void OnFolderClicked()
    {
        isCurrentlyDisplayed = !isCurrentlyDisplayed;
        canvasAnimator.SetBool(IsDisplayed,isCurrentlyDisplayed);
    }
}
