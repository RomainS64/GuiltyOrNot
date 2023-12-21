using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SuspectGenerator : MonoSingleton<SuspectGenerator>
{
    [SerializeField] private int minAge;
    [SerializeField] private int maxAge;
    [SerializeField] private AnimationCurve ageDistribution;
    
    [SerializeField] private int minSize;
    [SerializeField] private int maxSize;
    [SerializeField] private AnimationCurve sizeDistribution;
    
    
    public static float weightedRandom(float min,float max,AnimationCurve distribution)=>Mathf.Lerp(min,max,distribution.Evaluate(Random.Range(0, 2)));
    public Suspect GenerateSuspect()
    {
        bool isAMan = Random.Range(0,2) == 0;
        int age = (int)weightedRandom(minAge, maxAge, ageDistribution);
        Suspect newSuspect = new Suspect()
        {
            name = isAMan ? NameAndSurnameParser.GetManName() : NameAndSurnameParser.GetWomanName(),
            surname = NameAndSurnameParser.GetSurname(),
            age = age,
            date = DateTime.Now - TimeSpan.FromDays(Random.Range(0f, 364))- TimeSpan.FromDays(365*age),
            sexe = isAMan ? "Male" : "Female",
            height = (int)weightedRandom(minSize, maxSize, sizeDistribution),
        };
        return newSuspect;
    }
}

