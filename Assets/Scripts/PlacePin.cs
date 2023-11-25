using System;
using System.Collections;
using System.Collections.Generic;
using Rope;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacePin : MonoSingleton<PlacePin>
{
    [SerializeField] private GameObject pinPrefab;
    [SerializeField] private GameObject pinCanvas;
    [SerializeField] private GameObject ropePrefab;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 ropeOffset;
    [SerializeField] private Transform mouseFollow;
    private RopeController currentRope;
    
    private bool isFirstPin = false;
    private bool isInPinMode = false;

    public void SetPlacePinMode(bool _active) => isInPinMode = _active;
    public void PinablePined(Transform _pined,Transform _forcedPosition = null)
    {
        if (!isInPinMode) return;
        isFirstPin = !isFirstPin;

        GameObject newPin = Instantiate(pinPrefab, _pined);
        if (_forcedPosition != null)
        {
            newPin.transform.position = _forcedPosition.position+offset;
        }
        else
        {
            newPin.transform.position = mouseFollow.position+offset;
        }
        if (isFirstPin)
        {
            GameObject newRope = Instantiate(ropePrefab, mouseFollow.position+ropeOffset, Quaternion.identity);
            currentRope = newRope.GetComponent<RopeController>();
            currentRope.SetLeftPoin(newPin.transform,ropeOffset);
            currentRope.SetRightPoin(mouseFollow,Vector3.zero);
        }
        else
        {
            currentRope.SetRightPoin(newPin.transform,ropeOffset);
        }
    }

    private void Update()
    {
        mouseFollow.position = Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 2));
    }
}
