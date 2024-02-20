using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


[Serializable]
public struct CriminalRecordVisual
{
    public TMP_Text text;
    public TMP_Text date;
}
public class CriminalRecordPaper : MonoBehaviour
{
    [SerializeField] private int playerId;
    public int GetPlayerId() => playerId;

    [SerializeField] private CriminalRecordVisual[] transactions;
    
    public void SetCriminalRecords(List<KeyValuePair<string,string>> _records)
    {
        int i = 0;
        foreach (var record in _records)
        {
            transactions[i].date.text = record.Value;
            transactions[i].text.text = record.Key;
            ++i;
        }
    }
}
