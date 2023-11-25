using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PinableObject : MonoBehaviour,IPointerUpHandler,IPointerDownHandler
{
    [SerializeField]
    private MovableObject linkedMovable = null;

    [SerializeField] private Transform forcedPinPosition = null;
    private float downTime;
    private bool isPined = false;
    public void OnPointerUp(PointerEventData eventData)
    {
        linkedMovable?.OnPointerUp(null);
        if (Time.time - downTime <0.2 && !isPined)
        {
            isPined = true;
            PlacePin.Instance.PinablePined(gameObject.transform,forcedPinPosition);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        linkedMovable?.OnPointerDown(null);
        downTime = Time.time;
    }
}
