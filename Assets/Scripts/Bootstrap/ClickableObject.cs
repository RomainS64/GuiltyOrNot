using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableObject : MonoBehaviour,IPointerDownHandler
{
    public Action OnAssetClicked;
    [SerializeField] private Animator animator;
    private string clickParameter = "click";
    public void OnPointerDown(PointerEventData eventData)
    {
        OnAssetClicked?.Invoke();
        animator.SetTrigger(clickParameter);
    }
}
