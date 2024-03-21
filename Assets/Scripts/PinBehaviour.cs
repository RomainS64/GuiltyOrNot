using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PinBehaviour : MonoBehaviour, IPointerClickHandler
{
    private bool isLeftPin;
    private int pinId;
    public void Init(int _pinId,bool _idLeft)
    {
        pinId = _pinId;
        isLeftPin = _idLeft;
    }
    public void OnPointerClick(PointerEventData eventData)=>PlacePin.Instance.RemovePinsAndRope(pinId);
    
}
