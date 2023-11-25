using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public struct Suspect
{
    public int visualSeed;
    public string visualPrompt;

    public string name;
    public string surname;
    
    public int age;
    public DateTime date;
    public string sexe;
    public int height;
    
    
    public string criminalRecord;
    public string psychologicalReport;
    public string medicalReport;

    public Dictionary<EmotionType, Texture> emotions;
    public bool isFullyLoaded;

}
