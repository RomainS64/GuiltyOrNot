using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ThresholdSpawnableObject : MonoBehaviour
{
    [SerializeField] private int spawnThreshold;

    public void NotifyThreshold(int _amount)=>gameObject.SetActive(spawnThreshold>=_amount);
}
