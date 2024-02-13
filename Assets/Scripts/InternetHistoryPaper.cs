using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class InternetHistoryPaper : MonoBehaviour
{
    [SerializeField] private int playerId;
    public int GetPlayerId() => playerId;

    [SerializeField] private TMP_Text[] historyTexts;

    public void SetInternetHistory(List<string> _history)
    {
        
        for (int i = 0; i < _history.Count; i++)
        {
            historyTexts[i].text = _history[i];
        }
    }
}
