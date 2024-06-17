using System.Collections.Generic;
using Rope;
using UnityEngine;


public struct RopeAndPins
{
    public PinBehaviour leftPin;
    public PinBehaviour rightPin;
    public GameObject rope;
    public bool leftAskShow;
    public bool rightAskShow;
    public bool isTotallyHiden;
}
public class PlacePin : MonoSingleton<PlacePin>
{
    [SerializeField] private bool placePinAtStart = false;
    [SerializeField] private Camera camera;
    [SerializeField] private GameObject pinPrefab;
    [SerializeField] private GameObject pinCanvas;
    [SerializeField] private GameObject ropePrefab;
    [SerializeField] private Vector3 offset;
    [SerializeField] private Vector3 ropeOffset;
    [SerializeField] private Transform mouseFollow;
    private RopeController currentRope;

    private List<RopeAndPins> allRopeAndPins = new();
    
    private bool isFirstPin = false;
    private bool isInPinMode = false;

    private void Start()
    {
        SetPlacePinMode(placePinAtStart);
    }
    public void SetPlacePinMode(bool _active) => isInPinMode = _active;

    public int PlaceLink(Transform _parentLeft,Transform _parentRight, Transform _left ,Transform _right)
    {
        PinablePined(_parentLeft, _left,_right);
        PinablePined(_parentRight, _right);
        return allRopeAndPins.Count - 1;
    }

    public void PinablePined(Transform _pined,Transform _forcedPosition = null,Transform _ropeTmpPosition = null)
    {
        AudioManager.instance.audioEvents["Pin"].Play();
        if (!isInPinMode) return;
        isFirstPin = !isFirstPin;

        GameObject newPin = Instantiate(pinPrefab, _pined);
        PinBehaviour newPinBehaviour = newPin.GetComponent<PinBehaviour>();
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
            newPinBehaviour.Init(allRopeAndPins.Count,true);
            //Increment + add to list newRope and newPin
            GameObject newRope = Instantiate(ropePrefab, _ropeTmpPosition==null?mouseFollow.position+ropeOffset:_ropeTmpPosition.position+ropeOffset, Quaternion.identity);
            RopeAndPins ropeAndPins = new RopeAndPins()
            {
                leftPin = newPin.GetComponent<PinBehaviour>(),
                rope = newRope,
                leftAskShow = true,
                rightAskShow = true
            };
            
            allRopeAndPins.Add(ropeAndPins);
            currentRope = newRope.GetComponent<RopeController>();
            currentRope.SetLeftPoin(newPin.transform,ropeOffset);
            currentRope.SetRightPoin(mouseFollow,ropeOffset);
        }
        else
        {
            newPinBehaviour.Init(allRopeAndPins.Count-1,false);
            RopeAndPins ropeAndPins = allRopeAndPins[^1];
            ropeAndPins.rightPin =  newPin.GetComponent<PinBehaviour>();
            allRopeAndPins[^1] = ropeAndPins;
            currentRope.SetRightPoin(newPin.transform, ropeOffset);
        }
        _pined.GetComponent<PinableObject>()?.AddPin(allRopeAndPins.Count-1);
    }

    public void RemovePinsAndRope(int id)
    {
        if (id >= allRopeAndPins.Count ) return;
        RopeAndPins ropesAndPin = allRopeAndPins[id];
        allRopeAndPins.RemoveAt(id);
        Destroy(ropesAndPin.leftPin.gameObject);
        Destroy(ropesAndPin.rightPin.gameObject);
        Destroy(ropesAndPin.rope);
        
        
        for (int i = 0; i < allRopeAndPins.Count; i++)
        {
            allRopeAndPins[i].leftPin.Init(i,true);
            allRopeAndPins[i].rightPin.Init(i,false);
        }
    }

    public void ShowPinsAndRope(int id, bool _show)
    {
        RopeAndPins ropesAndPin = allRopeAndPins[id];
        ropesAndPin.rightPin.gameObject.SetActive(_show);
        ropesAndPin.leftPin.gameObject.SetActive(_show);
        ropesAndPin.isTotallyHiden = !_show;
        allRopeAndPins[id] = ropesAndPin;
        ropesAndPin.rope.GetComponent<LineRenderer>().enabled = _show;
    }

    public void ShowRope(int _id,bool _show,bool _isLeftAsking)
    {
        
        RopeAndPins ropesAndPin = allRopeAndPins[_id];

        if (_isLeftAsking) ropesAndPin.leftAskShow = _show;
        else ropesAndPin.rightAskShow = _show;
        allRopeAndPins[_id] = ropesAndPin;
        
        ropesAndPin.rope.GetComponent<LineRenderer>().enabled = ropesAndPin.leftAskShow && ropesAndPin.rightAskShow && !ropesAndPin.isTotallyHiden;
    }

    private void Update()
    {
        mouseFollow.position = camera.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 2));
    }
}
