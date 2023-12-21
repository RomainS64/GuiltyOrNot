using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowObject : MonoBehaviour
{
    [SerializeField]private bool followX;
    [SerializeField]private bool followY;
    [SerializeField]private bool followZ;
    [SerializeField]private Vector3 offset;
    [SerializeField] private Transform followingObject;
    [SerializeField] private Transform objectToFollow;
    [Range(0,1)] [SerializeField] private float lerpValue;
    void Update()
    {
        Vector3 newPos = new Vector3(
            followX ? objectToFollow.position.x + offset.x : followingObject.position.x,
            followY ? objectToFollow.position.y + offset.y : followingObject.position.y,
            followZ ? objectToFollow.position.z + offset.z : followingObject.position.z);
        Vector3.Lerp(followingObject.transform.position, newPos, lerpValue);
        followingObject.transform.position = newPos;
    }
}
