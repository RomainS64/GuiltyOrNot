using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SuspectGenerator : MonoBehaviour
{
    public static Suspect GenerateSuspect()
    {
        bool isAMan = Random.Range(0,2) == 0;
        int age = Random.Range(18, 80);
        Suspect newSuspect = new Suspect()
        {
            name = isAMan ? NameAndSurnameParser.GetManName() : NameAndSurnameParser.GetWomanName(),
            surname = NameAndSurnameParser.GetSurname(),
            age = age,
            date = DateTime.Now - TimeSpan.FromDays(Random.Range(0f, 364))- TimeSpan.FromDays(365*age),
            sexe = isAMan ? "Male" : "Female",
            height = Random.Range(140, 200),
            criminalRecord = "todo",
            medicalReport = "todo",
            psychologicalReport = "todo"
        };
        return newSuspect;
    }
}

