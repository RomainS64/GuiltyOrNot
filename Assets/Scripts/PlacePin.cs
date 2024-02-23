using System;
using System.Collections;
using System.Collections.Generic;
using Rope;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class PlacePin : MonoSingleton<PlacePin>
{
    [SerializeField] private bool placePinAtStart = false;
    [SerializeField ]private Camera camera;
    [SerializeField] private GameObject pinPrefab;
    [SerializeField] private GameObject pinCanvas;
    [SerializeField] private GameObject ropePrefab;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 ropeOffset;
    [SerializeField] private Transform mouseFollow;
    private RopeController currentRope;
    
    private bool isFirstPin = false;
    private bool isInPinMode = false;

    private void Start()
    {
        SetPlacePinMode(placePinAtStart);
    }
    public void SetPlacePinMode(bool _active) => isInPinMode = _active;
    public void PinablePined(Transform _pined,Transform _forcedPosition = null)
    {
        AudioManager.instance.audioEvents["Pin"].Play();
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
            currentRope.SetRightPoin(newPin.transform, ropeOffset);
        }
    }
    
    
    private void Update()
    {
        mouseFollow.position = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 2));
    }
}
