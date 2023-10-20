using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class SuspectAnimationHandler : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler,IPointerDownHandler
{
    [SerializeField] private DebugContent tmpDebugContent;
    private Animator animator;
    private const string hoverParameter = "hover";
    private const string clickParameter = "click";

    private bool tmpIsDisplaying;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool(hoverParameter,true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool(hoverParameter,false);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        animator.SetTrigger(clickParameter);
        if (!tmpIsDisplaying)
        {
           tmpDebugContent.DisplaySheet(); 
        }
        else
        {
            tmpDebugContent.HideSheet();
        }

        tmpIsDisplaying = !tmpIsDisplaying;
    }
}
