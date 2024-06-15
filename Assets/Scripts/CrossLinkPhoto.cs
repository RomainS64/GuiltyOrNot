using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossLinkPhoto : MonoBehaviour
{
    public int Id;
    private Animator animator;
    private string crossedParameter = "cross";
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    public void SetCross(bool isCross)
    {
        animator.SetBool(crossedParameter,isCross);
    }
}
