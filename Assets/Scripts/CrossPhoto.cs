using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class CrossPhoto : MonoBehaviour, IPointerClickHandler
{
    private Animator animator;
    private PhotoSetter photoSetter;
    private string crossedParameter = "cross";
    public bool IsCrossed { get; private set; } = false;
    
    private void Start()
    {
        photoSetter = GetComponent<PhotoSetter>();
        animator = GetComponent<Animator>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Right) return;
        IsCrossed = !IsCrossed;
        AudioManager.instance.audioEvents[IsCrossed?"Suspect Off":"Suspect On"].Play();
        ScenarioFlow.Instance.SetSuspectEliminationStatus(photoSetter.GetPlayerId(), IsCrossed);
        animator.SetBool(crossedParameter,IsCrossed);
    }
    
}
