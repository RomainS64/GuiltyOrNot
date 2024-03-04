using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField] private bool followZoom;
    [SerializeField] private float minZoom;
    [SerializeField] private float maxZoom;
    
    [SerializeField]private bool followX;
    [SerializeField]private bool followY;
    [SerializeField]private bool followZ;
    [SerializeField]private Vector3 offset;
    [SerializeField] private Transform followingObject;
    [SerializeField] private Transform objectToFollow;
    [Range(0,1)] [SerializeField] private float lerpValue;

    private void Start()
    {
        float scale = Mathf.Lerp(minZoom, maxZoom, CameraHandler.Instance.ZoomLevel);
        transform.localScale = new Vector3(scale,scale,1);
    }

    void Update()
    {
        if (followZoom)
        {
            float scale = Mathf.Lerp(minZoom, maxZoom, CameraHandler.Instance.ZoomLevel);
            transform.localScale = new Vector3(scale,scale,1);
        }
        
        Vector3 newPos = new Vector3(
            followX ? objectToFollow.position.x + offset.x : followingObject.position.x,
            followY ? objectToFollow.position.y + offset.y : followingObject.position.y,
            followZ ? objectToFollow.position.z + offset.z : followingObject.position.z);
        Vector3.Lerp(followingObject.transform.position, newPos, lerpValue);
        followingObject.transform.position = newPos;
    }
}
