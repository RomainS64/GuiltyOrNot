using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

public class GenerateRandomNumber : MonoBehaviour
{
    [SerializeField] private TMP_Text text;
    [SerializeField] private int randomDigitNumber;
    void Start()
    {
        text.text = String.Empty;
        for (int i = 0; i < randomDigitNumber; i++)
        {
            text.text +=  $"{Random.Range(0, 10)}";
        }
    }
    
}
