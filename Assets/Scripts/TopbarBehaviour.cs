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

    [SerializeField] private GameObject[] photos;
    [SerializeField] private GameObject groupPart;
    private int groupPartIndex = 6;
    private bool isDisplayed;
    

    void Start()
    {
        animator = GetComponent<Animator>();
        for (int i = 0; i < 50; i++)
        {
            int rdm = Random.Range(0, photos.Length);
            photos[rdm].transform.SetAsLastSibling();
        }
        groupPart.transform.SetSiblingIndex(groupPartIndex);
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
