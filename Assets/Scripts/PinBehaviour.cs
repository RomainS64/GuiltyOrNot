using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PinBehaviour : MonoBehaviour
{
    private bool isLeftPin;
    private int pinId;
    private Collider2D collider;
    private List<GameObject> overlappingObjects = new ();

    private void Awake()
    {
        collider = GetComponent<Collider2D>();
        MovableObject.OnSiblingChanged += NotifySiblingChanged;
        DocumentPlacement.Instance.OnDocumentReplaced += NotifySiblingChanged;
        DocumentPlacement.Instance.OnIdsSetToTop += ResetShowRope;

    }

    public void Init(int _pinId,bool _idLeft)
    {
        pinId = _pinId;
        isLeftPin = _idLeft;
    }

    public void ResetShowRope()
    {
        if(!isLeftPin)PlacePin.Instance.ShowRope(pinId,true,isLeftPin);
    }
    public void NotifySiblingChanged()
    {
        List<Collider2D> collidersInContact = new List<Collider2D>();
        if (collider == null) return;
        if (collider.GetContacts(collidersInContact)> 0)
        {
            foreach (var col in collidersInContact)
            {
                if (col.transform.GetSiblingIndex() > transform.parent.GetSiblingIndex())
                {
                    if(!overlappingObjects.Contains(col.gameObject))overlappingObjects.Add(col.gameObject);
                    PlacePin.Instance.ShowRope(pinId,false,isLeftPin);
                    return;
                }
                if (overlappingObjects.Contains(col.gameObject))
                {
                    overlappingObjects.Remove(col.gameObject);
                    if (overlappingObjects.Count == 0)
                    {
                        PlacePin.Instance.ShowRope(pinId,true,isLeftPin);
                    }
                }
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.transform.GetSiblingIndex() > transform.parent.GetSiblingIndex())
        {
            overlappingObjects.Add(col.gameObject);
            PlacePin.Instance.ShowRope(pinId,false,isLeftPin);
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (overlappingObjects.Contains(other.gameObject))
        {
            overlappingObjects.Remove(other.gameObject);
            if (overlappingObjects.Count == 0)
            {
                PlacePin.Instance.ShowRope(pinId,true,isLeftPin);
            }
        }
        
    }
}
