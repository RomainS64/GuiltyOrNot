using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

    }

    private void OnEnable()
    {
        animator = GetComponent<Animator>();
        animator.SetBool(crossedParameter,IsCrossed);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        IsCrossed = !IsCrossed;
        AudioManager.instance.audioEvents[IsCrossed?"Suspect Off":"Suspect On"].Play();
        ScenarioFlow.Instance.SetSuspectEliminationStatus(photoSetter.GetPlayerId(), IsCrossed);
        CrossLinkPhoto[] crossLinkPhotos = FindObjectsOfType<CrossLinkPhoto>(true).Where(photo => photo.Id == photoSetter.GetPlayerId()).ToArray();
        foreach (var crossLinkPhoto in crossLinkPhotos)
        {
            crossLinkPhoto.SetCross(IsCrossed);
        }
        animator.SetBool(crossedParameter,IsCrossed);
    }
    
}
