using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewDocUI : MonoSingleton<NewDocUI>
{
    [SerializeField] private int[] thresholds;
    private Animator animator;

    public void NotifyThreshold(int _amount,bool _isElimination)
    {
        if (_isElimination && thresholds.Contains(_amount))
        {
            animator.SetTrigger("Unlock");
            AudioManager.instance.audioEvents["New Document"].Play();
        }
    }

    private void Start()
    {
        animator = GetComponent<Animator>();
    }
}
