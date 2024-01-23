using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TopbarBehaviour : MonoBehaviour
{
    private Animator animator;
    private const string showParameter = "Show";
    private static readonly int Show = Animator.StringToHash(showParameter);
    
    [SerializeField]
    private float zoomThreshold;
    private bool isDisplayed;
    

    void Start()
    {
        animator = GetComponent<Animator>();
    }
    void Update()
    {
        if ((CameraHandler.Instance.ZoomLevel < zoomThreshold && !isDisplayed) || (CameraHandler.Instance.ZoomLevel >= zoomThreshold && isDisplayed))
        {
            isDisplayed = !isDisplayed;
            animator.SetBool(Show,isDisplayed);
        }
    }
}
