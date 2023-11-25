using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoverObject : MonoBehaviour,IPointerEnterHandler,IPointerExitHandler
{
    [SerializeField] private Animator animator;
    private string hoverParameter = "hover";
    public void OnPointerEnter(PointerEventData eventData)
    {
        animator.SetBool(hoverParameter,true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        animator.SetBool(hoverParameter,false);
    }
}
