using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PinableObject : MonoBehaviour,IPointerUpHandler,IPointerDownHandler
{
    [SerializeField] private bool infinitPin;
    [SerializeField]
    private MovableObject linkedMovable = null;

    [SerializeField] private Transform forcedPinPosition = null;
    private float downTime;
    private bool isPined = false;
    public List<int> PinIds { get; set; } = new();

    public void AddPin(int pinId)
    {
        if (!PinIds.Contains(pinId))
        {
            PinIds.Add(pinId);
        }
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        linkedMovable?.OnPointerUp(null);
        if (Time.time - downTime <0.15 && !isPined)
        {
            if(!infinitPin)isPined = true;
            PlacePin.Instance.PinablePined(gameObject.transform,forcedPinPosition);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        linkedMovable?.OnPointerDown(null);
        downTime = Time.time;
    }
}
