using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public struct Weights
{
    [SerializeField] public int manProbabilityWeight;
    [SerializeField] public int womanProbabilityWeight;
}
[Serializable]
public struct VisualAttribute
{
    [FormerlySerializedAs("attributeLabel")] [SerializeField]
    public string prompt;

    [SerializeField] public Weights weights;

}
[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/SuspectVisualAttributes")]
public class SuspectVisualAttributes : ScriptableObject
{
    [SerializeField]
    public VisualAttribute[] attributes;
    
}
