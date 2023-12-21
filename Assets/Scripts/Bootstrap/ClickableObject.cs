using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickableObject : MonoBehaviour,IPointerUpHandler,IPointerDownHandler
{
    public Action OnAssetClicked;
    [SerializeField] private Animator animator;
    private string clickParameter = "click";
    public bool IsLock { get; set; } = false;
    private float downTime;
    public void OnPointerDown(PointerEventData eventData)
    {
        if (IsLock) return;
        downTime = Time.time;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (IsLock) return;
        if (Time.time - downTime <0.2)
        {
            OnAssetClicked?.Invoke();
            animator.SetTrigger(clickParameter);
        }
    }
}
