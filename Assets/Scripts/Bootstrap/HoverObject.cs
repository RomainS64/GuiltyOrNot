using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverObject : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private Animator animator;
    private string hoverParameter = "hover";
    public bool IsLock { get; set; } = false;
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (IsLock) return;
        animator.SetBool(hoverParameter,true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (IsLock) return;
        animator.SetBool(hoverParameter,false);
    }
}
