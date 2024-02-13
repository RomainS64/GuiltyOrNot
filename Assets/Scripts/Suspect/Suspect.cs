using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[Serializable]
public struct Suspect
{
    public int visualSeed;
    public string visualPrompt;

    public bool isEliminated;
    
    public string name;
    public string surname;
    
    public int age;
    public DateTime date;
    
    public string gender;
    public int height;

    public Texture portrait;

    public InternetHistory internetHistory;
    public BankAccountHistory bankAccountHistory;
    public CriminalRecord criminalRecord;
    
}

public struct InternetHistory
{
    public List<string> stringList;
}

public struct BankAccountHistory
{
    public float current;
    public float saving;
    public List<KeyValuePair<string,float>> transactions;
}
public struct CriminalRecord { }